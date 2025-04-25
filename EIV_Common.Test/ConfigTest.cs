namespace EIV_Common.Test;

public class ConfigTest
{

    [Test]
    public void TestString()
    {
        ConfigINI.Write("test.ini", "test", "str", "test");
        Assert.Multiple(() =>
        {
            Assert.That(File.Exists("test.ini"));
            Assert.That(ConfigINI.Read("test.ini", "test", "str"), Is.EqualTo("test"));
        });
    }

    [Test]
    public void TestParsables()
    {
        ConfigINI.Write("test.ini", "test", "bool", true);
        Assert.That(ConfigINI.Read<bool>("test.ini", "test", "bool"), Is.EqualTo(true));
        ConfigINI.Write("test.ini", "test", "int", 1);
        Assert.That(ConfigINI.Read<int>("test.ini", "test", "int"), Is.EqualTo(1));
        ConfigINI.Write("test.ini", "test", "byte",(byte)1);
        Assert.That(ConfigINI.Read<byte>("test.ini", "test", "byte"), Is.EqualTo(1));
        ConfigINI.Write("test.ini", "test", "ushort", (ushort)1);
        Assert.That(ConfigINI.Read<ushort>("test.ini", "test", "ushort"), Is.EqualTo(1));
        ConfigINI.Write("test.ini", "test", "short", (short)1);
        Assert.That(ConfigINI.Read<short>("test.ini", "test", "short"), Is.EqualTo(1));
        ConfigINI.Write("test.ini", "test", "uint", (uint)1);
        Assert.That(ConfigINI.Read<uint>("test.ini", "test", "uint"), Is.EqualTo(1));
        ConfigINI.Write("test.ini", "test", "long", (long)1);
        Assert.That(ConfigINI.Read<long>("test.ini", "test", "long"), Is.EqualTo(1));
        ConfigINI.Write("test.ini", "test", "ulong", (ulong)1);
        Assert.That(ConfigINI.Read<ulong>("test.ini", "test", "ulong"), Is.EqualTo(1));
    }
}