using EIV_Common.JsonStuff;
using EIV_Common.Logger;
using EIV_DataPack;
using EIV_JsonLib;
using EIV_JsonLib.Base;
using EIV_JsonLib.Json;
using ModAPI;
using System.Reflection;
using System.Text.Json;
using System.Text;

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
        foreach (string json in Directory.GetFiles(Path.Combine(dir,  "Item"), "*.json", SearchOption.AllDirectories))
        {
            CoreItem? item = File.ReadAllText(json).ConvertFromString();
            if (item != null)
                Storage.Items.TryAdd(item.Id, item);
        }
        foreach (string json in Directory.GetFiles(Path.Combine(dir, "Effect"), "*.json", SearchOption.AllDirectories))
        {
            Effect? effect = JsonSerializer.Deserialize<Effect>(File.ReadAllText(json));
            if (effect != null)
                Storage.Effects.TryAdd(effect.EffectName, effect);
        }
        foreach (string? json in Directory.GetFiles(Path.Combine(dir, "Stash"), "*.json", SearchOption.AllDirectories))
        {
            Stash? stash = JsonSerializer.Deserialize<Stash>(File.ReadAllText(json), ConvertHelper.GetSerializerSettings());
            if (stash != null)
                Storage.Stashes.TryAdd(json, stash);
        }
        foreach (string? json in Directory.GetFiles(Path.Combine(dir, "Inventory"), "*.json", SearchOption.AllDirectories))
        {
            List<ItemRecreator>? recreators = JsonSerializer.Deserialize<List<ItemRecreator>>(File.ReadAllText(json), ConvertHelper.GetSerializerSettings());
            if (recreators != null)
                Storage.PredefinedCharactersInventory.TryAdd(json, recreators);
        }
    }

    public static void LoadAssets_Pack(DataPackReader reader)
    {
        foreach (string? item in reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Item")))
        {
            CoreItem? realItem = Encoding.UTF8.GetString(reader.GetFileData(item)).ConvertFromString();
            if (realItem != null)
                Storage.Items.TryAdd(realItem.Id, realItem);
        }
        foreach (string? item in reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Effect")))
        {
            Effect? effect = JsonSerializer.Deserialize<Effect>(Encoding.UTF8.GetString(reader.GetFileData(item)));
            if (effect != null)
                Storage.Effects.TryAdd(effect.EffectName, effect);
        }
        foreach (string? item in reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Stash")))
        {
            Stash? stash = JsonSerializer.Deserialize<Stash>(Encoding.UTF8.GetString(reader.GetFileData(item)), ConvertHelper.GetSerializerSettings());
            if (stash != null)
                Storage.Stashes.TryAdd(item, stash);
        }
        foreach (string? item in reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Inventory")))
        {
            List<ItemRecreator>? recreators = JsonSerializer.Deserialize<List<ItemRecreator>>(Encoding.UTF8.GetString(reader.GetFileData(item)), ConvertHelper.GetSerializerSettings());
            if (recreators != null)
                Storage.PredefinedCharactersInventory.TryAdd(item, recreators);
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

        static void Delegate(Type? retType, object? obj)
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