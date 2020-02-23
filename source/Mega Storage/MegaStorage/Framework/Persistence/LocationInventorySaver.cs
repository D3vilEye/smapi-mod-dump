﻿using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.Persistence
{
    public class LocationInventorySaver : ISaver
    {
        public string SaveDataKey => "LocationInventoryNiceChests";

        private Dictionary<GameLocation, Dictionary<Vector2, Dictionary<int, CustomChest>>> _locationInventoryCustomChests;

        public void HideAndSaveCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("LocationInventorySaver: HideAndSaveCustomChests");
            _locationInventoryCustomChests = new Dictionary<GameLocation, Dictionary<Vector2, Dictionary<int, CustomChest>>>();
            var deserializedChests = new List<DeserializedChest>();
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(x => x.indoors.Value).Where(x => x != null));
            foreach (var location in locations)
            {
                var chestPositions = location.objects.Pairs.Where(x => x.Value is Chest).ToDictionary(pair => pair.Key, pair => (Chest)pair.Value);
                if (!chestPositions.Any())
                {
                    continue;
                }

                var customChestsInChestPositions = new Dictionary<Vector2, Dictionary<int, CustomChest>>();
                foreach (var chestPosition in chestPositions)
                {
                    var position = chestPosition.Key;
                    var chest = chestPosition.Value;
                    var customChestsInChest = chest.items.OfType<CustomChest>().ToList();
                    if (!customChestsInChest.Any())
                    {
                        continue;
                    }

                    var customChestIndexes = new Dictionary<int, CustomChest>();
                    var locationName = location.uniqueName?.Value ?? location.Name;
                    foreach (var customChest in customChestsInChest)
                    {
                        var index = chest.items.IndexOf(customChest);
                        MegaStorageMod.ConvenientChests?.CopyChestData(customChest, chest);
                        chest.items[index] = customChest.ToChest();
                        customChestIndexes.Add(index, customChest);
                        var deserializedChest = customChest.ToDeserializedChest(locationName, position, index);
                        MegaStorageMod.ModMonitor.VerboseLog($"Hiding and saving: {deserializedChest}");
                        deserializedChests.Add(deserializedChest);
                    }
                    customChestsInChestPositions.Add(position, customChestIndexes);
                }
                _locationInventoryCustomChests.Add(location, customChestsInChestPositions);
            }
            if (!Context.IsMainPlayer)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Not main player!");
                return;
            }

            var saveData = new SaveData
            {
                DeserializedChests = deserializedChests
            };
            MegaStorageMod.ModHelper.Data.WriteSaveData(SaveDataKey, saveData);
        }

        public void ReAddCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("LocationInventorySaver: ReAddCustomChests");
            if (_locationInventoryCustomChests == null)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Nothing to re-add");
                return;
            }
            foreach (var customChestLocations in _locationInventoryCustomChests)
            {
                var location = customChestLocations.Key;
                var customChestsInChestPositions = customChestLocations.Value;
                foreach (var customChestsInChestPosition in customChestsInChestPositions)
                {
                    var position = customChestsInChestPosition.Key;
                    var chestIndexes = customChestsInChestPosition.Value;
                    foreach (var chestIndex in chestIndexes)
                    {
                        var index = chestIndex.Key;
                        var customChest = chestIndex.Value;
                        MegaStorageMod.ModMonitor.VerboseLog($"Re-adding: {customChest.Name} in chest at {position} ({index})");
                        var chest = (Chest)location.objects[position];
                        chest.items[index] = customChest;
                    }
                }
            }
        }

        public void LoadCustomChests()
        {
            MegaStorageMod.ModMonitor.VerboseLog("LocationInventorySaver: LoadCustomChests");
            if (!Context.IsMainPlayer)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Not main player!");
                return;
            }
            var saveData = MegaStorageMod.ModHelper.Data.ReadSaveData<SaveData>(SaveDataKey);
            if (saveData == null)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Nothing to load");
                return;
            }
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(x => x.indoors.Value).Where(x => x != null));
            foreach (var location in locations)
            {
                var locationName = location.uniqueName?.Value ?? location.Name;
                var customChestsInLocation = saveData.DeserializedChests.Where(x => x.LocationName == locationName);
                foreach (var deserializedChest in customChestsInLocation)
                {
                    var position = new Vector2(deserializedChest.PositionX, deserializedChest.PositionY);
                    if (!location.objects.ContainsKey(position))
                    {
                        MegaStorageMod.ModMonitor.VerboseLog("WARNING! Expected chest at position: " + position);
                        continue;
                    }
                    var chest = (Chest)location.objects[position];
                    var index = deserializedChest.InventoryIndex;
                    var hiddenCustomChest = (Chest)chest.items[index];
                    var customChest = hiddenCustomChest.ToCustomChest(deserializedChest.ChestType);
                    MegaStorageMod.ModMonitor.VerboseLog($"Loading: {deserializedChest}");
                    MegaStorageMod.ConvenientChests?.CopyChestData(chest, customChest);
                    chest.items[index] = customChest;
                }
            }
        }

    }
}
