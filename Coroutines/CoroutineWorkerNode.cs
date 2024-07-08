using Godot;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace EIV_Common.Coroutines
{
    public partial class CoroutineWorkerNode : Node
    {
        static object? _tmpRef;
        static Func<IEnumerator<double>, IEnumerator<double>>? ReplacementFunction;
        public static CoroutineWorkerNode Instance;
        List<double> Delays = [];
        List<Coroutine> ProcessCoroutines = [];
        List<Coroutine> PhysicsCoroutines = [];

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

        public void Quit()
        {
            Kill();
        }

        void ProcessCorUpdate(double deltaTime)
        {
            _mutex.Lock();
            {
                for (int i = 0; i < Instance.ProcessCoroutines.Count; i++)
                {
                    var item = Instance.ProcessCoroutines[i];
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
                    Instance.ProcessCoroutines[i] = item;
                }
                _mutex.Unlock();
            }
            Kill();
        }

        void ProcessPhysicsCorUpdate(double deltaTime)
        {
            _mutex.Lock();
            {
                for (int i = 0; i < Instance.PhysicsCoroutines.Count; i++)
                {
                    var item = Instance.PhysicsCoroutines[i];
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
                    Instance.PhysicsCoroutines[i] = item;
                }
                _mutex.Unlock();
            }
            Kill();
        }

        private void Kill()
        {
            _mutex.Lock();
            {
                for (int i = 0; i < Instance.ProcessCoroutines.Count; i++)
                {
                    var item = Instance.ProcessCoroutines[i];
                    if (item.ShouldKill)
                    {
                        Instance.ProcessCoroutines.Remove(item);
                        Instance.Delays.RemoveAt(i);
                    }
                }
                for (int i = 0; i < Instance.PhysicsCoroutines.Count; i++)
                {
                    var item = Instance.PhysicsCoroutines[i];
                    if (item.ShouldKill)
                    {
                        Instance.PhysicsCoroutines.Remove(item);
                        Instance.Delays.RemoveAt(i);
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
            Delays[index] = coroutine.Enumerator.Current;
            return result;
        }
        #endregion
        #region Coroutine Creators
        public static Coroutine StartCoroutine(IEnumerator<double> objects, CoroutineType type = CoroutineType.Process)
        {
            Coroutine coroutine = new(objects, type);
            switch (type)
            {
                case CoroutineType.Process:
                    Instance.ProcessCoroutines.Add(coroutine);
                    break;
                case CoroutineType.PhysicsProcess:
                    Instance.PhysicsCoroutines.Add(coroutine);
                    break;
                default:
                    break;
            }
            
            Instance.Delays.Add(0);
            return coroutine;
        }
        public static Coroutine CallDelayed(TimeSpan timeSpan, Action action, CoroutineType type = CoroutineType.Process)
        {
            return StartCoroutine(Instance._DelayedCall(timeSpan, action), type);
        }
        public static Coroutine CallContinuously(Action action, CoroutineType type = CoroutineType.Process)
        {
            return StartCoroutine(Instance._CallContinuously(TimeSpan.Zero, action), type);
        }
        public static Coroutine CallPeriodically(TimeSpan timeSpan, Action action, CoroutineType type = CoroutineType.Process)
        {
            return StartCoroutine(Instance._CallContinuously(timeSpan, action), type);
        }
        #endregion
        #region Static Helpers
        private static IEnumerator<double> ReturnTmpRefForRepFunc(IEnumerator<double> coptr)
        {
            return _tmpRef as IEnumerator<double>;
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
            return _StartWhenDone((Coroutine)_tmpRef, coptr);
        }

        private static IEnumerator<double> WaitUntilSignalHelper(IEnumerator<double> coptr)
        {
            return _StartWhenDoneSignal((ValueTuple<GodotObject, string>)_tmpRef, coptr);
        }
        #endregion
        #region Static IEnumerators
        private static IEnumerator<double> _StartWhenDone(Func<bool> evaluatorFunc, bool continueOn, IEnumerator<double> pausedProc)
        {
            while (evaluatorFunc() == continueOn)
            {
                yield return double.NegativeInfinity;
            }
            _tmpRef = pausedProc;
            ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(ReturnTmpRefForRepFunc);
            yield return float.NaN;
        }

        private static IEnumerator<double> _StartWhenTDone<T>(Func<T> evaluatorFunc, T continueOn, IEnumerator<double> pausedProc) where T : INumber<T>
        {
            while (evaluatorFunc() != continueOn)
            {
                yield return double.NegativeInfinity;
            }
            _tmpRef = pausedProc;
            ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(ReturnTmpRefForRepFunc);
            yield return float.NaN;
        }

        private static IEnumerator<double> _StartWhenDone(Coroutine coroutine, IEnumerator<double> pausedProc)
        {
            coroutine = GetCoroutine(coroutine);
            while (coroutine.IsSuccess != true)
            {
                coroutine = GetCoroutine(coroutine);
                yield return double.NegativeInfinity;
            }
            _tmpRef = pausedProc;
            ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(ReturnTmpRefForRepFunc);
            yield return double.NaN;
        }


        private static IEnumerator<double> _StartWhenDoneSignal(ValueTuple<GodotObject, string> ObjSignal, IEnumerator<double> pausedProc)
        {
            ObjSignal.Item1.Connect(ObjSignal.Item2, Callable.From(() => { GD.Print("Signal emitted! " + ObjSignal.Item2 ); } ), (uint)ConnectFlags.OneShot);
            while (ObjSignal.Item1.ToSignal(ObjSignal.Item1, ObjSignal.Item2).IsCompleted != true)
            {
                yield return double.NegativeInfinity;
            }
            _tmpRef = pausedProc;
            ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(ReturnTmpRefForRepFunc);
            yield return double.NaN;
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
        #region Static Floats
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
            var cor = GetCoroutine(coroutine);
            if (cor.IsSuccess)
            {
                return 0;
            }
            _tmpRef = cor;
            ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(StartAfterCoroutineHelper);
            return double.NaN;
        }

        public static double WaitUntilSignal(GodotObject godotObject, string Signal) 
        {
            if (godotObject.ToSignal(godotObject, Signal).IsCompleted)
            {
                return double.NaN;
            }
            _tmpRef = new ValueTuple<GodotObject, string>(godotObject, Signal);
            ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(WaitUntilSignalHelper);
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
            var index = GetCoroutineIndex(coroutine);
            if (index == -1)
                return;
            _mutex.Lock();
            {
                Coroutine cor;
                switch (coroutine.CoroutineType)
                {
                    case CoroutineType.Process:
                        cor = Instance.ProcessCoroutines[index];
                        cor.ShouldKill = true;
                        Instance.ProcessCoroutines[index] = cor;
                        break;
                    case CoroutineType.PhysicsProcess:
                        cor = Instance.PhysicsCoroutines[index];
                        cor.ShouldKill = true;
                        Instance.PhysicsCoroutines[index] = cor;
                        break;
                    default:
                        break;
                }
                _mutex.Unlock();
            }
        }

        public static int GetCoroutineIndex(Coroutine coroutine)
        {
            switch (coroutine.CoroutineType)
            {
                case CoroutineType.Process:
                    return Instance.ProcessCoroutines.FindIndex(x => x.Equals(coroutine));
                case CoroutineType.PhysicsProcess:
                    return Instance.PhysicsCoroutines.FindIndex(x => x.Equals(coroutine));
                default:
                    return -1;
            }
        }

        public static Coroutine GetCoroutine(Coroutine coroutine)
        {
            switch (coroutine.CoroutineType)
            {
                case CoroutineType.Process:
                    return Instance.ProcessCoroutines.Where(x => x.Equals(coroutine)).FirstOrDefault();
                case CoroutineType.PhysicsProcess:
                    return Instance.PhysicsCoroutines.Where(x => x.Equals(coroutine)).FirstOrDefault();
                default:
                    return default;
            }
        }

        #endregion
    }
}
