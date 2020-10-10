/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using StardewValley;
using SObject = StardewValley.Object;

namespace Entoarox.DynamicDungeons
{
    internal class DynamicDungeonsAPI
    {
        /*********
        ** Accessors
        *********/
        public double CurrentDifficulty => ModEntry.Location?.Difficulty ?? 0;
        public int CurrentFloorLevel => ModEntry.Location?.Floor ?? 0;
        public bool InDungeon => Game1.currentLocation != null && Game1.currentLocation is DynamicDungeon;


        /*********
        ** Public methods
        *********/
        public void RegisterLootEntry(string table, double dropChance, SObject itemLoot)
        {
            if (!LootHandler.LootTables.ContainsKey(table))
                LootHandler.LootTables.Add(table, new LootHandler());
            LootHandler.LootTables[table].Add(dropChance, itemLoot);
        }

        public void RegisterLootEntry(string table, double dropChance, Func<SObject> itemLootCallback)
        {
            if (!LootHandler.LootTables.ContainsKey(table))
                LootHandler.LootTables.Add(table, new LootHandler());
            LootHandler.LootTables[table].Add(dropChance, itemLootCallback);
        }
    }
}
