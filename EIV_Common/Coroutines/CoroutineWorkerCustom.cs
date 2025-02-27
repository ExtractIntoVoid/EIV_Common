using Serilog;
using System.Diagnostics;

namespace EIV_Common.Coroutines;
public class CoroutineWorkerCustom : ICoroutineWorker
{
    private List<(double Delay, Coroutine Cor)> _DelayAndCoroutines = [];
    public List<(double Delay, Coroutine Cor)> DelayAndCoroutines
    { 
        get 
        {
            if (MutexLock())
            {
                var to_ret = _DelayAndCoroutines;
                MutexUnLock();
                return to_ret;
            }
            return [];
        } 
    }
    public object? ReplacementObject { get; set; }
    public Func<IEnumerator<double>, IEnumerator<double>>? ReplacementFunction { get; set; }
    public bool PauseUpdate { get; set; } = false;
    public static double UpdateRate { get; set; } = 1f / 60f;  // "fps"

    #region Private fields
    Thread? UpdateThread;
    Stopwatch Watch = new Stopwatch();
    double prevTime = 0f;
    double accumulator = 0f;
    double TotalTime = 0f;
    #endregion

    #region Mutex
    private readonly Mutex _mutex = new();
    public bool MutexLock()
    {
        return _mutex.WaitOne(1);
    }

    public void MutexUnLock()
    {
        _mutex.ReleaseMutex();
    }
    #endregion
    #region Basic stuff (Init, Quit, Update)
    public void Init()
    {
        Watch.Start();
        prevTime = Watch.ElapsedMilliseconds / 1000f;
        UpdateThread = new(ThreadUpdate)
        {
            IsBackground = true
        };
        UpdateThread.Start();
    }
    public void Quit()
    {
        Kill();
        UpdateThread?.Interrupt();
        UpdateThread = null;
        Watch.Stop();
        Log.Debug("Was run until " + TotalTime);
    }

    public void Update(double deltaTime)
    {
        Kill();
        if (MutexLock())
        {
            for (int i = 0; i < _DelayAndCoroutines.Count; i++)
            {
                ((double Delay, Coroutine Cor) DelayAndCor, int index) cor_delay = new();
                GetCorAndDelayRef(i, ref cor_delay);
                Log.Debug("Index: {i}, DT: {DeltaTime} Delay: {Delay}, Cor: {Coroutine}", i, deltaTime, cor_delay.DelayAndCor.Delay, cor_delay.DelayAndCor.Cor);
                if (cor_delay.DelayAndCor.Delay > 0f)
                    cor_delay.DelayAndCor.Delay -= deltaTime;
                if (cor_delay.DelayAndCor.Delay <= 0f)
                {
                    CoroutineWork(ref cor_delay);
                }
                if (double.IsNaN(cor_delay.DelayAndCor.Delay))
                {
                    if (ReplacementFunction != null)
                    {
                        cor_delay.DelayAndCor.Cor.Enumerator = ReplacementFunction(cor_delay.DelayAndCor.Cor.Enumerator);
                        CoroutineWork(ref cor_delay);
                        ReplacementFunction = null;
                    }
                }
                SetCorAndDelayRef(ref cor_delay);
            }
            MutexUnLock();
        }
        Kill();
    }
    #endregion
    #region Kills
    public void KillCoroutineInstance(CoroutineHandle coroutine)
    {
        if (MutexLock())
        {
            ((double Delay, Coroutine Cor) DelayAndCor, int index) cor_delay = new();
            GetCorAndDelayRef(coroutine, ref cor_delay);
            if (cor_delay.index == -1)
            {
                Log.Debug("No Coroutine to kill! (Handle was: {Handle})", coroutine);
                MutexUnLock();
                return;
            }
            cor_delay.DelayAndCor.Cor.ShouldKill = true;
            Log.Debug("Coroutine {cor} changed ShouldKill state", cor_delay.DelayAndCor.Cor.GetHashCode());
            SetCorAndDelayRef(ref cor_delay);
            MutexUnLock();
        }
    }

    public void KillCoroutinesInstance(IList<CoroutineHandle> coroutines)
    {
        if (MutexLock())
        {
            foreach (CoroutineHandle coroutine in coroutines)
            {
                ((double Delay, Coroutine Cor) DelayAndCor, int index) cor_delay = new();
                GetCorAndDelayRef(coroutine, ref cor_delay);
                if (cor_delay.index == -1)
                {
                    Log.Debug("No Coroutine to kill! (Handle was: {Handle})", coroutine);
                    continue;
                }
                cor_delay.DelayAndCor.Cor.ShouldKill = true;
                Log.Debug("Coroutine {cor} changed ShouldKill state", cor_delay.DelayAndCor.Cor.GetHashCode());
                SetCorAndDelayRef(ref cor_delay);
            }
            MutexUnLock();
        }
    }

    public void KillCoroutineTagInstance(string tag)
    {
        var cors = DelayAndCoroutines.Where(x => x.Cor.Tag == tag).Select(x => (CoroutineHandle)x.Cor).ToList();
        KillCoroutinesInstance(cors);
    }
    #endregion
    #region Checks
    public bool HasAnyCoroutinesInstance()
    {
        bool success = false;
        if (MutexLock())
        {
            success = DelayAndCoroutines.Count != 0;
            MutexUnLock();
        }
        return success;
    }

    public bool IsCoroutineExistsInstance(CoroutineHandle coroutine)
    {
        return GetCoroutineIndex(coroutine) != -1;
    }

