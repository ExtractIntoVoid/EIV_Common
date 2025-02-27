namespace EIV_Common.Coroutines;

public interface ICoroutineWorker
{
    /// <summary>
    /// Replacement object for able to work in update
    /// </summary>
    object? ReplacementObject { get; set; }
    /// <summary>
    /// Function to replace if Delay is set to NaN / infinity
    /// </summary>
    Func<IEnumerator<double>, IEnumerator<double>>? ReplacementFunction { get; set; }
    /// <summary>
    /// A list for delays and Coroutine
    /// </summary>
    List<(double Delay, Coroutine Cor)> DelayAndCoroutines { get; }
    /// <summary>
    /// Should the updating pause
    /// </summary>
    bool PauseUpdate { get; set; }
    void Init();
    void Quit();
    void Update(double deltaTime);
    bool MutexLock();
    void MutexUnLock();

    public void AddCoroutineInstance(Coroutine coroutine);
    public void KillCoroutinesInstance(IList<CoroutineHandle> coroutines);
    public void KillCoroutineInstance(CoroutineHandle coroutine);
    public void KillCoroutineTagInstance(string tag);
    public void PauseCoroutineInstance(CoroutineHandle coroutine);
    public bool IsCoroutineExistsInstance(CoroutineHandle coroutine);
    public bool IsCoroutineSuccessInstance(CoroutineHandle coroutine);
    public bool IsCoroutineRunningInstance(CoroutineHandle coroutine);
    public bool HasAnyCoroutinesInstance();

    // Internal use only !!! (Or if you use outside just use MutexLock/MutexUnlock)
    public int GetCoroutineIndex(CoroutineHandle coroutine);
    public Coroutine GetCoroutine(CoroutineHandle coroutine);

}