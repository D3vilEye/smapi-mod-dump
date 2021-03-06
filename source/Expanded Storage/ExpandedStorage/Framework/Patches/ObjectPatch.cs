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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ExpandedStorage.Framework.Patches
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ObjectPatch : HarmonyPatch
    {
        private readonly Type _type = typeof(StardewValley.Object);
        internal ObjectPatch(IMonitor monitor, ModConfig config)
            : base(monitor, config) { }
        
        protected internal override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(_type, nameof(StardewValley.Object.placementAction)),
                new HarmonyMethod(GetType(), nameof(PlacementAction)));
            
            if (Config.AllowCarryingChests)
            {
                harmony.Patch(AccessTools.Method(_type, nameof(StardewValley.Object.getDescription)),
                    postfix: new HarmonyMethod(GetType(), nameof(getDescription_Postfix)));
            }
        }
        
        public static bool PlacementAction(StardewValley.Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            var config = ExpandedStorage.GetConfig(__instance);
            
            // Disallow non-placeable storages
            if (config != null && !config.IsPlaceable)
            {
                __result = false;
                return false;
            }
            
            if (config == null)
                return true;
            
            var pos = new Vector2(x, y) / 64f;
            pos.X = (int) pos.X;
            pos.Y = (int) pos.Y;
            if (location.objects.ContainsKey(pos) || location is MineShaft || location is VolcanoDungeon)
                return true;
            
            // Place Expanded Storage Chest
            if (!Enum.TryParse(config.SpecialChestType, out Chest.SpecialChestTypes specialChestType))
                specialChestType = Chest.SpecialChestTypes.None;
            var chest = new Chest(true, pos, __instance.ParentSheetIndex)
            {
                name = __instance.Name,
                shakeTimer = 50,
                SpecialChestType = specialChestType
            };
            chest.owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;
            chest.resetLidFrame();

            // Copy properties from previously held chest
            if (__instance is Chest oldChest)
            {
                chest.playerChoiceColor.Value = oldChest.playerChoiceColor.Value;
                if (oldChest.items.Any())
                    chest.items.CopyFrom(oldChest.items);
            }
            
            foreach (var modData in __instance.modData)
                chest.modData.CopyFrom(modData);
            
            location.objects.Add(pos, chest);
            location.playSound("hammer");
            __result = true;
            return false;
        }

        /// <summary>Adds count of chests contents to its description.</summary>
        public static void getDescription_Postfix(StardewValley.Object __instance, ref string __result)
        {
            if (__instance is not Chest chest || !ExpandedStorage.HasConfig(__instance))
                return;
            if (chest.items?.Count > 0)
                __result += "\n" + $"Contains {chest.items.Count} items.";
        }
    }
}