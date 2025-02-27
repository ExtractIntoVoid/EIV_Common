using EIV_Common.Logger;

namespace EIV_Common.Coroutines.Extensions;

public static class CoroutineStaticExt
{
    public static ICoroutineWorker? StaticWorker { get; private set; }

    public static void StartIfNotExists()
    {
        if (StaticWorker == null)
            Start();
    }

    public static void Start(CoroutineWorkerEnum coroutineWorker = CoroutineWorkerEnum.Custom, Func<ICoroutineWorker>? func = null)
    {
        switch (coroutineWorker)
        {
            case CoroutineWorkerEnum.Custom:
                StaticWorker = new CoroutineWorkerCustom();
                break;
            case CoroutineWorkerEnum.Node:
                StaticWorker = new CoroutineWorkerNode();
                break;
            default:
                StaticWorker = func?.Invoke();
                break;
        }
        StaticWorker?.Init();
    }

    public static void Stop()
    {
        StaticWorker?.Quit();
        StaticWorker = null;
    }

    public static CoroutineHandle StartCoroutine(IEnumerator<double> objects, CoroutineType type = CoroutineType.Custom, string tag = "")
    {
        StartIfNotExists();
        Coroutine coroutine = new(objects, type, tag);
        MainLog.logger?.Debug(coroutine.ToString());
        StaticWorker!.AddCoroutineInstance(coroutine);
        return coroutine;
    }

    public static CoroutineHandle CallDelayed(TimeSpan timeSpan, Action action, CoroutineType type = CoroutineType.Custom, string tag = "")
    {
        return StartCoroutine(CoroutineEnumeratorExt.DelayedCall(timeSpan, action), type, tag);
    }
    public static CoroutineHandle CallContinuously(Action action, CoroutineType type = CoroutineType.Custom, string tag = "")
    {
        return StartCoroutine(CoroutineEnumeratorExt.CallContinuously(TimeSpan.Zero, action), type, tag);
    }
    public static CoroutineHandle CallPeriodically(TimeSpan timeSpan, Action action, CoroutineType type = CoroutineType.Custom, string tag = "")
    {
        return StartCoroutine(CoroutineEnumeratorExt.CallContinuously(timeSpan, action), type, tag);
    }

    public static void KillCoroutines(IList<CoroutineHandle> coroutines)
    {
        StartIfNotExists();
        StaticWorker!.KillCoroutinesInstance(coroutines);
    }

    public static void KillCoroutine(CoroutineHandle coroutine)
    {
        StartIfNotExists();
        StaticWorker!.KillCoroutineInstance(coroutine);
    }
    public static void KillCoroutineTag(string tag)
    {
        StartIfNotExists();
        StaticWorker!.KillCoroutineTagInstance(tag);
    }

    public static void PauseCoroutine(CoroutineHandle coroutine)
    {
        StartIfNotExists();
        StaticWorker!.PauseCoroutineInstance(coroutine);
    }

    public static bool IsCoroutineExists(CoroutineHandle coroutine)
    {
        StartIfNotExists();
        return StaticWorker!.IsCoroutineExistsInstance(coroutine);
    }

    public static bool IsCoroutineSuccess(CoroutineHandle coroutine)
    {
        StartIfNotExists();
        return StaticWorker!.IsCoroutineSuccessInstance(coroutine);
    }

    public static bool IsCoroutineRunning(CoroutineHandle coroutine)
    {
        StartIfNotExists();
        return StaticWorker!.IsCoroutineRunningInstance(coroutine);
    }

    public static bool HasAnyCoroutines()
    {
        StartIfNotExists();
        return StaticWorker!.HasAnyCoroutinesInstance();
    }
}

public enum CoroutineWorkerEnum
{
    Custom,
    Node,
}