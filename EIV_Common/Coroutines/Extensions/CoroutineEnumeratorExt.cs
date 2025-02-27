using Godot;
using System.Numerics;
using static Godot.GodotObject;

namespace EIV_Common.Coroutines.Extensions;

public static class CoroutineEnumeratorExt
{
    #region Simple Enumerators
    public static IEnumerator<double> Empty()
    {
        yield return 0f;
    }

    public static IEnumerator<double> DelayedCall(TimeSpan timeSpan, Action action)
    {
        yield return timeSpan.TotalSeconds;
        action();
    }

    public static IEnumerator<double> CallContinuously(TimeSpan timeSpan, Action action)
    {
        while (true)
        {
            yield return timeSpan.TotalSeconds;
            action();
        }
    }
    #endregion
    #region StartWhenDone
    public static IEnumerator<double> StartWhenDone(Func<bool>? evaluatorFunc, bool continueOn, IEnumerator<double> pausedProc)
    {
        if (evaluatorFunc == null)
            yield break;
        while (evaluatorFunc() == continueOn)
        {
            yield return double.NegativeInfinity;
        }
        CoroutineStaticExt.StartIfNotExists();
        CoroutineStaticExt.StaticWorker!.ReplacementObject = pausedProc;
        CoroutineStaticExt.StaticWorker!.ReplacementFunction = ReturnTmpRefForRepFunc;
        yield return float.NaN;
    }

    public static IEnumerator<double> StartWhenTDone<T>(Func<T>? evaluatorFunc, T continueOn, IEnumerator<double> pausedProc) where T : INumber<T>
    {
        if (evaluatorFunc == null)
            yield break;
        while (evaluatorFunc() != continueOn)
        {
            yield return double.NegativeInfinity;
        }
        CoroutineStaticExt.StartIfNotExists();
        CoroutineStaticExt.StaticWorker!.ReplacementObject = pausedProc;
        CoroutineStaticExt.StaticWorker!.ReplacementFunction = ReturnTmpRefForRepFunc;
        yield return float.NaN;
    }
    public static IEnumerator<double> StartWhenDone(Coroutine? coroutine, IEnumerator<double> pausedProc)
    {
        yield break;
        /*
        if (!coroutine.HasValue)
            yield break;
        coroutine = Instance.CustomCoroutines.Where(x => x.Equals(coroutine)).FirstOrDefault();
        while (coroutine.Value.IsSuccess != true)
        {
            coroutine = Instance.CustomCoroutines.Where(x => x.Equals(coroutine)).FirstOrDefault();
            yield return double.NegativeInfinity;
        }
        CoroutineStaticExt.StaticWorker!.ReplacementObject = pausedProc;
        ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(ReturnTmpRefForRepFunc);
        yield return double.NaN;
        */
    }
    public static IEnumerator<double> StartWhenDoneSignal(ValueTuple<GodotObject, string>? objSignal, IEnumerator<double> pausedProc)
    {
        if (objSignal == null)
            yield break;
        objSignal.Value.Item1.Connect(objSignal.Value.Item2, Callable.From(() => { GD.Print("Signal emitted! " + objSignal.Value.Item2); }), (uint)ConnectFlags.OneShot);
        while (objSignal.Value.Item1.ToSignal(objSignal.Value.Item1, objSignal.Value.Item2).IsCompleted != true)
        {
            yield return double.NegativeInfinity;
        }
        CoroutineStaticExt.StartIfNotExists();
        CoroutineStaticExt.StaticWorker!.ReplacementObject = pausedProc;
        CoroutineStaticExt.StaticWorker!.ReplacementFunction = ReturnTmpRefForRepFunc;
        yield return double.NaN;
    }
    #endregion
    #region Helpers
    public static IEnumerator<double> ReturnTmpRefForRepFunc(IEnumerator<double> coptr)
    {
        CoroutineStaticExt.StartIfNotExists();
        if (CoroutineStaticExt.StaticWorker!.ReplacementObject == null)
            return Empty();
        if (CoroutineStaticExt.StaticWorker!.ReplacementObject is IEnumerator<double> that && that != null)
            return that;
        return Empty();
    }

    public static IEnumerator<double> WaitUntilFalseHelper(IEnumerator<double> coptr)
    {
        CoroutineStaticExt.StartIfNotExists();
        return StartWhenDone(CoroutineStaticExt.StaticWorker!.ReplacementObject as Func<bool>, true, coptr);
    }

    public static IEnumerator<double> WaitUntilTrueHelper(IEnumerator<double> coptr)
    {
        CoroutineStaticExt.StartIfNotExists();
        return StartWhenDone(CoroutineStaticExt.StaticWorker!.ReplacementObject as Func<bool>, false, coptr);
    }

    public static IEnumerator<double> WaitUntilTHelper<T>(IEnumerator<double> coptr) where T : INumber<T>
    {
        CoroutineStaticExt.StartIfNotExists();
        return StartWhenTDone<T>(CoroutineStaticExt.StaticWorker!.ReplacementObject as Func<T>, T.Zero, coptr);
    }

    public static IEnumerator<double> StartAfterCoroutineHelper(IEnumerator<double> coptr)
    {
        CoroutineStaticExt.StartIfNotExists();
        return StartWhenDone((Coroutine?)CoroutineStaticExt.StaticWorker!.ReplacementObject, coptr);
    }

    public static IEnumerator<double> WaitUntilSignalHelper(IEnumerator<double> coroutineEnumerator)
    {
        CoroutineStaticExt.StartIfNotExists();
        return StartWhenDoneSignal((ValueTuple<GodotObject, string>?)CoroutineStaticExt.StaticWorker!.ReplacementObject, coroutineEnumerator);
    }
    #endregion
}
