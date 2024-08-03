using IniParser;
using IniParser.Model;

namespace EIV_Common;

public class ConfigINI
{
    public static string Read(string filename, string section, string key)
    {
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(filename, System.Text.Encoding.UTF8);
        if (!data.Sections.ContainsSection(section))
            return string.Empty;
        if (!data[section].ContainsKey(key))
            return string.Empty;
        return data[section][key];
    }

    public static T? Read<T>(string filename, string section, string key) where T : IConvertible, IParsable<T>
    {
        string readed = Read(filename, section, key);
        if (string.IsNullOrEmpty(readed))
            return default;
        T? value = default;
        T.TryParse(readed, null, out value);
        return value;
    }


    public static void Write(string filename, string category, string section, string? Value)
    {
        if (Value == null)
            return;
        if (!File.Exists(filename))
        {
            var path = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(path); 
            }
            File.WriteAllText(filename, ";TMP");
        }
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(filename, System.Text.Encoding.UTF8);
        data[category][section] = Value;
        parser.WriteFile(filename, data, System.Text.Encoding.UTF8);
    }

    public static void Write<T>(string filename, string category, string section, T Value) where T : IConvertible
    {
        Write(filename, category, section, Value.ToString());
    }
}
