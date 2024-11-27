using EIV_Common.JsonStuff;
using EIV_Common.Logger;
using EIV_DataPack;
using EIV_JsonLib;
using EIV_JsonLib.Base;
using EIV_JsonLib.Json;
using ModAPI;
using System.Reflection;
using System.Text.Json;

namespace EIV_Common;

public class ModManager
{
    public delegate void LoadModWithTypeDelegate(Type? retType, object? obj);

    public static void Init()
    {
        if (MainLog.logger == null)
            MainLog.CreateNew();
        Debugger.ParseLogger(MainLog.logger!);
        MainLoader.Init(new LoadSettings()
        {
            LoadAsMainCaller = false,
            ModAPI = ModAPIEnum.All,
            LoadWithoutAssemblyDefinedModAPI = true
        });
        MainLoader.SetMainModAssembly(Assembly.GetCallingAssembly());
        MainLoader.LoadDependencies();
    }

    public static void LoadAssets_Unpack(string dir)
    {
        if (!Directory.Exists(Path.Combine(dir, "Assets", "Items")))
            return;

        foreach (string json in Directory.GetFiles(Path.Combine(dir, "Assets", "Items"), "*.json", SearchOption.AllDirectories))
        {
            ItemBase? item = File.ReadAllText(json).ConvertFromString();
            if (item != null)
            {
                Storage.Items.TryAdd(item.BaseID, item);
            }
        }
        foreach (string json in Directory.GetFiles(Path.Combine(dir, "Assets", "Effects"), "*.json", SearchOption.AllDirectories))
        {
            Effect? effect = JsonSerializer.Deserialize<Effect>(File.ReadAllText(json));
            if (effect != null)
            {
                Storage.Effects.TryAdd(effect.EffectID, effect);
            }
        }
    }

    public static void LoadAssets_Pack(DataPackReader reader)
    {
        foreach (string? item in reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Assets/Items")))
        {
            EIV_JsonLib.Base.ItemBase? realItem = System.Text.Encoding.UTF8.GetString(reader.GetFileData(item)).ConvertFromString();
            if (realItem != null)
            {
                Storage.Items.TryAdd(realItem.BaseID, realItem);
            }
        }
    }

    public static void LoadMod(Type intefaceType, Assembly assembly, LoadModWithTypeDelegate @delegate)
    {
        Type? type = assembly.GetTypes().Where(intefaceType.IsAssignableFrom).ToList().FirstOrDefault();
        if (type == null)
            return;

        @delegate.Invoke(type, Activator.CreateInstance(type!));
    }

    public static void LoadMod_JsonLib(Assembly assembly)
    {
        LoadMod(typeof(IJsonLibConverter), assembly, Delegate);
        return;

        void Delegate(Type? retType, object? obj)
        {
            IJsonLibConverter? jsonLib = (IJsonLibConverter?)obj;
            if (jsonLib == null) return;
            CoreConverters.Converters.Add(jsonLib);
        }
    }

    public static void DeInit()
    {
        Storage.ClearAll();
        MainLoader.DeInit();
        Debugger.ClearLogger();
    }
}