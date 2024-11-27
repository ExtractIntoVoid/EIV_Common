using EIV_Common.Logger;
using System.Diagnostics;
using System.Numerics;

namespace EIV_Common.Coroutines;

public class CoroutineWorkerCustom
{
    static object? _tmpRef;
    static Func<IEnumerator<double>, IEnumerator<double>>? ReplacementFunction;
    static CoroutineWorkerCustom? instance;
    public static CoroutineWorkerCustom Instance
    {
        get
        {
            if (instance == null)
                return instance = new();
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    List<double> Delays = [];
    List<Coroutine> CustomCoroutines = [];

    Thread UpdateThread;
    Stopwatch Watch;
    const double updateRate = 1f / 30f;  // 30 "fps"
    double prevTime = 0f;
    double accumulator = 0f;
    double TotalTime = 0f;
    public bool PauseUpdate = false;
    private readonly Mutex _mutex = new();
    public CoroutineWorkerCustom()
    {
        Instance = this;
        Watch = Stopwatch.StartNew();
        prevTime = Watch.ElapsedMilliseconds / 1000f;
        UpdateThread = new(Update)
        {
            IsBackground = true
        };
        UpdateThread.Start();
    }

    #region Needed stuff for running

    public void Quit()
    {
        Kill();
        UpdateThread.Interrupt();
        MainLog.logger?.Debug("Was run until " + TotalTime);
    }

    void Update()
    {
        while (UpdateThread.ThreadState == System.Threading.ThreadState.Background)
        {
            if (PauseUpdate)
                continue;

            double currTime = Watch.ElapsedMilliseconds / 1000f;
            accumulator += currTime - prevTime;
            prevTime = currTime;

            if (accumulator > updateRate)
            {
                accumulator -= updateRate;
                CustomCorUpdate(updateRate);
                TotalTime += updateRate;
            }
        }
    }


    void CustomCorUpdate(double deltaTime)
    {
        Kill();
        if (_mutex.WaitOne(1))
        {

            for (int i = 0; i < Instance.CustomCoroutines.Count; i++)
            {
                Coroutine item = Instance.CustomCoroutines[i];
                MainLog.logger?.Debug(Instance.Delays[i] + " " + i + " " + deltaTime + " " + item);
                if (Instance.Delays[i] > 0f)
                    Instance.Delays[i] -= deltaTime;
                if (Instance.Delays[i] <= 0f)
                {
                    CoroutineWork(ref item, i);
                }
                if (double.IsNaN(Instance.Delays[i]))
                {
                    if (ReplacementFunction != null)
                    {
                        item.Enumerator = ReplacementFunction(item.Enumerator);
                        CoroutineWork(ref item, i);
                        ReplacementFunction = null;
                    }
                }
                Instance.CustomCoroutines[i] = item;
            }
            _mutex.ReleaseMutex();
        }
        Kill();
    }

    private void Kill()
    {
        if (_mutex.WaitOne(1))
        {
            for (int i = 0; i < Instance.CustomCoroutines.Count; i++)
            {
                Coroutine item = Instance.CustomCoroutines[i];
                if (item.ShouldKill)
                {
                    Instance.CustomCoroutines.Remove(item);
                    Instance.Delays.RemoveAt(i);
                }
            }
            _mutex.ReleaseMutex();
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
        Instance.Delays[index] = coroutine.Enumerator.Current;
        return result;
    }
    #endregion
    #region Coroutine Creators
    public static Coroutine StartCoroutine(IEnumerator<double> objects, CoroutineType type = CoroutineType.Custom)
    {
        Coroutine coroutine = new(objects, type);
        MainLog.logger?.Debug(coroutine.ToString());
        Instance.CustomCoroutines.Add(coroutine);
        Instance.Delays.Add(0);
        return coroutine;
    }
    public static Coroutine CallDelayed(TimeSpan timeSpan, Action action, CoroutineType type = CoroutineType.Custom)
    {
        return StartCoroutine(Instance._DelayedCall(timeSpan, action), type);
    }
    public static Coroutine CallContinuously(Action action, CoroutineType type = CoroutineType.Custom)
    {
        return StartCoroutine(Instance._CallContinuously(TimeSpan.Zero, action), type);
    }
    public static Coroutine CallPeriodically(TimeSpan timeSpan, Action action, CoroutineType type = CoroutineType.Custom)
    {
        return StartCoroutine(Instance._CallContinuously(timeSpan, action), type);
    }
    #endregion
    #region Static Helpers
    private static IEnumerator<double> ReturnTmpRefForRepFunc(IEnumerator<double> coptr)
    {
        if (_tmpRef == null)
            return _Empty();
        if (_tmpRef is IEnumerator<double> that && that != null)
            return that;
        return _Empty();
    }

    private static IEnumerator<double> WaitUntilFalseHelper(IEnumerator<double> coptr)
    {
        return _StartWhenDone(_tmpRef as Func<bool>, true, coptr);
    }

    private static IEnumerator<double> WaitUntilTrueHelper(IEnumerator<double> coptr)
    {
        return _StartWhenDone(_tmpRef as Func<bool>, false, coptr);
    }

    private static IEnumerator<double> WaitUntilTHelper<T>(IEnumerator<double> coptr) where T : INumber<T>
    {
        return _StartWhenTDone<T>(_tmpRef as Func<T>, T.Zero, coptr);
    }

    private static IEnumerator<double> StartAfterCoroutineHelper(IEnumerator<double> coptr)
    {
        return _StartWhenDone((Coroutine?)_tmpRef, coptr);
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
        ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(ReturnTmpRefForRepFunc);
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
        ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(ReturnTmpRefForRepFunc);
        yield return float.NaN;
    }

    private static IEnumerator<double> _StartWhenDone(Coroutine? coroutine, IEnumerator<double> pausedProc)
    {
        if (!coroutine.HasValue)
            yield break;
        coroutine = Instance.CustomCoroutines.Where(x => x.Equals(coroutine)).FirstOrDefault();
        while (coroutine.Value.IsSuccess != true)
        {
            coroutine = Instance.CustomCoroutines.Where(x => x.Equals(coroutine)).FirstOrDefault();
            yield return double.NegativeInfinity;
        }
        _tmpRef = pausedProc;
        ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(ReturnTmpRefForRepFunc);
        yield return double.NaN;
    }
    private static IEnumerator<double> _Empty()
    {
        yield return 0f;
    }
    #endregion
    #region IEnumerators
    private IEnumerator<double> _DelayedCall(TimeSpan timeSpan, Action action)
    {
        yield return timeSpan.TotalSeconds;
        action();
    }
    private IEnumerator<double> _CallContinuously(TimeSpan timeSpan, Action action)
    {
        while (true)
        {
            yield return timeSpan.TotalSeconds;
            action();
        }
    }
    #endregion
    #region Static Doubles
    public static double WaitUntilFalse(Func<bool> evaluatorFunc)
    {
        if (evaluatorFunc == null || !evaluatorFunc())
        {
            return double.NaN;
        }
        _tmpRef = evaluatorFunc;
        ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(WaitUntilFalseHelper);
        return double.NaN;
    }

    public static double WaitUntilTrue(Func<bool> evaluatorFunc)
    {
        if (evaluatorFunc == null || evaluatorFunc())
        {
            return double.NaN;
        }
        _tmpRef = evaluatorFunc;
        ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(WaitUntilTrueHelper);
        return double.NaN;
    }

    public static double WaitUntilZero<T>(Func<T> evaluatorFunc) where T : INumber<T>
    {
        if (evaluatorFunc() == T.Zero)
        {
            return double.NaN;
        }
        _tmpRef = evaluatorFunc;
        ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(WaitUntilTHelper<T>);
        return double.NaN;
    }

    public static double StartAfterCoroutine(Coroutine coroutine)
    {
        Coroutine cor = Instance.CustomCoroutines.Where(x => x.Equals(coroutine)).FirstOrDefault();
        if (cor.IsSuccess)
        {
            return 0;
        }
        _tmpRef = cor;
        ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(StartAfterCoroutineHelper);
        return double.NaN;
    }
    #endregion
    #region Statis funcs

    public static void KillCoroutines(params Coroutine[] coroutines)
    {
        for (int i = 0; i < coroutines.Length; i++)
        {
            KillCoroutineInstance(coroutines[i]);
        }
    }

    public static void KillCoroutineInstance(Coroutine coroutine)
    {
        Instance.KillCoroutine(coroutine);
    }
    public void KillCoroutine(Coroutine coroutine)
    {
        int index = Instance.CustomCoroutines.FindIndex(x => x.Equals(coroutine));
        if (index == -1)
        {
            MainLog.logger?.Debug("No Coroutine!");
            return;
        }
        if (_mutex.WaitOne(1))
        {
            Coroutine cor = Instance.CustomCoroutines[index];
            cor.ShouldKill = true;
            Instance.CustomCoroutines[index] = cor;
            _mutex.ReleaseMutex();
        }
    }

    public static void PauseCoroutineInstance(Coroutine coroutine)
    {
        Instance.PauseCoroutine(coroutine);
    }
    public void PauseCoroutine(Coroutine coroutine)
    {
        int index = Instance.CustomCoroutines.FindIndex(x => x.Equals(coroutine));
        if (index == -1)
        {
            MainLog.logger?.Debug("No Coroutine!");
            return;
        }
        if (_mutex.WaitOne(1))
        {
            Coroutine cor = Instance.CustomCoroutines[index];
            cor.ShouldPause = true;
            Instance.CustomCoroutines[index] = cor;
            _mutex.ReleaseMutex();
        }
    }
    #endregion
}
