using Godot;
using System.Numerics;

namespace EIV_Common.Coroutines.Extensions;

public static class CoroutineDoubleExt
{
    public static double WaitUntilFalse(Func<bool> evaluatorFunc)
    {
        if (evaluatorFunc == null || !evaluatorFunc())
        {
            return 0;
        }
        CoroutineStaticExt.StartIfNotExists();
        CoroutineStaticExt.StaticWorker!.ReplacementObject = evaluatorFunc;
        CoroutineStaticExt.StaticWorker!.ReplacementFunction = CoroutineEnumeratorExt.WaitUntilFalseHelper;
        return double.NaN;
    }

    public static double WaitUntilTrue(Func<bool> evaluatorFunc)
    {
        if (evaluatorFunc == null || evaluatorFunc())
        {
            return 0;
        }
        CoroutineStaticExt.StartIfNotExists();
        CoroutineStaticExt.StaticWorker!.ReplacementObject = evaluatorFunc;
        CoroutineStaticExt.StaticWorker!.ReplacementFunction = CoroutineEnumeratorExt.WaitUntilTrueHelper;
        return double.NaN;
    }

    public static double WaitUntilZero<T>(Func<T> evaluatorFunc) where T : INumber<T>
    {
        if (evaluatorFunc() == T.Zero)
        {
            return 0;
        }
        CoroutineStaticExt.StartIfNotExists();
        CoroutineStaticExt.StaticWorker!.ReplacementObject = evaluatorFunc;
        CoroutineStaticExt.StaticWorker!.ReplacementFunction = CoroutineEnumeratorExt.WaitUntilTHelper<T>;
        return double.NaN;
    }

    public static double StartAfterCoroutine(CoroutineHandle coroutine)
    {
        return 0;
        /*
        Coroutine cor = Instance.CustomCoroutines.FirstOrDefault(x => coroutine.Equals((CoroutineHandle)x));
        if (cor.IsSuccess)
        {
            return 0;
        }
        CoroutineStaticExt.StartIfNotExists();
        CoroutineStaticExt.StaticWorker!.ReplacementObject = cor;
        CoroutineStaticExt.StaticWorker!.ReplacementFunction = new Func<IEnumerator<double>, IEnumerator<double>>(CoroutineEnumeratorExt.StartAfterCoroutineHelper);
        return double.NaN;
        */
    }
    public static double WaitUntilSignal(GodotObject godotObject, string signal)
    {
        if (godotObject.ToSignal(godotObject, signal).IsCompleted)
            return 0;
        CoroutineStaticExt.StartIfNotExists();
        CoroutineStaticExt.StaticWorker!.ReplacementObject = new ValueTuple<GodotObject, string>(godotObject, signal);
        CoroutineStaticExt.StaticWorker!.ReplacementFunction = CoroutineEnumeratorExt.WaitUntilSignalHelper;
        return double.NaN;
    }
}
