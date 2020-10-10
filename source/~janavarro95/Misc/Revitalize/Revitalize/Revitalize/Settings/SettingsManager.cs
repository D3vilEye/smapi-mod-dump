/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Settings
{

    class SettingsManager
    {

        public static MachineSettings machineSettings;
        public static PaintSettings paintSettings;

        public static List<SettingsInterface> SettingsList;

        public static void Initialize()
        {
            Serialize.SettingsPath = Path.Combine(Class1.path, "Settings");
            if (!Directory.Exists(Serialize.SettingsPath))
            {
                Directory.CreateDirectory(Serialize.SettingsPath);
            }

            SettingsList = new List<SettingsInterface>();
            SettingsList.Add(machineSettings = new MachineSettings());
            SettingsList.Add(paintSettings = new PaintSettings());
        }

        public static void SaveAllSettings()
        {
            foreach(var v in SettingsList)
            {
                v.SaveSettings();
            }
        }

        public static void LoadAllSettings()
        {
            foreach(var v in SettingsList)
            {
                try
                {
                    v.LoadSettings();
                }
                catch(Exception e)
                {
                    //Log.AsyncR(e);
                }
            }
        }
    }
}
