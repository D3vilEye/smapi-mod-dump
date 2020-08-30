﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewAquarium.Editors;
using StardewAquarium.TilesLogic;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewAquarium
{
    internal static class Utils
    {
        private static NetStringList MasterPlayerMail => Game1.MasterPlayer.mailReceived;

        private static IModHelper _helper;
        private static IMonitor _monitor;
        private static IManifest _manifest;

        /// <summary>
        /// Maps the InternalName of the fish to its internalname without spaces, eg. Tranbow Trout to RainbowTrout
        /// </summary>
        public static Dictionary<string, string> InternalNameToDonationName { get; set; } = new Dictionary<string, string>();
        public static List<int> FishIDs = new List<int>();

        /// <summary>
        /// Maps the internal name without spaces to its localized display name
        /// </summary>
        public static Dictionary<string, string> FishDisplayNames { get; set; } = new Dictionary<string, string>();

        private static LastDonatedFishSign _fishSign;

        public static void Initialize(IModHelper helper, IMonitor monitor, IManifest modManifest)
        {
            _helper = helper;
            _monitor = monitor;
            _manifest = modManifest;

            _helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            _helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;

            _fishSign = new LastDonatedFishSign(helper, monitor);
        }

        private static void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            //clear these dictionaries if 
            InternalNameToDonationName.Clear();
            FishDisplayNames.Clear();

            foreach (var kvp in Game1.objectInformation)
            {
                var info = kvp.Value.Split('/');
                var fishName = info[0];
                if (info[3].Contains("-4"))
                {
                    FishIDs.Add(kvp.Key);
                    InternalNameToDonationName.Add(fishName, fishName.Replace(" ",String.Empty));
                    FishDisplayNames.Add(fishName.Replace(" ", String.Empty),info[4]);
                }
            }
        }

        public static bool IsUnDonatedFish(Item i)
        {
            if (i?.Category != -4)
                return false;

            return !MasterPlayerMail.Contains(GetDonatedMailFlag(i));
        }

        public static bool IsUnDonatedFish(string s)
        {
            return !MasterPlayerMail.Contains($"AquariumDonated:{s}");
        }

        public static bool DonateFish(Item i)
        {
            if (!IsUnDonatedFish(i))
                return false;

            MasterPlayerMail.Add(GetDonatedMailFlag(i)); 
            string numDonated = $"AquariumFishDonated:{GetNumDonatedFish()}";
            if (!MasterPlayerMail.Contains(numDonated))
                MasterPlayerMail.Add(numDonated);

            _fishSign.UpdateLastDonatedFish(i);

            return true;
        }

        internal static void TryAwardTrophy(bool puffDonated = false)
        {
            if (Game1.player.freeSpotsInInventory() > 0)
            {
                int id = ModEntry.JsonAssets.GetBigCraftableId("Stardew Aquarium Trophy");
                Object trophy = new Object(Vector2.Zero, id);
                Game1.player.holdUpItemThenMessage(trophy, true);
                Game1.player.addItemToInventory(trophy);
                Game1.MasterPlayer.mailReceived.Add("AquariumTrophyPickedUp");
            }
            else
            {
                Game1.drawObjectDialogue(_helper.Translation.Get("NoInventorySpace"));
            }

            if (puffDonated)
                Game1.activeClickableMenu.exitFunction = () =>
                {
                    Game1.drawObjectDialogue(_helper.Translation.Get("PufferchickDonated"));
                };
        }

        internal static bool PlayerInventoryContains(int fishId)
        {
            foreach (Item item in Game1.player.Items)
            {
                if (item is Object obj && obj.ParentSheetIndex == fishId)
                    return true;
            }

            return false;
        }

        public static int GetNumDonatedFish()
        {
            return MasterPlayerMail.Count(flag => flag.StartsWith("AquariumDonated:"));
        }

        public static string GetDonatedMailFlag(Item i)
        {
            return $"AquariumDonated:{InternalNameToDonationName[i.Name]}";
        }

        public static bool DoesPlayerHaveDonatableFish()
        {
            foreach (var item in Game1.player.Items)
            {
                if (IsUnDonatedFish(item)) return true;
            }

            return false;
        }

        public static IEnumerable<int> GetUndonatedFishInInventory()
        {
            foreach (var item in Game1.player.Items)
            {
                if (IsUnDonatedFish(item)) yield return item.ParentSheetIndex;
            }

        }

        public static void DonationMenuExit(bool achievementUnlock, bool donated, bool pufferchickDonated)
        {
            string mainMessage;

            if (achievementUnlock)
            {
                mainMessage = _helper.Translation.Get("AchievementCongratulations");
                UnlockAchievement();

                var mp = _helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                mp.globalChatInfoMessage("StardewAquarium.AchievementUnlocked");

            }
            else if (donated)
            {
                mainMessage = _helper.Translation.Get("MenuCloseFishDonated");
            }
            else
            {
                mainMessage = _helper.Translation.Get("MenuCloseNoFishDonated");
            }

            Game1.drawObjectDialogue(mainMessage);
            var dialoguesField = _helper.Reflection.GetField<List<string>>(Game1.activeClickableMenu, "dialogues");

            if (pufferchickDonated)
            {
                var dialogues = dialoguesField.GetValue();
                dialogues.Add(_helper.Translation.Get("PufferchickDonated"));
                dialoguesField.SetValue(dialogues);
                return;
            }

            if (achievementUnlock)
            {
                TryAwardTrophy();
            }

        }


        private const string AchievementMessageType = "Achievement";
        public static bool CheckAchievement()
        {
            return GetNumDonatedFish() >= InternalNameToDonationName.Count;
        }

        public static void UnlockAchievement()
        {
            int id = AchievementEditor.AchievementId;
            if (Game1.player.achievements.Contains(id))
                return;

            Game1.player.achievements.Add(id);
            Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("AchievementName"), true));
            Game1.playSound("achievement");

            _helper.Multiplayer.SendMessage(true, AchievementMessageType, modIDs: new[] { _manifest.UniqueID });

        }

        private static void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == _manifest.UniqueID && e.Type == AchievementMessageType)
            {
                UnlockAchievement();
            }
        }
    }
}
