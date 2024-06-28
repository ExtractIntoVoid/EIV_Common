using EIV_Common.ItemStuff;
using EIV_Common.Logger;
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
            Debugger.ParseLogger(MainLog.logger);
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
