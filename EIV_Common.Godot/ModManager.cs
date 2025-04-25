using EIV_Common.JsonStuff;
using EIV_Common.Logger;
using EIV_DataPack;
using EIV_JsonLib;
using EIV_JsonLib.Base;
using EIV_JsonLib.Json;
using EIV_JsonLib.Profile;
using EIV_JsonLib.Profile.ProfileModules;
using ModAPI;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace EIV_Common.Godot;

public class ModManager
{
    public delegate void LoadModWithTypeDelegate(Type retType, object? obj);

    public static void Init()
    {
        if (MainLog.Logger == null)
            MainLog.CreateNew();
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
        foreach (string json in Directory.GetFiles(Path.Combine(dir, "Item"), "*.json", SearchOption.AllDirectories))
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
                Storage.Stashes.TryAdd(NormalizeMod(json, Path.Combine(dir, "Stash")), stash);
        }
        foreach (string? json in Directory.GetFiles(Path.Combine(dir, "Inventory"), "*.json", SearchOption.AllDirectories))
        {
            Inventory? inventory = JsonSerializer.Deserialize<Inventory>(File.ReadAllText(json), ConvertHelper.GetSerializerSettings());
            if (inventory != null)
                Storage.Inventories.TryAdd(NormalizeMod(json, Path.Combine(dir, "Inventory")), inventory);
        }
        foreach (string? json in Directory.GetFiles(Path.Combine(dir, "Origins"), "*.json", SearchOption.AllDirectories))
        {
            List<IProfileModule>? profileModules = JsonSerializer.Deserialize<List<IProfileModule>>(File.ReadAllText(json), ConvertHelper.GetSerializerSettings());
            if (profileModules != null)
                //TODO: Make the Key the name of the json file
                Storage.OriginToModules.TryAdd(NormalizeMod(json, Path.Combine(dir, "Origins")), profileModules);
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
                Storage.Stashes.TryAdd(NormalizeMod(item), stash);
        }
        foreach (string? item in reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Inventory")))
        {
            Inventory? inventory = JsonSerializer.Deserialize<Inventory>(Encoding.UTF8.GetString(reader.GetFileData(item)), ConvertHelper.GetSerializerSettings());
            if (inventory != null)
                Storage.Inventories.TryAdd(NormalizeMod(item), inventory);
        }
        foreach (string? item in reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Origins")))
        {
            List<IProfileModule>? profileModules = JsonSerializer.Deserialize<List<IProfileModule>>(Encoding.UTF8.GetString(reader.GetFileData(item)), ConvertHelper.GetSerializerSettings());
            if (profileModules != null)
                //TODO: Make the Key the name of the json file
                Storage.OriginToModules.TryAdd(NormalizeMod(item), profileModules);
        }
    }

    /// <summary>
    /// Load One Type that is assignable from <paramref name="intefaceType"/>
    /// </summary>
    /// <param name="intefaceType"></param>
    /// <param name="assembly"></param>
    /// <param name="delegate"></param>
    public static void LoadMod(Type intefaceType, Assembly assembly, LoadModWithTypeDelegate @delegate)
    {
        Type? type = assembly.GetTypes().Where(intefaceType.IsAssignableFrom).ToList().FirstOrDefault();
        if (type == null)
            return;

        @delegate.Invoke(type, Activator.CreateInstance(type!));
    }

    /// <summary>
    /// Load ALL type that is is assignable from <paramref name="intefaceType"/>
    /// </summary>
    /// <param name="intefaceType"></param>
    /// <param name="assembly"></param>
    /// <param name="delegate"></param>
    public static void LoadMods(Type intefaceType, Assembly assembly, LoadModWithTypeDelegate @delegate)
    {
        List<Type> types = [.. assembly.GetTypes().Where(intefaceType.IsAssignableFrom)];
        if (types.Count == 0)
            return;

        types.ForEach(t => @delegate.Invoke(t, Activator.CreateInstance(t)));
    }

    public static void LoadModHarmony(Assembly assembly, string? name = null)
    {
        if (name == null)
        {
            var asm_name = assembly.GetName();
            name = $"{asm_name.Name} v{asm_name.Version}";
        }
        if (name == " v0.0.0.0") // Is this the right check?
        {
            name = assembly.FullName;
        }
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        HarmonyLib.Harmony harmony = new(name);
        try
        {
            
            harmony.PatchAll();
        }
        catch (Exception ex)
        {
            harmony.UnpatchAll(name);
            Log.Error("Exception has been catched! Unpatching everything!\n {Exception}", ex);
        }

    }

    public static void LoadMod_JsonLib(Assembly assembly)
    {
        LoadMod(typeof(IJsonLibConverter), assembly, Delegate);

        static void Delegate(Type retType, object? obj)
        {
            IJsonLibConverter? jsonLib = (IJsonLibConverter?)obj;
            if (jsonLib == null)
                return;
            CoreConverters.Converters.Add(jsonLib);
        }
    }

    public static void DeInit()
    {
        Storage.ClearAll();
        MainLoader.DeInit();
    }

    public static string NormalizeMod(string path, string parentPath = "")
    {
        path = path.Replace('\\', '_').Replace('/', '_');
        if (path.Contains(".json"))
            path = path.Split(".json")[0];
        if (parentPath != "")
            path = path.Replace(parentPath, "");
        return path;
    }
}