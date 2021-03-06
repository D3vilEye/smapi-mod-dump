/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ExpandedStorage.Framework.Patches
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ItemPatch : HarmonyPatch
    {
        private readonly Type _type = typeof(Item);
        internal ItemPatch(IMonitor monitor, ModConfig config)
            : base(monitor, config) { }
        
        protected internal override void Apply(HarmonyInstance harmony)
        {
            if (Config.AllowCarryingChests)
            {
                harmony.Patch(AccessTools.Method(_type, nameof(Item.canStackWith), new []{typeof(ISalable)}),
                    new HarmonyMethod(GetType(), nameof(canStackWith_Prefix)));
            }
        }

        /// <summary>Disallow chests containing items to be stacked.</summary>
        public static bool canStackWith_Prefix(Item __instance, ISalable other, ref bool __result)
        {
            var config = ExpandedStorage.GetConfig(__instance);
            if (config == null || __instance is not Chest chest || other is not Chest otherChest)
                return true;
            if (!config.AccessCarried && chest.items.Count == 0 && otherChest.items.Count == 0)
                return true;
            __result = false;
            return false;
        }
    }
}