    public bool IsCoroutineSuccessInstance(CoroutineHandle coroutine)
    {
        return GetCoroutine(coroutine).IsSuccess;
    }
    public bool IsCoroutineRunningInstance(CoroutineHandle coroutine)
    {
        return GetCoroutine(coroutine).IsRunning;
    }
    #endregion
    #region Other Coroutine stuff
    public void PauseCoroutineInstance(CoroutineHandle coroutine)
    {
        if (GetCoroutineIndex(coroutine) == -1)
            return;
        if (MutexLock())
        {
            ((double Delay, Coroutine Cor) DelayAndCor, int index) cor_delay = new();
            GetCorAndDelayRef(coroutine, ref cor_delay);
            if (cor_delay.index == -1)
            {
                Log.Debug("No Coroutine to Pause! (Handle was: {Handle})", coroutine);
                MutexUnLock();
                return;
            }
            cor_delay.DelayAndCor.Cor.ShouldPause = !cor_delay.DelayAndCor.Cor.ShouldPause;
            Log.Debug("Coroutine {cor} changed ShouldPause state", cor_delay.DelayAndCor.Cor.GetHashCode());
            SetCorAndDelayRef(ref cor_delay);
            MutexUnLock();
        }
    }
    public void AddCoroutineInstance(Coroutine coroutine)
    {
        if (MutexLock())
        {
            _DelayAndCoroutines.Add((0, coroutine));
            MutexUnLock();
        }
    }
    public int GetCoroutineIndex(CoroutineHandle coroutine)
    {
        if (MutexLock())
        {
            int index = _DelayAndCoroutines.FindIndex(x => coroutine.Equals((CoroutineHandle)x.Cor));
            if (index < 0)
            {
                MutexUnLock();
                return -1;
            }
            MutexUnLock();
            return index;
        }
        return -1;
    }

    public Coroutine GetCoroutine(CoroutineHandle coroutine)
    {
        if (MutexLock())
        {
            int index = _DelayAndCoroutines.FindIndex(x => coroutine.Equals((CoroutineHandle)x.Cor));
            if (index < 0)
            {
                MutexUnLock();
                return default;
            }
            MutexUnLock();
            return _DelayAndCoroutines[index].Cor;
        }
        return default;
    }
    #endregion
    #region Private stuff
    private void CoroutineWork(ref ((double Delay, Coroutine Cor) DelayAndCor, int index) ref_values)
    {
        if (ref_values.DelayAndCor.Cor.ShouldKill)
            return;
        if (ref_values.DelayAndCor.Cor.ShouldPause)
            return;
        if (ref_values.DelayAndCor.Cor.IsSuccess)
            return;
        ref_values.DelayAndCor.Cor.IsRunning = true;
        if (!MoveNext(ref ref_values))
        {
            ref_values.DelayAndCor.Cor.IsRunning = false;
            ref_values.DelayAndCor.Cor.IsSuccess = true;
            ref_values.DelayAndCor.Cor.ShouldKill = true;
            Log.Debug("Coroutine {cor} changed states", ref_values.DelayAndCor.Cor);
        }
    }

    private bool MoveNext(ref ((double Delay, Coroutine Cor) DelayAndCor, int index) ref_values)
    {
        bool result = ref_values.DelayAndCor.Cor.Enumerator.MoveNext();
        ref_values.DelayAndCor.Delay = ref_values.DelayAndCor.Cor.Enumerator.Current;
        return result;
    }
    private void Kill()
    {
        if (MutexLock())
        {
            for (int i = 0; i < _DelayAndCoroutines.Count; i++)
            {
                if (_DelayAndCoroutines[i].Cor.ShouldKill)
                {
                    _DelayAndCoroutines.RemoveAt(i);
                }
            }
            MutexUnLock();
        }
    }

    private void GetCorAndDelayRef(int index, ref ((double Delay, Coroutine Cor), int index) ref_values)
    {
        if (index == -1)
            return;
        if (MutexLock())
        {
            ref_values = (_DelayAndCoroutines[index], index);
            MutexUnLock();
        }
    }

    private void GetCorAndDelayRef(CoroutineHandle coroutine, ref ((double Delay, Coroutine Cor), int index) ref_values)
    {
        if (MutexLock())
        {
            int index = _DelayAndCoroutines.FindIndex(x => coroutine.Equals((CoroutineHandle)x.Cor));
            if (index < 0)
            {
                ref_values = (default, index);
                MutexUnLock();
                return;
            }
            ref_values = (_DelayAndCoroutines[index], index);
            MutexUnLock();
        }
    }

    private void SetCorAndDelayRef(ref ((double Delay, Coroutine Cor) DelayAndCor, int index) ref_values)
    {
        if (ref_values.index == -1)
            return;
        if (MutexLock())
        {
            _DelayAndCoroutines[ref_values.index] = ref_values.DelayAndCor;
            MutexUnLock();
        }
    }
    private void ThreadUpdate()
    {
        if (UpdateThread == null)
            return;
        Log.Debug("UpdateThread.ThreadState: {state}", UpdateThread.ThreadState);
        while (UpdateThread.ThreadState == System.Threading.ThreadState.Background)
        {
            if (PauseUpdate)
                continue;

            double currTime = Watch.ElapsedMilliseconds / 1000f;
            accumulator += currTime - prevTime;
            prevTime = currTime;

            if (accumulator > UpdateRate)
            {
                accumulator -= UpdateRate;
                Update(UpdateRate);
                TotalTime += UpdateRate;
            }
        }
    }
    #endregion
    #region NewStuff
    #endregion
}
