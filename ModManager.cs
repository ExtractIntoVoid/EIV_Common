using EIV_Common.JsonStuff;
using EIV_Common.Logger;
using EIV_DataPack;
using EIV_JsonLib;
using EIV_JsonLib.Convert;
using EIV_JsonLib.Modding;
using ModAPI;
using System.Reflection;

namespace EIV_Common
{
    public class ModManager
    {
        public static Dictionary<string, List<string>> ModsFiles = new();

        public delegate void LoadModWithTypeDelegate(Type? retType, object? obj);

        public static void Init()
        {
            if (MainLog.logger == null) 
                MainLog.CreateNew();
            Debugger.ParseLogger( MainLog.logger! );
            MainLoader.Init( new LoadSettings()
            {
                LoadAsMainCaller = false,
                ModAPI = ModAPIEnum.All,
                LoadWithoutAssemblyDefinedModAPI = true
            });
            MainLoader.SetMainModAssembly( Assembly.GetCallingAssembly() );
            MainLoader.LoadDependencies();
        }

        public static void LoadAssets_Unpack(string Dir)
        {
            if (!Directory.Exists( Path.Combine(Dir, "Assets", "Items") ))
                return;

            foreach (var json in Directory.GetFiles( Path.Combine(Dir, "Assets", "Items") , "*.json", SearchOption.AllDirectories))
            {
                var item = ConvertHelper.ConvertFromString( File.ReadAllText(json) );
                if (item != null)
                {
                    bool ret = Storage.Items.TryAdd( item.BaseID, item);
                    if (!ret)
                        continue;
                }
            }
        }

        public static void LoadAssets_Pack(DataPackReader reader)
        {
            var items = reader.Pack.FileNames.Where(x => x.Contains(".json") && x.Contains("Assets/Items") );
            foreach (var item in items)
            {
                var real_item = ConvertHelper.ConvertFromString( System.Text.Encoding.UTF8.GetString( reader.GetFileData(item) ) );
                if (real_item != null)
                {
                    bool ret = Storage.Items.TryAdd( real_item.BaseID, real_item);
                    if (!ret)
                        continue;
                }
            }
        }

        public static void LoadMod(Type intefaceType, Assembly assembly, LoadModWithTypeDelegate @delegate)
        {
            var type = assembly.GetTypes().Where(intefaceType.IsAssignableFrom).ToList().FirstOrDefault();
            if (type == null)
                return;

            var modType = Activator.CreateInstance(type!);
            @delegate.Invoke(type, modType);
        }

        public static void LoadMod_JsonLib(Assembly assembly)
        {
            LoadModWithTypeDelegate @delegate = (Type? retType, object? obj) =>
            {
                var jsonLib = (IJsonLibConverter?)obj;
                if (jsonLib == null)
                    return;
                JsonLibConverters.ModdedConverters.Add(jsonLib);
            };
            LoadMod(typeof(IJsonLibConverter), assembly, @delegate);
        }

        public static void DeInit()
        {
            ModsFiles.Clear();
            Storage.ClearAll();
            MainLoader.DeInit();
            Debugger.ClearLogger();
        }
    }
}
