using EIV_Common.ItemStuff;
using EIV_Common.Logger;
using EIV_DataPack;
using EIV_JsonLib.Convert;
using ModAPI;
using System.Reflection;

namespace EIV_Common
{
    public class ModManager
    {
        public static Dictionary<string, List<string>> JsonMods = new();
        public static Dictionary<string, List<string>> ModsFiles = new();

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

        public static void LoadAssets_Unpack(string Dir)
        {
            if (!Directory.Exists(Path.Combine(Dir, "Assets", "Items")))
                return;

            foreach (var json in Directory.GetFiles(Path.Combine(Dir, "Assets", "Items"), "*.json", SearchOption.AllDirectories))
            {
                var item = ConvertHelper.ConvertFromString(File.ReadAllText(json));
                if (item != null)
                {
                    bool ret = ItemMaker.Items.TryAdd(item.BaseID, item);
                    if (!ret)
                        continue;
                    if (JsonMods.ContainsKey(Dir))
                    {
                        JsonMods[Dir].Add(item.BaseID);
                        continue;
                    }
                    JsonMods.Add(Dir, new() { item.BaseID });
                }
            }
        }

        public static void LoadAssets_Pack(DataPackReader reader, string filename)
        {
            var items = reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Assets/Items"));
            foreach (var item in items)
            {
                var real_item = ConvertHelper.ConvertFromString(System.Text.Encoding.UTF8.GetString(reader.GetFileData(item)));
                if (real_item != null)
                {
                    bool ret = ItemMaker.Items.TryAdd(real_item.BaseID, real_item);
                    if (!ret)
                        continue;
                    if (JsonMods.ContainsKey(filename))
                    {
                        JsonMods[filename].Add(real_item.BaseID);
                        continue;
                    }
                    JsonMods.Add(filename, new() { real_item.BaseID });
                }
            }
        }

        public static void DeInit()
        {
            JsonMods.Clear();
            ModsFiles.Clear();
            ItemMaker.Items.Clear();
            MainLoader.DeInit();
            Debugger.ClearLogger();
        }
    }
}
