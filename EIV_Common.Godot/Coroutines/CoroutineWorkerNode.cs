using EIV_Coroutines;
using EIV_Coroutines.CoroutineWorkers;
using Godot;
using GD = Godot;
using Serilog;

namespace EIV_Common.Coroutines;

public partial class CoroutineWorkerNode : Node, ICoroutineWorker<double>
{
    private readonly List<(double Delay, Coroutine<double> Cor)> _DelayAndCoroutines = [];
    public List<(double Delay, Coroutine<double> Cor)> DelayAndCoroutines
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
    public bool DontKillSuccess { get; set; } = false;

    #region NodeWrite
    public override void _Ready()
    {
        Init();
    }

    public override void _Process(double delta)
    {
        Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        Update(delta);
    }

    public override void _ExitTree()
    {
        Quit();
    }

    #endregion
    #region Mutex
    private readonly GD.Mutex _mutex = new();
    public bool MutexLock()
    {
        return _mutex.TryLock();
    }

    public void MutexUnLock()
    {
        _mutex.Unlock();
    }
    #endregion
    #region Basic stuff (Init, Quit, Update)
    public void Init()
    {

    }
    public void Quit()
    {
        Kill();
    }

    public void Update(double deltaTime)
    {
        Kill();
        if (MutexLock())
        {
            for (int i = 0; i < _DelayAndCoroutines.Count; i++)
            {
                ((double Delay, Coroutine<double> Cor) DelayAndCor, int index) cor_delay = new();
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
            ((double Delay, Coroutine<double> Cor) DelayAndCor, int index) cor_delay = new();
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
                ((double Delay, Coroutine<double> Cor) DelayAndCor, int index) cor_delay = new();
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
        var cor = GetCoroutine(coroutine);
        if (cor is null)
            return false;
        return cor.IsSuccess;
    }
    public bool IsCoroutineRunningInstance(CoroutineHandle coroutine)
    {
        var cor = GetCoroutine(coroutine);
        if (cor is null)
            return false;
        return cor.IsRunning;
    }
    #endregion
    #region Other Coroutine stuff
    public void PauseCoroutineInstance(CoroutineHandle coroutine)
    {
        if (GetCoroutineIndex(coroutine) == -1)
            return;
        if (MutexLock())
        {
            ((double Delay, Coroutine<double> Cor) DelayAndCor, int index) cor_delay = new();
            GetCorAndDelayRef(coroutine, ref cor_delay);
            if (cor_delay.index == -1)
            {
                Log.Debug("No Coroutine to Pause! (Handle was: {Handle})", coroutine);
                MutexUnLock();
                return;
            }
            cor_delay.DelayAndCor.Cor.IsPaused = !cor_delay.DelayAndCor.Cor.IsPaused;
            Log.Debug("Coroutine {cor} changed ShouldPause state", cor_delay.DelayAndCor.Cor.GetHashCode());
            SetCorAndDelayRef(ref cor_delay);
            MutexUnLock();
        }
    }
    public void AddCoroutineInstance(Coroutine<double> coroutine)
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

    public Coroutine<double>? GetCoroutine(CoroutineHandle coroutine)
    {
        if (MutexLock())
        {
            int index = _DelayAndCoroutines.FindIndex(x => coroutine.Equals((CoroutineHandle)x.Cor));
            if (index < 0)
            {
                MutexUnLock();
                return null;
            }
            MutexUnLock();
            return _DelayAndCoroutines[index].Cor;
        }
        return null;
    }
    #endregion
    #region Private Stuff

    private void CoroutineWork(ref ((double Delay, Coroutine<double> Cor) DelayAndCor, int index) ref_values)
    {
        if (ref_values.DelayAndCor.Cor.ShouldKill)
            return;
        if (ref_values.DelayAndCor.Cor.IsPaused)
            return;
        if (ref_values.DelayAndCor.Cor.IsSuccess)
            return;
        ref_values.DelayAndCor.Cor.IsRunning = true;
        if (!MoveNext(ref ref_values))
        {
            ref_values.DelayAndCor.Cor.IsRunning = false;
            ref_values.DelayAndCor.Cor.IsSuccess = true;
            if (!DontKillSuccess)
                ref_values.DelayAndCor.Cor.ShouldKill = true;
            Log.Debug("Coroutine {cor} changed states", ref_values.DelayAndCor.Cor);
        }
    }

    private static bool MoveNext(ref ((double Delay, Coroutine<double> Cor) DelayAndCor, int index) ref_values)
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

    private void GetCorAndDelayRef(int index, ref ((double Delay, Coroutine<double> Cor), int index) ref_values)
    {
        if (index == -1)
            return;
        if (MutexLock())
        {
            ref_values = (_DelayAndCoroutines[index], index);
            MutexUnLock();
        }
    }

    private void GetCorAndDelayRef(CoroutineHandle coroutine, ref ((double Delay, Coroutine<double> Cor), int index) ref_values)
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

    private void SetCorAndDelayRef(ref ((double Delay, Coroutine<double> Cor) DelayAndCor, int index) ref_values)
    {
        if (ref_values.index == -1)
            return;
        if (MutexLock())
        {
            _DelayAndCoroutines[ref_values.index] = ref_values.DelayAndCor;
            MutexUnLock();
        }
    }
    #endregion
    #region NewStuff
    #endregion
}
