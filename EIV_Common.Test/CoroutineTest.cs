using EIV_Common.Coroutines;
using System.Diagnostics;

namespace EIV_Common.Test;

public class CoroutineTest
{
    [OneTimeSetUp]
    public void SetUp()
    {
        // This exist here to make our test faster, running at 144 fps
        CoroutineWorkerCustom.UpdateRate = 1 / 144f;
        CoroutineWorkerCustom.Instance.Start();
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        CoroutineWorkerCustom.Instance.Quit();
    }

    [Test]
    public void TestWaitUntils()
    {
        var handle = CoroutineWorkerCustom.StartCoroutine(_CountingDown(), CoroutineType.Custom, "Test");
        Assert.IsNotNull(handle);
        Assert.That(handle.CoroutineHash, Is.Not.EqualTo(0));
        Assert.That(CoroutineWorkerCustom.IsCoroutineExists(handle), Is.EqualTo(true));
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!CoroutineWorkerCustom.IsCoroutineSuccessInstance(handle))
        {
            // wait until test over.
        }
        stopwatch.Stop();
        Thread.Sleep(100);
        Assert.That(CoroutineWorkerCustom.IsCoroutineExists(handle), Is.EqualTo(false));
        Assert.That(CoroutineWorkerCustom.IsCoroutineSuccessInstance(handle), Is.EqualTo(false));
    }

    [Test]
    public void TestKillTag()
    {
        var handle = CoroutineWorkerCustom.StartCoroutine(_FakeCountingDown(), CoroutineType.Custom, "Test");
        Assert.That(CoroutineWorkerCustom.IsCoroutineExists(handle), Is.EqualTo(true));
        Assert.That(CoroutineWorkerCustom.IsCoroutineSuccessInstance(handle), Is.EqualTo(false));
        CoroutineWorkerCustom.KillCoroutineTagInstance("Test");
        Thread.Sleep(100);
        Assert.That(CoroutineWorkerCustom.IsCoroutineExists(handle), Is.EqualTo(false));
    }

    [Test]
    public void TestKill()
    {
        var handle = CoroutineWorkerCustom.StartCoroutine(_FakeCountingDown(), CoroutineType.Custom, "Test");
        Assert.That(CoroutineWorkerCustom.IsCoroutineExists(handle), Is.EqualTo(true));
        Assert.That(CoroutineWorkerCustom.IsCoroutineSuccessInstance(handle), Is.EqualTo(false));
        CoroutineWorkerCustom.KillCoroutineInstance(handle);
        Thread.Sleep(100);
        Assert.That(CoroutineWorkerCustom.IsCoroutineExists(handle), Is.EqualTo(false));
    }

    [Test]
    public void TestNoCor()
    {
        Assert.That(CoroutineWorkerCustom.HasAnyCoroutines(), Is.EqualTo(false));
        var handle = CoroutineWorkerCustom.StartCoroutine(_FakeCountingDown(), CoroutineType.Custom, "Test");
        CoroutineWorkerCustom.KillCoroutineInstance(handle);
    }


    public IEnumerator<double> _CountingDown()
    {
        byte i = byte.MaxValue;
        yield return CoroutineWorkerCustom.WaitUntilZero<byte>(
            () =>
            {
                i--; 
                return i; 
            });
        yield return 0;
        yield break;
    }
    public IEnumerator<double> _FakeCountingDown()
    {
        yield return CoroutineWorkerCustom.WaitUntilZero<byte>(
            () =>
            {
                return 1;
            });
        yield return 0;
        yield break;
    }
}
