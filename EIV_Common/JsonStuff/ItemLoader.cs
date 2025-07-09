using EIV_Common.JsonStuff;
using EIV_DataPack;
using EIV_JsonLib;
using EIV_JsonLib.Base;
using EIV_JsonLib.Json;
using EIV_JsonLib.Profile;
using EIV_JsonLib.Profile.ProfileModules;
using System.Text;
using System.Text.Json;

namespace EIV_Common.JsonStuff;

public static class ItemLoader
{
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
}