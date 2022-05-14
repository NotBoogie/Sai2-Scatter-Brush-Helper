using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sai2_Scatter_Brush_Helper
{
    class StaticSettingsS2SBH
    {
        public static string FilenameKey { get; set; }
        public static string SettingsFileLoc { get { return Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"/settings.json"; } }

        public static string Sai2ScatterBrushFolderLocation { get; set; } = "";
        public static string BaseINIFileLocation { get; set; } = "";

        public static bool Loaded { get; set; } = false;
        public static void LoadSettings(string filenameKey="")
        {
            FilenameKey = filenameKey;
            Console.WriteLine("Loading settings file from: " + SettingsFileLoc);
            StaticSettingsS2SBH_B settingsObj = new StaticSettingsS2SBH_B();
            if (File.Exists(SettingsFileLoc))
            {
                using (StreamReader r = new StreamReader(SettingsFileLoc))
                {
                    string json = r.ReadToEnd();
                    settingsObj = JsonConvert.DeserializeObject<StaticSettingsS2SBH_B>(json);
                }
                settingsObj.Loaded = true;
            }
            MapToStaticClass(settingsObj);
        }
        public static void SaveSettings()
        {
            StaticSettingsS2SBH_B settingsObj = MapFromStaticClass();
            File.WriteAllText(SettingsFileLoc, JsonConvert.SerializeObject(settingsObj));
        }
        public static void MapToStaticClass(StaticSettingsS2SBH_B source)
        {
            var sourceProperties = source.GetType().GetProperties();

            //Key thing here is to specify we want the static properties only
            var destinationProperties = typeof(StaticSettingsS2SBH)
                .GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (var prop in sourceProperties)
            {
                if (!prop.CanWrite) continue;
                //Find matching property by name
                var destinationProp = destinationProperties
                    .Single(p => p.Name == prop.Name);

                //Set the static property value
                destinationProp.SetValue(null, prop.GetValue(source));
            }
        }

        public static StaticSettingsS2SBH_B MapFromStaticClass()
        {
            StaticSettingsS2SBH_B bb = new StaticSettingsS2SBH_B();
            var destinationProperties = bb.GetType().GetProperties();

            //Key thing here is to specify we want the static properties only
            var sourceProperties = typeof(StaticSettingsS2SBH)
                .GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (var prop in destinationProperties)
            {
                if (!prop.CanWrite) continue;
                //Find matching property by name
                var sourceProp = sourceProperties
                    .Single(p => p.Name == prop.Name);

                //Set the static property value
                prop.SetValue(bb, sourceProp.GetValue(null));
            }
            return bb;
        }
    }


    /// <summary>
    /// Fake class which maps to the real static one, has the same properties but not static
    /// </summary>
    public class StaticSettingsS2SBH_B
    {
        public string FilenameKey { get; set; }
        public string SettingsFileLoc { get { return Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"/" + FilenameKey + "settings.json"; } }

        public string Sai2ScatterBrushFolderLocation { get; set; } = "";
        public string BaseINIFileLocation { get; set; } = "";
        public bool Loaded { get; set; } = false;
    }
}
