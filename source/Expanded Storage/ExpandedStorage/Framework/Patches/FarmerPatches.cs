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
using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace ExpandedStorage.Framework.Patches
{
    internal class FarmerPatches : HarmonyPatch
    {
        private readonly Type _type = typeof(Farmer);
        
        internal FarmerPatches(IMonitor monitor, ModConfig config)
            : base(monitor, config) { }

        protected internal override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(_type, nameof(Farmer.addItemToInventory), new []{typeof(Item), typeof(List<Item>)}),
                new HarmonyMethod(GetType(), nameof(addItemToInventory_Prefix)));
        }

        public static bool addItemToInventory_Prefix(Farmer __instance, ref Item __result, Item item, List<Item> affected_items_list)
        {
            var config = ExpandedStorage.GetConfig(item);
            if(config == null || !config.AccessCarried)
                return true;

            if (item is not Chest chest)
            {
                if (!Enum.TryParse(config.SpecialChestType, out Chest.SpecialChestTypes specialChestType))
                    specialChestType = Chest.SpecialChestTypes.None;
                chest = new Chest(true, Vector2.Zero, item.ParentSheetIndex)
                {
                    name = item.Name,
                    Stack = item.Stack,
                    SpecialChestType = specialChestType
                };
            }
            chest.resetLidFrame();
            
            foreach (var modData in item.modData)
                chest.modData.CopyFrom(modData);
            
            // Find first stackable slot
            for (var j = 0; j < __instance.MaxItems; j++)
            {
                if (j >= __instance.Items.Count
                    || __instance.Items[j] == null
                    || !__instance.Items[j].Name.Equals(item.Name)
                    || __instance.Items[j].ParentSheetIndex != item.ParentSheetIndex
                    || !chest.canStackWith(__instance.Items[j]))
                    continue;
                
                var stackLeft = __instance.Items[j].addToStack(chest);
                affected_items_list?.Add(__instance.Items[j]);
                if (stackLeft <= 0)
                {
                    __result = null;
                    return false;
                }
                chest.Stack = stackLeft;
            }
            
            // Find first empty slot
            for (var i = 0; i < __instance.MaxItems; i++)
            {
                if (i > __instance.Items.Count || __instance.Items[i] != null)
                    continue;
                
                __instance.Items[i] = chest;
                affected_items_list?.Add(__instance.Items[i]);
                
                __result = null;
                return false;
            }
            
            __result = chest;
            return false;
        }
    }
}