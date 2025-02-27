using EIV_Common.Coroutines;
using EIV_Common.Coroutines.Extensions;
using EIV_Common.Logger;
using Serilog;
using System.Diagnostics;

namespace EIV_Common.Test;

public class CoroutineTest
{
    [OneTimeSetUp]
    public void SetUp()
    {
        MainLog.CreateNew();
        // This exist here to make our test faster, running at 144 fps
        CoroutineWorkerCustom.UpdateRate = 1 / 144f;
        CoroutineStaticExt.Start();

    }

    [OneTimeTearDown]
    public void Teardown()
    {
        CoroutineStaticExt.Stop();
        MainLog.Close();
    }

    [Test]
    public void TestWaitCountdown()
    {
        var handle = CoroutineStaticExt.StartCoroutine(_CountingDown(), CoroutineType.Custom, "Test");
        Assert.That(handle, Is.Not.Zero);
        Assert.That(handle.CoroutineHash, Is.Not.EqualTo(0));
        Thread.Sleep(100);
        Assert.That(CoroutineStaticExt.IsCoroutineExists(handle), Is.True);
        Thread.Sleep(10);
        Assert.That(CoroutineStaticExt.IsCoroutineRunning(handle), Is.True);
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!CoroutineStaticExt.IsCoroutineSuccess(handle))
        {
            if (stopwatch.Elapsed > TimeSpan.FromSeconds(10))
            {
                Log.Information("killing after 10 sec");
                Assert.Fail();
            }
        }
        stopwatch.Stop();

        Assert.That(CoroutineStaticExt.IsCoroutineExists(handle), Is.False);
        Assert.That(CoroutineStaticExt.IsCoroutineSuccess(handle), Is.False);

    }


    [Test]
    public void TestWaitFor()
    {
        var handle = CoroutineStaticExt.StartCoroutine(_WaitForTrue(), CoroutineType.Custom, "_WaitForTrue");
        Assert.That(handle, Is.Not.Zero);
        Assert.That(handle.CoroutineHash, Is.Not.EqualTo(0));
        Thread.Sleep(10);
        Assert.That(CoroutineStaticExt.IsCoroutineExists(handle), Is.EqualTo(true));
        var WaitAndSetTrue_handle = CoroutineStaticExt.StartCoroutine(_WaitAndSetTrue(), CoroutineType.Custom, "_WaitAndSetTrue");
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!CoroutineStaticExt.IsCoroutineSuccess(handle))
        {
            if (stopwatch.Elapsed > TimeSpan.FromSeconds(10))
            {
                Log.Information("killing after 10 sec");
                Assert.Fail();
            }
        }
        stopwatch.Stop();
        Thread.Sleep(100);
        Assert.That(_TestBoolValue, Is.True);
        Assert.That(CoroutineStaticExt.IsCoroutineExists(handle), Is.EqualTo(false));
        Assert.That(CoroutineStaticExt.IsCoroutineSuccess(handle), Is.EqualTo(false));
        //CoroutineStaticExt.KillCoroutines([handle, WaitAndSetTrue_handle]);
        _TestBoolValue = false;
    }

    [Test]
    public void TestKillTag()
    {
        var handle = CoroutineStaticExt.StartCoroutine(_FakeCountingDown(), CoroutineType.Custom, "Test");
        Thread.Sleep(10);
        Assert.That(CoroutineStaticExt.IsCoroutineExists(handle), Is.EqualTo(true));
        Thread.Sleep(10);
        Assert.That(CoroutineStaticExt.IsCoroutineSuccess(handle), Is.EqualTo(false));
        CoroutineStaticExt.KillCoroutineTag("Test");
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (CoroutineStaticExt.IsCoroutineExists(handle))
        {
            if (stopwatch.Elapsed > TimeSpan.FromSeconds(5))
                Assert.Fail();
            // wait until test over.
        }
        Assert.That(CoroutineStaticExt.IsCoroutineExists(handle), Is.EqualTo(false));
    }

    [Test]
    public void TestKill()
    {
        var handle = CoroutineStaticExt.StartCoroutine(_FakeCountingDown(), CoroutineType.Custom, "Test_KILL");
        Thread.Sleep(10);
        Assert.That(CoroutineStaticExt.IsCoroutineExists(handle), Is.EqualTo(true));
        Thread.Sleep(10);
        Assert.That(CoroutineStaticExt.IsCoroutineSuccess(handle), Is.EqualTo(false));
        CoroutineStaticExt.KillCoroutine(handle);
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (CoroutineStaticExt.IsCoroutineExists(handle))
        {
            if (stopwatch.Elapsed > TimeSpan.FromSeconds(5))
                Assert.Fail();
            // wait until test over.
        }
        Thread.Sleep(10);
        Assert.That(CoroutineStaticExt.IsCoroutineExists(handle), Is.EqualTo(false));
    }

    [Test]
    public void TestNoCor()
    {
        Assert.That(CoroutineStaticExt.HasAnyCoroutines(), Is.EqualTo(false));
        var handle = CoroutineStaticExt.StartCoroutine(_FakeCountingDown(), CoroutineType.Custom, "Test");
        CoroutineStaticExt.KillCoroutine(handle);
    }


    public IEnumerator<double> _CountingDown()
    {
        yield return 0;
        byte i = byte.MaxValue;
        //Log.Information("_CountingDown set i to byte max");
        yield return CoroutineDoubleExt.WaitUntilZero<byte>(
            () =>
            {
                i--;
                //Log.Information("i: "+i);
                return i; 
            });
        yield return 0;
        //Log.Information("_CountingDown bye bye");
        yield break;
    }
    public IEnumerator<double> _FakeCountingDown()
    {
        yield return CoroutineDoubleExt.WaitUntilZero<byte>(
            () =>
            {
                return 1;
            });
        yield return 0;
        yield break;
    }

    private bool _TestBoolValue = false;

    public IEnumerator<double> _WaitForTrue()
    {
        //Log.Information("_WaitForTrue! ");
        yield return CoroutineDoubleExt.WaitUntilTrue(() => _TestBoolValue);
        //Log.Information("true! " + _TestBoolValue);
        yield return 0;
        yield break;
    }

    public IEnumerator<double> _WaitAndSetTrue()
    {
        yield return 2;
        _TestBoolValue = true;
        yield break;
    }
}
