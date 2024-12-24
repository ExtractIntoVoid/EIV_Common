using Godot;
using System.Numerics;

namespace EIV_Common.Coroutines;

public partial class CoroutineWorkerNode : Node
{
    static object? _tmpRef;
    static Func<IEnumerator<double>, IEnumerator<double>>? _replacementFunction;
    static CoroutineWorkerNode? _instance;
    public static CoroutineWorkerNode Instance
    {
        get
        {
            if (_instance == null)
                _instance = new();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
    List<double> _delays = [];
    List<Coroutine> _processCoroutines = [];
    List<Coroutine> _physicsCoroutines = [];

    private readonly Godot.Mutex _mutex = new();

    #region NodeWrite
    public override void _Ready()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        ProcessCorUpdate(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        ProcessPhysicsCorUpdate(delta);
    }

    public override void _ExitTree()
    {
        Quit();
    }

    #endregion
    #region Needed stuff for running

    public void Quit() => Kill();

    void ProcessCorUpdate(double deltaTime)
    {
        _mutex.Lock();
        {
            for (int i = 0; i < Instance._processCoroutines.Count; i++)
            {
                Coroutine item = Instance._processCoroutines[i];
                if (Instance._delays[i] > 0f)
                    Instance._delays[i] -= deltaTime;
                if (Instance._delays[i] <= 0f)
                {
                    CoroutineWork(ref item, i);
                }
                if (double.IsNaN(Instance._delays[i]))
                {
                    if (_replacementFunction != null)
                    {
                        item.Enumerator = _replacementFunction(item.Enumerator);
                        CoroutineWork(ref item, i);
                        _replacementFunction = null;
                    }
                }
                Instance._processCoroutines[i] = item;
            }
            _mutex.Unlock();
        }
        Kill();
    }

    void ProcessPhysicsCorUpdate(double deltaTime)
    {
        _mutex.Lock();
        {
            for (int i = 0; i < Instance._physicsCoroutines.Count; i++)
            {
                Coroutine item = Instance._physicsCoroutines[i];
                if (Instance._delays[i] > 0f)
                    Instance._delays[i] -= deltaTime;
                if (Instance._delays[i] <= 0f)
                {
                    CoroutineWork(ref item, i);
                }
                if (double.IsNaN(Instance._delays[i]))
                {
                    if (_replacementFunction != null)
                    {
                        item.Enumerator = _replacementFunction(item.Enumerator);
                        CoroutineWork(ref item, i);
                        _replacementFunction = null;
                    }
                }
                Instance._physicsCoroutines[i] = item;
            }
            _mutex.Unlock();
        }
        Kill();
    }

    private void Kill()
    {
        _mutex.Lock();
        {
            for (int i = 0; i < Instance._processCoroutines.Count; i++)
            {
                Coroutine item = Instance._processCoroutines[i];
                if (item.ShouldKill)
                {
                    Instance._processCoroutines.Remove(item);
                    Instance._delays.RemoveAt(i);
                }
            }
            for (int i = 0; i < Instance._physicsCoroutines.Count; i++)
            {
                Coroutine item = Instance._physicsCoroutines[i];
                if (item.ShouldKill)
                {
                    Instance._physicsCoroutines.Remove(item);
                    Instance._delays.RemoveAt(i);
                }
            }
            _mutex.Unlock();
        }
    }

    private void CoroutineWork(ref Coroutine coroutine, int index)
    {
        if (coroutine.ShouldKill)
            return;
        if (coroutine.ShouldPause)
            return;
        if (coroutine.IsSuccess)
            return;
        coroutine.IsRunning = true;
        if (!MoveNext(ref coroutine, index))
        {
            coroutine.IsRunning = false;
            coroutine.IsSuccess = true;
            coroutine.ShouldKill = true;
        }
    }

    private bool MoveNext(ref Coroutine coroutine, int index)
    {
        bool result = coroutine.Enumerator.MoveNext();
        _delays[index] = coroutine.Enumerator.Current;
        return result;
    }
    #endregion
    #region Coroutine Creators
    public static CoroutineHandle? StartCoroutine(IEnumerator<double> objects, CoroutineType type = CoroutineType.Process, string tag = "")
    {
        Coroutine coroutine = new(objects, type, tag);
        switch (type)
        {
            case CoroutineType.Process:
                Instance._processCoroutines.Add(coroutine);
                break;
            case CoroutineType.PhysicsProcess:
                Instance._physicsCoroutines.Add(coroutine);
                break;
            default:
                return null;
        }

        Instance._delays.Add(0);
        return coroutine;
    }
    public static CoroutineHandle? CallDelayed(TimeSpan timeSpan, Action action, CoroutineType type = CoroutineType.Process, string tag = "")
    {
        return StartCoroutine(_DelayedCall(timeSpan, action), type, tag);
    }
    public static CoroutineHandle? CallContinuously(Action action, CoroutineType type = CoroutineType.Process, string tag = "")
    {
        return StartCoroutine(_CallContinuously(TimeSpan.Zero, action), type, tag);
    }
    public static CoroutineHandle? CallPeriodically(TimeSpan timeSpan, Action action, CoroutineType type = CoroutineType.Process, string tag = "")
    {
        return StartCoroutine(_CallContinuously(timeSpan, action), type, tag);
    }
    #endregion
    #region Static Helpers
    private static IEnumerator<double> ReturnTmpRefForRepFunc(IEnumerator<double> coroutineEnumerator)
    {
        if (_tmpRef == null)
            return _Empty();
        if (_tmpRef is IEnumerator<double> that)
            return that;
        return _Empty();
    }

    private static IEnumerator<double> WaitUntilFalseHelper(IEnumerator<double> coroutineEnumerator)
    {
        return _StartWhenDone(_tmpRef as Func<bool>, true, coroutineEnumerator);
    }

    private static IEnumerator<double> WaitUntilTrueHelper(IEnumerator<double> coroutineEnumerator)
    {
        return _StartWhenDone(_tmpRef as Func<bool>, false, coroutineEnumerator);
    }
    private static IEnumerator<double> WaitUntilTHelper<T>(IEnumerator<double> coroutineEnumerator) where T : INumber<T>
    {
        return _StartWhenTDone<T>(_tmpRef as Func<T>, T.Zero, coroutineEnumerator);
    }
    private static IEnumerator<double> StartAfterCoroutineHelper(IEnumerator<double> coroutineEnumerator)
    {
        return _StartWhenDone((Coroutine?)_tmpRef, coroutineEnumerator);
    }

    private static IEnumerator<double> WaitUntilSignalHelper(IEnumerator<double> coroutineEnumerator)
    {
        return _StartWhenDoneSignal((ValueTuple<GodotObject, string>?)_tmpRef, coroutineEnumerator);
    }

    private static IEnumerator<double> _Empty()
    {
        yield return 0f;
    }
    #endregion
    #region Static IEnumerators
    private static IEnumerator<double> _StartWhenDone(Func<bool>? evaluatorFunc, bool continueOn, IEnumerator<double> pausedProc)
    {
        if (evaluatorFunc == null)
            yield break;
        while (evaluatorFunc() == continueOn)
        {
            yield return double.NegativeInfinity;
        }
        _tmpRef = pausedProc;
        _replacementFunction = ReturnTmpRefForRepFunc;
        yield return float.NaN;
    }

    private static IEnumerator<double> _StartWhenTDone<T>(Func<T>? evaluatorFunc, T continueOn, IEnumerator<double> pausedProc) where T : INumber<T>
    {
        if (evaluatorFunc == null)
            yield break;
        while (evaluatorFunc() != continueOn)
        {
            yield return double.NegativeInfinity;
        }
        _tmpRef = pausedProc;
        _replacementFunction = ReturnTmpRefForRepFunc;
        yield return float.NaN;
    }

    private static IEnumerator<double> _StartWhenDone(Coroutine? coroutine, IEnumerator<double> pausedProc)
    {
        if (!coroutine.HasValue)
            yield break;
        coroutine = GetCoroutine(coroutine.Value);
        while (coroutine.Value.IsSuccess != true)
        {
            coroutine = GetCoroutine(coroutine.Value);
            yield return double.NegativeInfinity;
        }
        _tmpRef = pausedProc;
        _replacementFunction = ReturnTmpRefForRepFunc;
        yield return double.NaN;
    }


    private static IEnumerator<double> _StartWhenDoneSignal(ValueTuple<GodotObject, string>? objSignal, IEnumerator<double> pausedProc)
    {
        if (objSignal == null)
            yield break;
        objSignal.Value.Item1.Connect(objSignal.Value.Item2, Callable.From(() => { GD.Print("Signal emitted! " + objSignal.Value.Item2); }), (uint)ConnectFlags.OneShot);
        while (objSignal.Value.Item1.ToSignal(objSignal.Value.Item1, objSignal.Value.Item2).IsCompleted != true)
        {
            yield return double.NegativeInfinity;
        }
        _tmpRef = pausedProc;
        _replacementFunction = ReturnTmpRefForRepFunc;
        yield return double.NaN;
    }

    private static IEnumerator<double> _DelayedCall(TimeSpan timeSpan, Action action)
    {
        yield return timeSpan.TotalSeconds;
        action();
    }
    private static IEnumerator<double> _CallContinuously(TimeSpan timeSpan, Action action)
    {
        while (true)
        {
            yield return timeSpan.TotalSeconds;
            action();
        }
        // ReSharper disable once IteratorNeverReturns
    }
    #endregion
    #region Static Floats
    public static double WaitUntilFalse(Func<bool> evaluatorFunc)
    {
        if (!evaluatorFunc())
            return double.NaN;
        _tmpRef = evaluatorFunc;
        _replacementFunction = WaitUntilFalseHelper;
        return double.NaN;
    }

    public static double WaitUntilTrue(Func<bool> evaluatorFunc)
    {
        if (evaluatorFunc())
            return double.NaN;
        _tmpRef = evaluatorFunc;
        _replacementFunction = WaitUntilTrueHelper;
        return double.NaN;
    }

    public static double WaitUntilZero<T>(Func<T> evaluatorFunc) where T : INumber<T>
    {
        if (evaluatorFunc() == T.Zero)
        {
            return double.NaN;
        }
        _tmpRef = evaluatorFunc;
        _replacementFunction = WaitUntilTHelper<T>;
        return double.NaN;
    }

    public static double StartAfterCoroutine(CoroutineHandle coroutine)
    {
        Coroutine cor = GetCoroutine(coroutine);
        if (cor.IsSuccess)
            return double.NaN;
        _tmpRef = cor;
        _replacementFunction = StartAfterCoroutineHelper;
        return double.NaN;
    }

    public static double WaitUntilSignal(GodotObject godotObject, string signal)
    {
        if (godotObject.ToSignal(godotObject, signal).IsCompleted)
            return double.NaN;
        _tmpRef = new ValueTuple<GodotObject, string>(godotObject, signal);
        _replacementFunction = WaitUntilSignalHelper;
        return double.NaN;
    }

    #endregion
    #region Statis funcs

    private static int GetCoroutineIndex(CoroutineHandle coroutine)
    {
        return coroutine.CoroutineType switch
        {
            CoroutineType.Process => Instance._processCoroutines.FindIndex(x => x.Equals(coroutine)),
            CoroutineType.PhysicsProcess => Instance._physicsCoroutines.FindIndex(x => x.Equals(coroutine)),
            _ => -1,
        };
    }

    private static Coroutine GetCoroutine(CoroutineHandle coroutine)
    {
        return coroutine.CoroutineType switch
        {
            CoroutineType.Process => Instance._processCoroutines.FirstOrDefault(x => x.Equals(coroutine)),
            CoroutineType.PhysicsProcess => Instance._physicsCoroutines.FirstOrDefault(x => x.Equals(coroutine)),
            _ => default,
        };
    }

    public static void KillCoroutines(IList<CoroutineHandle> coroutines)
    {
        for (int i = 0; i < coroutines.Count; i++)
        {
            KillCoroutineInstance(coroutines[i]);
        }
    }

    public static void KillCoroutineInstance(CoroutineHandle coroutine)
    {
        Instance.KillCoroutine(coroutine);
    }

    public void KillCoroutine(CoroutineHandle coroutine)
    {
        int index = GetCoroutineIndex(coroutine);
        if (index == -1)
            return;
        _mutex.Lock();
        {
            Coroutine cor;
            switch (coroutine.CoroutineType)
            {
                case CoroutineType.Process:
                    cor = Instance._processCoroutines[index];
                    cor.ShouldKill = true;
                    Instance._processCoroutines[index] = cor;
                    break;
                case CoroutineType.PhysicsProcess:
                    cor = Instance._physicsCoroutines[index];
                    cor.ShouldKill = true;
                    Instance._physicsCoroutines[index] = cor;
                    break;
                default:
                    break;
            }
            _mutex.Unlock();
        }
    }

    public static void KillCoroutineTagInstance(string Tag)
    {
        Instance.KillCoroutineTag(Tag);
    }

    public void KillCoroutineTag(string tag)
    {
        _mutex.Lock();
        {
            KillCoroutines(Instance._processCoroutines.Where(x => x.Tag == tag).Select(x => (CoroutineHandle)x).ToList());
            KillCoroutines(Instance._processCoroutines.Where(x => x.Tag == tag).Select(x => (CoroutineHandle)x).ToList());
            _mutex.Unlock();
        }
    }

    /// <summary>
    /// Pausing or Resuming a Coroutine. 
    /// </summary>
    /// <param name="coroutine">The coroute</param>
    public static void PauseCoroutineInstance(CoroutineHandle coroutine)
    {
        Instance.PauseCoroutine(coroutine);
    }

    /// <summary>
    /// Pausing or Resuming a Coroutine. 
    /// </summary>
    /// <param name="coroutine">The coroute</param>
    public void PauseCoroutine(CoroutineHandle coroutine)
    {
        int index = GetCoroutineIndex(coroutine);
        if (index == -1)
            return;
        _mutex.Lock();
        {
            Coroutine cor;
            switch (coroutine.CoroutineType)
            {
                case CoroutineType.Process:
                    cor = Instance._processCoroutines[index];
                    cor.ShouldPause = !cor.ShouldPause;
                    Instance._processCoroutines[index] = cor;
                    break;
                case CoroutineType.PhysicsProcess:
                    cor = Instance._physicsCoroutines[index];
                    cor.ShouldPause = !cor.ShouldPause;
                    Instance._physicsCoroutines[index] = cor;
                    break;
            }
            _mutex.Unlock();
        }
    }
    public static bool IsCoroutineExistsInstance(CoroutineHandle coroutine)
    {

        return Instance.IsCoroutineExists(coroutine);
    }

    public bool IsCoroutineExists(CoroutineHandle coroutine)
    {
        bool IsExist = false;
        _mutex.Lock();
        {
            IsExist = GetCoroutineIndex(coroutine) != -1;
            _mutex.Unlock();
        }
        return IsExist;
    }

    public static bool IsCoroutineRunningInstance(CoroutineHandle coroutine)
    {
        return Instance.IsCoroutineRunning(coroutine);
    }

    public bool IsCoroutineRunning(CoroutineHandle coroutine)
    {
        bool isRunning = false;
        _mutex.Lock();
        {
            isRunning = GetCoroutine(coroutine).IsRunning;
            _mutex.Unlock();
        }
        return isRunning;
    }

    public static bool IsCoroutineSuccessInstance(CoroutineHandle coroutine)
    {
        return Instance.IsCoroutineSuccess(coroutine);
    }

    public bool IsCoroutineSuccess(CoroutineHandle coroutine)
    {
        bool success = false;
        _mutex.Lock();
        {
            success = GetCoroutine(coroutine).IsSuccess;
            _mutex.Unlock();
        }
        return success;
    }

    public static bool HasAnyCoroutines()
    {
        return Instance._processCoroutines.Count != 0 && Instance._physicsCoroutines.Count != 0;
    }


    #endregion
}
