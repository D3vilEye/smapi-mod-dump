/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;

namespace SlimeHutchLimit
{
    public class ModEntry : Mod
    {
        private static Config _config;
        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<Config>();

            Helper.ConsoleCommands.Add("SetSlimeHutchLimit", "Changes the max number of slimes that can inhabit a slime hutch.\n\nUsage: SetSlimeHutchLimit <value>\n- value: the number of slimes", ChangeMaxSlimes);

            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            harmony.Patch(AccessTools.Method(typeof(SlimeHutch), nameof(SlimeHutch.isFull)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.SlimeHutch_isFull_postfix)));

        }

        private static void SlimeHutch_isFull_postfix(GameLocation __instance, ref bool __result)
        {
            __result = __instance.characters.Count >= (_config?.MaxSlimesInHutch ?? 20);
        }

        private void ChangeMaxSlimes(string arg1, string[] arg2)
        {

            if (int.TryParse(arg2[0], out int newLimit))
            {
                _config.MaxSlimesInHutch = newLimit;
                Helper.WriteConfig(_config);
                Monitor.Log($"The new Slime limit is: {_config.MaxSlimesInHutch}");
            }
            else
            {
                Monitor.Log($"Invalid input.");
            }
        }
    }

    public class Config
    {
        public int MaxSlimesInHutch { get; set; } = 20;
    }
}
