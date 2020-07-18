﻿using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;

namespace Magic
{
    public class MultiplayerSaveData
    {
        public static string FilePath => Path.Combine(Constants.CurrentSavePath, "magic0.2.json");

        public class PlayerData
        {
            public int freePoints = 0;

            public SpellBook spellBook = new SpellBook();
        }
        public Dictionary<long, PlayerData> players = new Dictionary<long, PlayerData>();

        internal static JsonSerializerSettings networkSerializerSettings { get; }  = new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
        };

        internal void syncMineFull()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((int)1);
                writer.Write(Game1.player.UniqueMultiplayerID);
                writer.Write(JsonConvert.SerializeObject(players[Game1.player.UniqueMultiplayerID], networkSerializerSettings));
                SpaceCore.Networking.BroadcastMessage(Magic.MSG_DATA, stream.ToArray());
            }
        }
    }
}
