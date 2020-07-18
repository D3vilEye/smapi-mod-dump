﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using static Harmony.AccessTools;

namespace MultipleSpouses
{
    public static class Maps
	{
		private static IMonitor Monitor;
        private static ModConfig config;
        private static IModHelper PHelper;
        public static Dictionary<string, Map> tmxSpouseRooms = new Dictionary<string, Map>();
		public static Dictionary<string, int> roomIndexes = new Dictionary<string, int>{
			{ "Abigail", 0 },
			{ "Penny", 1 },
			{ "Leah", 2 },
			{ "Haley", 3 },
			{ "Maru", 4 },
			{ "Sebastian", 5 },
			{ "Alex", 6 },
			{ "Harvey", 7 },
			{ "Elliott", 8 },
			{ "Sam", 9 },
			{ "Shane", 10 },
			{ "Emily", 11 },
			{ "Krobus", 12 },
		};

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
		{
			Monitor = monitor;
			config = ModEntry.config;
			PHelper = ModEntry.PHelper;
		}

		public static void BuildSpouseRooms(FarmHouse farmHouse)
		{
			try
			{
				if(farmHouse is Cabin)
                {
					Monitor.Log("BuildSpouseRooms for Cabin");
				}

				Farmer f = farmHouse.owner;
				if (f == null)
					return;
				Misc.ResetSpouses(f);
				Monitor.Log("Building all spouse rooms");
				if (Misc.GetSpouses(f, 1).Count == 0 || farmHouse.upgradeLevel > 3)
                {
					ModEntry.PMonitor.Log("No spouses");
					farmHouse.showSpouseRoom();
					return;
				}

				List<string> spousesWithRooms = new List<string>();

				foreach (string spouse in Misc.GetSpouses(f, 0).Keys)
				{
					Monitor.Log($"checking {spouse} for spouse room");
					if (roomIndexes.ContainsKey(spouse) || tmxSpouseRooms.ContainsKey(spouse))
                    {
						Monitor.Log($"Adding {spouse} to list for spouse rooms");
						spousesWithRooms.Add(spouse);
					}
				}

				if (spousesWithRooms.Count == 0)
				{
					ModEntry.PMonitor.Log("No spouses with rooms");
					return;
				}

				if (f.spouse != null)
				{
					if (!f.friendshipData[f.spouse].IsEngaged() && (roomIndexes.ContainsKey(f.spouse) || tmxSpouseRooms.ContainsKey(f.spouse)))
					{
						Monitor.Log($"Building spouse room for official spouse {f.spouse}");
						BuildOneSpouseRoom(farmHouse, f.spouse, -1);
					}
					else
					{
						Monitor.Log($"No spouse room for official spouse {f.spouse}, placing for {spousesWithRooms[0]} instead.");
						BuildOneSpouseRoom(farmHouse, spousesWithRooms[0], -1);
						spousesWithRooms = new List<string>(spousesWithRooms.Skip(1));
					}
				}

				if (!ModEntry.config.BuildAllSpousesRooms)
					return;

				Monitor.Log($"Building {spousesWithRooms.Count} additional spouse rooms");

				List<string> sheets = new List<string>();
				for (int i = 0; i < farmHouse.map.TileSheets.Count; i++)
				{
					sheets.Add(farmHouse.map.TileSheets[i].Id);
				}
				int untitled = sheets.IndexOf("untitled tile sheet");
				int floorsheet = sheets.IndexOf("walls_and_floors");
				int indoor = sheets.IndexOf("indoor");

				Monitor.Log($"Map has sheets: {string.Join(", ", sheets)}");

				int ox = ModEntry.config.ExistingSpouseRoomOffsetX;
				int oy = ModEntry.config.ExistingSpouseRoomOffsetY;
				if (farmHouse.upgradeLevel > 1)
				{
					ox += 6;
					oy += 9;
				}

				Monitor.Log($"Preliminary adjustments");

				for (int i = 0; i < 7; i++)
				{
					farmHouse.setMapTileIndex(ox + 29 + i, oy + 11, 0, "Buildings", indoor);
					farmHouse.removeTile(ox + 29 + i, oy + 9, "Front");
					farmHouse.removeTile(ox + 29 + i, oy + 10, "Buildings");
					farmHouse.setMapTileIndex(ox + 28 + i, oy + 10, 165, "Front", indoor);
					farmHouse.removeTile(ox + 29 + i, oy + 10, "Back");
				}
				for (int i = 0; i < 8; i++)
				{
					farmHouse.setMapTileIndex(ox + 28 + i, oy + 10, 165, "Front", indoor);
				}
				for (int i = 0; i < 10; i++)
				{
					farmHouse.removeTile(ox + 35, oy + 0 + i, "Buildings");
					farmHouse.removeTile(ox + 35, oy + 0 + i, "Front");
				}
				for (int i = 0; i < 7; i++)
				{
					// horiz hall
					farmHouse.setMapTileIndex(ox + 29 + i, oy + 10, (i % 2 == 0 ? 352: 336), "Back", floorsheet);
				}


				for (int i = 0; i < 7; i++)
				{
					//farmHouse.removeTile(ox + 28, oy + 4 + i, "Back");
					//farmHouse.setMapTileIndex(ox + 28, oy + 4 + i, (i % 2 == 0 ? 352 : ModEntry.config.HallTileEven), "Back", 0);
				}


				farmHouse.removeTile(ox + 28, oy + 9, "Front");
				farmHouse.removeTile(ox + 28, oy + 10, "Buildings");
				
				if(farmHouse.upgradeLevel > 1) 
					farmHouse.setMapTileIndex(ox + 28, oy + 10, 163, "Front", indoor);
				farmHouse.removeTile(ox + 35, oy + 0, "Front");
				farmHouse.removeTile(ox + 35, oy + 0, "Buildings");



				int count = 0;

				ExtendMap(farmHouse, ox + 37 + (7* spousesWithRooms.Count));

				// remove and rebuild spouse rooms
				for (int j = 0; j < spousesWithRooms.Count; j++)
				{
					farmHouse.removeTile(ox + 35 + (7 * count), oy + 0, "Buildings");
					for (int i = 0; i < 10; i++)
					{
						farmHouse.removeTile(ox + 35 + (7 * count), oy + 1 + i, "Buildings");
					}
					BuildOneSpouseRoom(farmHouse, spousesWithRooms[j], count++);
				}

				Monitor.Log($"Building far wall");

				// far wall
				farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 0, 11, "Buildings", indoor);
				for (int i = 0; i < 10; i++)
				{
					farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 1 + i, 68, "Buildings", indoor);
				}
				farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 10, 130, "Front", indoor);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(BuildSpouseRooms)}:\n{ex}", LogLevel.Error);
			}

		}

        public static void BuildOneSpouseRoom(FarmHouse farmHouse, string name, int count)
		{

			NPC spouse = Game1.getCharacterFromName(name);
			string back = "Back";
			string buildings = "Buildings";
			string front = "Front";
			if (spouse != null || name == "")
			{
				if (farmHouse.owner.friendshipData[spouse.Name] != null && farmHouse.owner.friendshipData[spouse.Name].IsEngaged())
					name = "";
				Map refurbishedMap;
				if (name == "")
				{
					refurbishedMap = PHelper.Content.Load<Map>("Maps\\" + farmHouse.Name + ((farmHouse.upgradeLevel == 0) ? "" : ((farmHouse.upgradeLevel == 3) ? "2" : string.Concat(farmHouse.upgradeLevel))) + "_marriage", ContentSource.GameContent);
				}
				else
				{
					refurbishedMap = PHelper.Content.Load<Map>("Maps\\spouseRooms", ContentSource.GameContent);
				}
				int indexInSpouseMapSheet = -1;

                if (roomIndexes.ContainsKey(name))
                {
					indexInSpouseMapSheet = roomIndexes[name];
					if(name == "Emily")
                    {
						farmHouse.temporarySprites.RemoveAll((s) => s is EmilysParrot);

						int offset = (1 + count) * 7 * 64;
						Vector2 parrotSpot = new Vector2(2064f + offset, 160f);
						int upgradeLevel = farmHouse.upgradeLevel;
						if (upgradeLevel - 2 <= 1)
						{
							parrotSpot = new Vector2(2448f + offset, 736f);
						}
						farmHouse.temporarySprites.Add(new EmilysParrot(parrotSpot));
					}
				}
				else if (tmxSpouseRooms.ContainsKey(name))
				{

					refurbishedMap = tmxSpouseRooms[name];
					if (refurbishedMap == null)
					{
						ModEntry.PMonitor.Log($"Couldn't load TMX spouse room for spouse {name}", LogLevel.Error);
						return;
					}
                    try 
					{
						back = refurbishedMap.Layers[0].Id;
						buildings = refurbishedMap.Layers[1].Id;
						front = refurbishedMap.Layers[2].Id;
					}
					catch(Exception ex)
                    {
						ModEntry.PMonitor.Log($"Couldn't load TMX spouse room layers for spouse {name}. Exception: {ex}", LogLevel.Error);
					}

					indexInSpouseMapSheet = 0;
				}
                else if(name != "")
                {
					return;
                }


				Monitor.Log($"Building {name}'s room", LogLevel.Debug);

				Microsoft.Xna.Framework.Rectangle areaToRefurbish = (farmHouse.upgradeLevel == 1) ? new Microsoft.Xna.Framework.Rectangle(36 + (7 * count), 1, 6, 9) : new Microsoft.Xna.Framework.Rectangle(42 + (7 * count), 10, 6, 9);

				Point mapReader;
				if (name == "")
				{
					mapReader = new Point(areaToRefurbish.X, areaToRefurbish.Y);
				}
				else
				{
					mapReader = new Point(indexInSpouseMapSheet % 5 * 6, indexInSpouseMapSheet / 5 * 9);
				}
				farmHouse.map.Properties.Remove("DayTiles");
				farmHouse.map.Properties.Remove("NightTiles");


				List<string> sheetNames = new List<string>();
				for (int i = 0; i < farmHouse.map.TileSheets.Count; i++)
				{
					sheetNames.Add(farmHouse.map.TileSheets[i].Id);
					//Monitor.Log($"tilesheet id: {farmHouse.map.TileSheets[i].Id}");
				}
				int untitled = sheetNames.IndexOf("untitled tile sheet");
				int floorsheet = sheetNames.IndexOf("walls_and_floors");
				int indoor = sheetNames.IndexOf("indoor");


				int ox = ModEntry.config.ExistingSpouseRoomOffsetX;
				int oy = ModEntry.config.ExistingSpouseRoomOffsetY;
				if (farmHouse.upgradeLevel > 1)
				{ 
					ox += 6;
					oy += 9;
				}

				if (ModEntry.config.BuildAllSpousesRooms)
				{



					for (int i = 0; i < 7; i++)
					{
						// bottom wall
						farmHouse.setMapTileIndex(ox + 36 + i + (count * 7), oy + 10, 165, "Front", indoor);
						farmHouse.setMapTileIndex(ox + 36 + i + (count * 7), oy + 11, 0, "Buildings", indoor);


						farmHouse.removeTile(ox + 35 + (7 * count), oy + 4 + i, "Back");

						if (count % 2 == 0)
						{
							// vert hall
							farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 4 + i, (i % 2 == 0 ? 352 : 336), "Back", floorsheet);
							// horiz hall
							farmHouse.setMapTileIndex(ox + 36 + i + (count * 7), oy + 10, (i % 2 == 0 ? 336 : 352), "Back", floorsheet);
						}
						else
						{
							// vert hall
							
							farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 4 + i, (i % 2 == 0 ? 336 : 352), "Back", floorsheet);
							// horiz hall
							farmHouse.setMapTileIndex(ox + 36 + i + (count * 7), oy + 10, (i % 2 == 0 ? 352 : 336), "Back", floorsheet);
						}
					}

					for (int i = 0; i < 6; i++)
					{
						// top wall
						farmHouse.setMapTileIndex(ox + 36 + i + (count * 7), oy + 0, 2, "Buildings", indoor);
					}

					// vert wall
					farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 0, 87, "Buildings", untitled);
					farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 1, 99, "Buildings", untitled);
					farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 2, 111, "Buildings", untitled);
					farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 3, 123, "Buildings", untitled);
					farmHouse.setMapTileIndex(ox + 35 + (7 * count), oy + 4, 135, "Buildings", untitled);

				}
				for (int x = 0; x < areaToRefurbish.Width; x++)
				{
					for (int y = 0; y < areaToRefurbish.Height; y++)
					{
						//PMonitor.Log($"x {x}, y {y}", LogLevel.Debug);
						if (refurbishedMap.GetLayer(back).Tiles[mapReader.X + x, mapReader.Y + y] != null)
						{
							farmHouse.map.GetLayer("Back").Tiles[areaToRefurbish.X + x, areaToRefurbish.Y + y] = new StaticTile(farmHouse.map.GetLayer("Back"), farmHouse.map.GetTileSheet(refurbishedMap.GetLayer(back).Tiles[mapReader.X + x, mapReader.Y + y].TileSheet.Id), BlendMode.Alpha, refurbishedMap.GetLayer(back).Tiles[mapReader.X + x, mapReader.Y + y].TileIndex);
						}
						if (refurbishedMap.GetLayer(buildings).Tiles[mapReader.X + x, mapReader.Y + y] != null)
						{
							farmHouse.map.GetLayer("Buildings").Tiles[areaToRefurbish.X + x, areaToRefurbish.Y + y] = new StaticTile(farmHouse.map.GetLayer("Buildings"), farmHouse.map.GetTileSheet(refurbishedMap.GetLayer(buildings).Tiles[mapReader.X + x, mapReader.Y + y].TileSheet.Id), BlendMode.Alpha, refurbishedMap.GetLayer(buildings).Tiles[mapReader.X + x, mapReader.Y + y].TileIndex);

							typeof(GameLocation).GetMethod("adjustMapLightPropertiesForLamp", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(farmHouse, new object[] { refurbishedMap.GetLayer(buildings).Tiles[mapReader.X + x, mapReader.Y + y].TileIndex, areaToRefurbish.X + x, areaToRefurbish.Y + y, "Buildings" });
						}
						else
						{
							farmHouse.map.GetLayer("Buildings").Tiles[areaToRefurbish.X + x, areaToRefurbish.Y + y] = null;
						}
						if (y < areaToRefurbish.Height - 1 && refurbishedMap.GetLayer(front).Tiles[mapReader.X + x, mapReader.Y + y] != null)
						{
							farmHouse.map.GetLayer("Front").Tiles[areaToRefurbish.X + x, areaToRefurbish.Y + y] = new StaticTile(farmHouse.map.GetLayer("Front"), farmHouse.map.GetTileSheet(refurbishedMap.GetLayer(front).Tiles[mapReader.X + x, mapReader.Y + y].TileSheet.Id), BlendMode.Alpha, refurbishedMap.GetLayer(front).Tiles[mapReader.X + x, mapReader.Y + y].TileIndex);
							typeof(GameLocation).GetMethod("adjustMapLightPropertiesForLamp", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(farmHouse, new object[] { refurbishedMap.GetLayer(front).Tiles[mapReader.X + x, mapReader.Y + y].TileIndex, areaToRefurbish.X + x, areaToRefurbish.Y + y, "Front" });
						}
						else if (y < areaToRefurbish.Height - 1)
						{
							farmHouse.map.GetLayer("Front").Tiles[areaToRefurbish.X + x, areaToRefurbish.Y + y] = null;
						}
						if (x == 4 && y == 4)
						{
							try
							{
								farmHouse.map.GetLayer("Back").Tiles[areaToRefurbish.X + x, areaToRefurbish.Y + y].Properties["NoFurniture"] = new PropertyValue("T");
							}
							catch (Exception ex)
							{
								Monitor.Log(ex.ToString());
							}
						}
					}
				}

				if(name == "Sebastian" && Game1.netWorldState.Value.hasWorldStateID("sebastianFrog"))
                {
					Monitor.Log("building Sebastian's terrarium");
					Vector2 spot = new Vector2(37 + (7 * count) + ox, 7 + oy);
					farmHouse.removeTile((int)spot.X, (int)spot.Y - 1, "Front");
					farmHouse.removeTile((int)spot.X + 1, (int)spot.Y - 1, "Front");
					farmHouse.removeTile((int)spot.X + 2, (int)spot.Y - 1, "Front");
					farmHouse.temporarySprites.Add(new TemporaryAnimatedSprite
					{
						texture = Game1.mouseCursors,
						sourceRect = new Microsoft.Xna.Framework.Rectangle(641, 1534, 48, 37),
						animationLength = 1,
						sourceRectStartingPos = new Vector2(641f, 1534f),
						interval = 5000f,
						totalNumberOfLoops = 9999,
						position = spot * 64f + new Vector2(0f, -5f) * 4f,
						scale = 4f,
						layerDepth = (spot.Y + 2f + 0.1f) * 64f / 10000f
					});
					if (Game1.random.NextDouble() < 0.85)
					{
						Texture2D crittersText2 = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
						farmHouse.TemporarySprites.Add(new SebsFrogs
						{
							texture = crittersText2,
							sourceRect = new Microsoft.Xna.Framework.Rectangle(64, 224, 16, 16),
							animationLength = 1,
							sourceRectStartingPos = new Vector2(64f, 224f),
							interval = 100f,
							totalNumberOfLoops = 9999,
							position = spot * 64f + new Vector2((float)((Game1.random.NextDouble() < 0.5) ? 22 : 25), (float)((Game1.random.NextDouble() < 0.5) ? 2 : 1)) * 4f,
							scale = 4f,
							flipped = (Game1.random.NextDouble() < 0.5),
							layerDepth = (spot.Y + 2f + 0.11f) * 64f / 10000f,
							Parent = farmHouse
						});
					}
					if (!Game1.player.activeDialogueEvents.ContainsKey("sebastianFrog2") && Game1.random.NextDouble() < 0.5)
					{
						Texture2D crittersText3 = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
						farmHouse.TemporarySprites.Add(new SebsFrogs
						{
							texture = crittersText3,
							sourceRect = new Microsoft.Xna.Framework.Rectangle(64, 240, 16, 16),
							animationLength = 1,
							sourceRectStartingPos = new Vector2(64f, 240f),
							interval = 150f,
							totalNumberOfLoops = 9999,
							position = spot * 64f + new Vector2(8f, 3f) * 4f,
							scale = 4f,
							layerDepth = (spot.Y + 2f + 0.11f) * 64f / 10000f,
							flipped = (Game1.random.NextDouble() < 0.5),
							pingPong = false,
							Parent = farmHouse
						});
					}
				}

				/*
				List<string> sheets = new List<string>();
				for (int i = 0; i < farmHouse.map.TileSheets.Count; i++)
				{
					sheets.Add(farmHouse.map.TileSheets[i].Id);
				}
				for (int i = 0; i < 7; i++)
				{

					// vert floor ext
					farmHouse.removeTile(ox + 42 + (7 * count), oy + 4 + i, "Back");
					farmHouse.setMapTileIndex(ox + 42 + (7 * count), oy + 4 + i, farmHouse.getTileIndexAt(ox + 41 + (7 * count), oy + 4 + i, "Back"), "Back", sheets.IndexOf(farmHouse.getTileSheetIDAt(ox + 41 + (7 * count), oy + 4 + i, "Back")));
				}
				*/
			}
		}

        private static void ExtendMap(FarmHouse farmHouse, int v)
        {
			ModEntry.PMonitor.Log($"Extending map width to {v}");
			List<Layer> layers = FieldRefAccess<Map, List<Layer>>(farmHouse.map, "m_layers");
			for (int i = 0; i < layers.Count; i++)
			{
				Tile[,] tiles = FieldRefAccess<Layer, Tile[,]>(layers[i], "m_tiles");
				Size size = FieldRefAccess<Layer, Size>(layers[i], "m_layerSize");
				if (size.Width >= v)
					continue;
				size = new Size(v, size.Height);
				FieldRefAccess<Layer, Size>(layers[i], "m_layerSize") = size;
				FieldRefAccess<Map, List<Layer>>(farmHouse.map, "m_layers") = layers;

				Tile[,] newTiles = new Tile[v, tiles.GetLength(1)];

				for (int k = 0; k < tiles.GetLength(0); k++)
				{
					for (int l = 0; l < tiles.GetLength(1); l++)
					{
						newTiles[k, l] = tiles[k, l];
					}
				}

				FieldRefAccess<Layer, Tile[,]>(layers[i], "m_tiles") = newTiles;
				FieldRefAccess<Layer, TileArray>(layers[i], "m_tileArray") = new TileArray(layers[i], newTiles);
			}
			FieldRefAccess<Map, List<Layer>>(farmHouse.map, "m_layers") = layers;
		}

		public static void ReplaceBed(FarmHouse farmHouse)
		{
			try
			{
				Monitor.Log("Replacing bed");
				if (farmHouse == null || farmHouse.map == null)
					return;

				// bed
				Map map;
				if (ModEntry.config.SleepOnCovers && !config.TransparentSheets)
				{
					map = PHelper.Content.Load<Map>("assets/CustomBed.tmx");
				}
				else
				{
					map = PHelper.Content.Load<Map>("Maps\\" + farmHouse.Name + ((farmHouse.upgradeLevel == 0) ? "" : (((farmHouse.upgradeLevel == 3) ? "2" : string.Concat(farmHouse.upgradeLevel)) + "_marriage")), ContentSource.GameContent);

					if (!ModEntry.config.TransparentSheets && farmHouse.owner != null && !farmHouse.owner.activeDialogueEvents.ContainsKey("pennyRedecorating"))
					{
						int whichQuilt = -1;
						if (farmHouse.owner.mailReceived.Contains("pennyQuilt0"))
						{
							whichQuilt = 0;
						}
						else if (farmHouse.owner.mailReceived.Contains("pennyQuilt1"))
						{
							whichQuilt = 1;
						}
						else if (farmHouse.owner.mailReceived.Contains("pennyQuilt2"))
						{
							whichQuilt = 2;
						}
						if (whichQuilt != -1)
						{
							Point startTile = Point.Zero;
							if (farmHouse.upgradeLevel >= 2)
							{
								startTile = new Point(27, 12);
							}
							else if (farmHouse.upgradeLevel == 1)
							{
								startTile = new Point(21, 3);
							}
							if (!startTile.Equals(Point.Zero))
							{
								int startIndex = 61 + whichQuilt * 3;
								setMapTileIndex(ref map, startTile.X, startTile.Y, startIndex, "Front", 1);
								setMapTileIndex(ref map, startTile.X + 1, startTile.Y, startIndex + 1, "Front", 1);
								setMapTileIndex(ref map, startTile.X + 2, startTile.Y, startIndex + 2, "Front", 1);
								setMapTileIndex(ref map, startTile.X, startTile.Y + 1, startIndex + 12, "Front", 1);
								setMapTileIndex(ref map, startTile.X + 1, startTile.Y + 1, startIndex + 13, "Front", 1);
								setMapTileIndex(ref map, startTile.X + 2, startTile.Y + 1, startIndex + 14, "Front", 1);
							}
						}
					}
				}
				int untitled = 0;
				List<string> sheets = new List<string>();
				for (int i = 0; i < farmHouse.map.TileSheets.Count; i++)
				{
					sheets.Add(farmHouse.map.TileSheets[i].Id);
					Monitor.Log($"bed sheet {farmHouse.map.TileSheets[i].Id} index: {i}");
				}
				untitled = sheets.IndexOf("untitled tile sheet");


				int ox = ModEntry.config.ExistingBedOffsetX;
				int oy = ModEntry.config.ExistingBedOffsetY;
				if (farmHouse.upgradeLevel > 1)
				{
					ox += 6;
					oy += 9;
				}

				int bedWidth = Misc.GetBedWidth(farmHouse);
				int width = bedWidth - 1;
				int start = 21 - (farmHouse.upgradeLevel > 1 ? (bedWidth / 2) - 1 : 0);

				List<int> backIndexes = new List<int>();
				List<int> frontIndexes = new List<int>();
				List<int> buildIndexes = new List<int>();
				List<int> backSheets = new List<int>();
				List<int> frontSheets = new List<int>();
				List<int> buildSheets = new List<int>();

				int sheetx = 0;
				int sheety = 0;
				if (!ModEntry.config.SleepOnCovers || config.TransparentSheets)
                {
					sheetx = ox + 21;
					sheety = oy + 2;
				}

				for (int i = 0; i < 12; i++)
				{
					farmHouse.removeTile(ox + 21 + (i % 3), oy + 2 + (i / 3), "Front");
					if (i > 2 && i < 9)
                    {
						//farmHouse.removeTile(ox + 21 + (i % 3), oy + 2 + (i / 3), "Buildings");
					}
					backIndexes.Add(getTileIndexAt(map, sheetx + (i % 3), sheety + (i / 3), "Back"));
					frontIndexes.Add(getTileIndexAt(map, sheetx + (i % 3), sheety + (i / 3), "Front"));
					buildIndexes.Add(getTileIndexAt(map, sheetx + (i % 3), sheety + (i / 3), "Buildings"));
					if (ModEntry.config.SleepOnCovers && !config.TransparentSheets)
                    {
						backSheets.Add(untitled);
						frontSheets.Add(untitled);
						buildSheets.Add(untitled);
					}
					else
                    {
						backSheets.Add(sheets.IndexOf(getTileSheetIDAt(map, sheetx + (i % 3), sheety + (i / 3), "Back")));
						frontSheets.Add(sheets.IndexOf(getTileSheetIDAt(map, sheetx + (i % 3), sheety + (i / 3), "Front")));
						buildSheets.Add(sheets.IndexOf(getTileSheetIDAt(map, sheetx + (i % 3), sheety + (i / 3), "Buildings")));
					}
				}


				setupTile(start + ox, 2 + oy, 0, 0, farmHouse, frontIndexes, frontSheets, 3, 0);
				setupTile(start + ox, 3 + oy, 0, 1, farmHouse, frontIndexes, frontSheets, 3, 0);
				setupTile(start + ox, 3 + oy, 0, 1, farmHouse, buildIndexes, buildSheets, 3, 1);
				setupTile(start + ox, 4 + oy, 0, 2, farmHouse, frontIndexes, frontSheets, 3, 0);
				setupTile(start + ox, 4 + oy, 0, 2, farmHouse, backIndexes, backSheets, 3, 2);
				setupTile(start + ox, 5 + oy, 0, 3, farmHouse, buildIndexes, buildSheets, 3, 1);

                if (ModEntry.config.TransparentSheets)
                {
					farmHouse.removeTile(start + ox, 4 + oy, "Back");
					farmHouse.setMapTileIndex(start + ox, 4 + oy, 212, "Back", untitled);
				}

				farmHouse.setTileProperty(start + ox, oy + 3, "Back", "Bed", "T");
				farmHouse.setTileProperty(start + ox, oy + 4, "Back", "Bed", "T");
				farmHouse.setTileProperty(start + ox, oy + 3, "Back", "NoFurniture", "T");
				farmHouse.setTileProperty(start + ox, oy + 4, "Back", "NoFurniture", "T");

				//farmHouse.removeTile(ox + start, oy + 3, "Buildings");
				for (int i = 1; i < width; i++)
				{
					//farmHouse.removeTile(ox + start + i, oy + 3, "Buildings");

					setupTile(i + start + ox, 2 + oy, 1, 0, farmHouse, frontIndexes, frontSheets, 3, 0);
					setupTile(i + start + ox, 3 + oy, 1, 1, farmHouse, frontIndexes, frontSheets, 3, 0);
					setupTile(i + start + ox, 3 + oy, 1, 1, farmHouse, buildIndexes, buildSheets, 3, 1);
					setupTile(i + start + ox, 4 + oy, 1, 2, farmHouse, frontIndexes, frontSheets, 3, 0);
					setupTile(i + start + ox, 4 + oy, 1, 2, farmHouse, backIndexes, backSheets, 3, 2);
					setupTile(i + start + ox, 5 + oy, 1, 3, farmHouse, buildIndexes, buildSheets, 3, 1);

					if (ModEntry.config.TransparentSheets)
					{
						farmHouse.removeTile(i + start + ox, 4 + oy, "Back");
						farmHouse.setMapTileIndex(i + start + ox, 4 + oy, 213, "Back", untitled);
					}

					farmHouse.setTileProperty(i + start + ox, oy + 3, "Back", "Bed", "T");
					farmHouse.setTileProperty(i + start + ox, oy + 4, "Back", "Bed", "T");
					farmHouse.setTileProperty(i + start + ox, oy + 3, "Back", "NoFurniture", "T");
					farmHouse.setTileProperty(i + start + ox, oy + 4, "Back", "NoFurniture", "T");
				}
				//farmHouse.removeTile(width + ox + start, oy + 3, "Buildings");

				setupTile(width + start + ox, 2 + oy, 2, 0, farmHouse, frontIndexes, frontSheets, 3, 0);
				setupTile(width + start + ox, 3 + oy, 2, 1, farmHouse, frontIndexes, frontSheets, 3, 0);
				setupTile(width + start + ox, 3 + oy, 2, 1, farmHouse, buildIndexes, buildSheets, 3, 1);
				setupTile(width + start + ox, 4 + oy, 2, 2, farmHouse, frontIndexes, frontSheets, 3, 0);
				setupTile(width + start + ox, 4 + oy, 2, 2, farmHouse, backIndexes, backSheets, 3, 2);
				setupTile(width + start + ox, 5 + oy, 2, 3, farmHouse, buildIndexes, buildSheets, 3, 1);

				if (ModEntry.config.TransparentSheets)
				{
					farmHouse.removeTile(width + start + ox, 4 + oy, "Back");
					farmHouse.setMapTileIndex(width + start + ox, 4 + oy, 214, "Back", untitled);
				}

				farmHouse.setTileProperty(width + start + ox, oy + 3, "Back", "Bed", "T");
				farmHouse.setTileProperty(width + start + ox, oy + 4, "Back", "Bed", "T");
				farmHouse.setTileProperty(width + start + ox, oy + 3, "Back", "NoFurniture", "T");
				farmHouse.setTileProperty(width + start + ox, oy + 4, "Back", "NoFurniture", "T");

				farmHouse.setTileProperty(21 + ox, oy + 4, "Back", "asdf", "asdf");
				farmHouse.setTileProperty(22 + ox, oy + 4, "Back", "TouchAction", "Sleep");
				farmHouse.setTileProperty(23 + ox, oy + 4, "Back", "TouchAction", "Sleep2");

			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ReplaceBed)}:\n{ex}", LogLevel.Error);
			}

		}


		private static void setupTile(int x1, int y1, int x, int y, FarmHouse farmHouse, List<int> indexes, List<int> sheets, int width, int sheetNo)
        {
			string[] sheetNames = {
				"Front",
				"Buildings",
				"Back"
			};
			int idx = y * width + x;
			try
			{
				farmHouse.removeTile(x1, y1, sheetNames[sheetNo]);
				farmHouse.setMapTileIndex(x1, y1, indexes[idx], sheetNames[sheetNo], sheets[idx]);
			}
            catch(Exception ex)
            {
				Monitor.Log("x1: "+x1);
				Monitor.Log("y1: "+y1);
				Monitor.Log("x: "+x);
				Monitor.Log("y: "+y);
				Monitor.Log("sheet: "+sheetNo);
				Monitor.Log("index: "+ idx);
				Monitor.Log("Exception: "+ex, LogLevel.Error);
            }
		}
		internal static void ExpandKidsRoom(FarmHouse farmHouse)
		{

			ModEntry.PMonitor.Log("Expanding kids room");

			int extraWidth = Math.Max(ModEntry.config.ExtraCribs,0) * 3 + Math.Max(ModEntry.config.ExtraKidsRoomWidth, 0) + Math.Max(ModEntry.config.ExtraKidsBeds, 0) * 4;
			int width = 14;
			int height = 9;
			int startx = 15;
			int starty = 0;
			int ox = ModEntry.config.ExistingKidsRoomOffsetX;
			int oy = ModEntry.config.ExistingKidsRoomOffsetY;

			Map map = PHelper.Content.Load<Map>("Maps\\" + farmHouse.Name + "2"+ (Misc.GetSpouses(farmHouse.owner,1).Count > 0? "_marriage":""), ContentSource.GameContent);

			List<string> sheets = new List<string>();
			for (int i = 0; i < map.TileSheets.Count; i++)
			{
				sheets.Add(map.TileSheets[i].Id);
			}

			List<int> backIndexes = new List<int>();
			List<int> frontIndexes = new List<int>();
			List<int> buildIndexes = new List<int>();
			List<int> backSheets = new List<int>();
			List<int> frontSheets = new List<int>();
			List<int> buildSheets = new List<int>();


			for (int i = 0; i < width * height; i++)
			{
				backIndexes.Add(getTileIndexAt(map, ox + startx + (i % width), oy + starty + (i / width), "Back"));
				backSheets.Add(sheets.IndexOf(getTileSheetIDAt(map, ox + startx + (i % width), oy + starty + (i / width), "Back")));
				frontIndexes.Add(getTileIndexAt(map, ox + startx + (i % width), oy + starty + (i / width), "Front"));
				frontSheets.Add(sheets.IndexOf(getTileSheetIDAt(map, ox + startx + (i % width), oy + starty + (i / width), "Front")));
				buildIndexes.Add(getTileIndexAt(map, ox + startx + (i % width), oy + starty + (i / width), "Buildings"));
				buildSheets.Add(sheets.IndexOf(getTileSheetIDAt(map, ox + startx + (i % width), oy + starty + (i / width), "Buildings")));
			}

			if(extraWidth > 0)
            {
				Monitor.Log("total width: " + (29 + ox + extraWidth));
				ExtendMap(farmHouse, 29 + ox + extraWidth);
			}

			int cribsWidth = (Math.Max(ModEntry.config.ExtraCribs, 0) + 1)* 3;
			int bedsWidth = (Math.Max(ModEntry.config.ExtraKidsBeds, 0) + 1) * 4;
			int spaceWidth = Math.Max(ModEntry.config.ExtraKidsRoomWidth, 0) + 3;

			// cribs
			int k = 0;

			if (ModEntry.config.ExtraCribs >= 0)
            {
				for (int j = 0; j < ModEntry.config.ExtraCribs + 1; j++)
				{
					for (int i = 0; i < 3 * height; i++)
					{
						int x = ox + startx + (3 * j) + i % 3;
						int y = oy + starty + i / 3;
						int xt = i % 3;
						int yt = i / 3;

						setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
						setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
						setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
					}
					farmHouse.setTileProperty(ox + startx + (3 * j), oy + starty + 5, "Buildings", "Action", $"Crib{j}");
					farmHouse.setTileProperty(ox + startx + (3 * j) + 1, oy + starty + 5, "Buildings", "Action", $"Crib{j}");
					farmHouse.setTileProperty(ox + startx + (3 * j) + 2, oy + starty + 5, "Buildings", "Action", $"Crib{j}");
				}

			}
			else // remove existing crib
            {
				k = 0;
				for (int i = 0; i < 3 * height; i++)
				{
					k %= (3 * height);

					int x = ox + startx + i % 3;
					int y = oy + starty + i / 3;
					int xt = 3 + (k % 3);
					int yt = k / 3;

					setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
					k++;
				}
			}

			// mid space

			k = 0;
			for (int i = 0; i < 3 * height; i++)
			{
				k %= (3 * height);

				int x = cribsWidth + ox + startx + i % 3;
				int y = oy + starty + i / 3;
				int xt = 3 + (k % 3);
				int yt = k / 3;

				setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
				setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
				setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
				k++;
			}

			if(ModEntry.config.ExtraKidsRoomWidth > 0)
            {
				k = 0;
				for (int j = 0; j < ModEntry.config.ExtraKidsRoomWidth / 3; j++)
				{
					k = 0;
					for (int i = 0; i < 3 * height; i++)
					{
						k %= (3 * height);

						int x = cribsWidth + 3 + ox + startx + (3 * j) + i % 3;
						int y = oy + starty + i / 3;
						int xt = 3 + (k % 3);
						int yt = k / 3;

						setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
						setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
						setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
						k++;
					}
				}
				for (int j = 0; j < ModEntry.config.ExtraKidsRoomWidth % 3; j++)
				{
					k %= 3;
					for (int i = 0; i < height; i++)
					{

						int x = cribsWidth + 3 + (3 * (ModEntry.config.ExtraKidsRoomWidth / 3)) + ox + startx + j;
						int y = oy + starty + i;
						int xt = 3 + j;
						int yt = i;

						setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
						setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
						setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
					}
				}
			}


			// beds
			if (ModEntry.config.ExtraKidsBeds >= 0)
			{
				for (int j = 0; j < ModEntry.config.ExtraKidsBeds + 1; j++)
				{
					k = 0;
					for (int i = 0; i < 4 * height; i++)
					{
						k %= (4 * height);

						int x = cribsWidth + spaceWidth + ox + startx + (4 * j) + i % 4;
						int y = oy + starty + i / 4;
						int xt = 6 + (k % 4);
						int yt = k / 4;

						setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
						setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
						setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
						k++;
					}
				}

				// far bed and wall
				k = 0;
				for (int i = 0; i < 4 * height; i++)
				{
					k %= (4 * height);
					setupTile(cribsWidth + spaceWidth + bedsWidth + ox + startx + i % 4, oy + starty + i / 4, 10 + (k % 4), k / 4, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(cribsWidth + spaceWidth + bedsWidth + ox + startx + i % 4, oy + starty + i / 4, 10 + (k % 4), k / 4, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(cribsWidth + spaceWidth + bedsWidth + ox + startx + i % 4, oy + starty + i / 4, 10 + (k % 4), k / 4, farmHouse, backIndexes, backSheets, 14, 2);
					k++;
				}
			}
			else if (ModEntry.config.ExtraKidsBeds == -1)
			{
				// remove left bed

				k = 0;
				for (int i = 0; i < 3 * height; i++)
				{
					k %= (3 * height);

					int x = cribsWidth + spaceWidth + ox + startx + i % 3;
					int y = oy + starty + i / 3;
					int xt = 3 + (k % 3);
					int yt = k / 3;

					setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
					k++;
				}
				// extra strip

				for (int i = 0; i < height; i++)
				{
					setupTile(cribsWidth + spaceWidth + 3 + ox + startx, oy + starty + i, 3, i, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(cribsWidth + spaceWidth + 3 + ox + startx, oy + starty + i, 3, i, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(cribsWidth + spaceWidth + 3 + ox + startx, oy + starty + i, 3, i, farmHouse, backIndexes, backSheets, 14, 2);
				}

				// far wall and bed
				k = 0;

				for (int i = 0; i < 4 * height; i++)
				{
					k %= (4 * height);
					setupTile(cribsWidth + spaceWidth + 4 + ox + startx + i % 4, oy + starty + i / 4, 10 + (k % 4), k / 4, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(cribsWidth + spaceWidth + 4 + ox + startx + i % 4, oy + starty + i / 4, 10 + (k % 4), k / 4, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(cribsWidth + spaceWidth + 4 + ox + startx + i % 4, oy + starty + i / 4, 10 + (k % 4), k / 4, farmHouse, backIndexes, backSheets, 14, 2);
					k++;
				}
			}
			else
			{
				// remove both beds
				k = 0;
				for (int i = 0; i < 3 * height; i++)
				{
					k %= (3 * height);

					int x = cribsWidth + spaceWidth + ox + startx + i % 3;
					int y = oy + starty + i / 3;
					int xt = 3 + (k % 3);
					int yt = k / 3;

					setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
					k++;
				}
				k = 0;
				for (int i = 0; i < 3 * height; i++)
				{
					k %= (3 * height);

					int x = cribsWidth + spaceWidth + 3 + ox + startx + i % 3;
					int y = oy + starty + i / 3;
					int xt = 3 + (k % 3);
					int yt = k / 3;

					setupTile(x, y, xt, yt, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(x, y, xt, yt, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(x, y, xt, yt, farmHouse, backIndexes, backSheets, 14, 2);
					k++;
				}

				// extra strip

				for (int i = 0; i < height; i++)
				{
					setupTile(cribsWidth + spaceWidth + 6 + ox + startx, oy + starty + i, i == height - 1 ? 12 : 3, i, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(cribsWidth + spaceWidth + 6 + ox + startx, oy + starty + i, 3, i, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(cribsWidth + spaceWidth + 6 + ox + startx, oy + starty + i, 3, i, farmHouse, backIndexes, backSheets, 14, 2);
				}

				// far wall

				for (int i = 0; i < height; i++)
				{
					setupTile(cribsWidth + spaceWidth + 7 + ox + startx, oy + starty + i, 13, i, farmHouse, frontIndexes, frontSheets, 14, 0);
					setupTile(cribsWidth + spaceWidth + 7 + ox + startx, oy + starty + i, 13, i, farmHouse, buildIndexes, buildSheets, 14, 1);
					setupTile(cribsWidth + spaceWidth + 7 + ox + startx, oy + starty + i, 13, i, farmHouse, backIndexes, backSheets, 14, 2);
				}

			}
			// bottom
			for (int i = 0; i < extraWidth; i++)
			{
				Tile tile = map.GetLayer("Buildings").PickTile(new Location((29 + ox + i) * Game1.tileSize, (9 + oy) * Game1.tileSize), Game1.viewport.Size);
				if (tile == null || tile.TileIndex == -1)
				{
					Monitor.Log($"Adding building tile at {29 + ox + i},{ 9 + oy}");
					farmHouse.setMapTileIndex(29 + ox + i, 9 + oy, 0, "Buildings");
				}
			}

		}

		public static string getTileSheetIDAt(Map map, int x, int y, string layer)
		{
			if (map.GetLayer(layer) == null)
			{
				return "";
			}
			Tile tmp = map.GetLayer(layer).Tiles[x, y];
			if (tmp != null)
			{
				return tmp.TileSheet.Id;
			}
			return "";
		}

		public static int getTileIndexAt(Map map, int x, int y, string layer)
		{
			if (map.GetLayer(layer) == null)
			{
				return -1;
			}
			Tile tmp = map.GetLayer(layer).Tiles[x, y];
			if (tmp != null)
			{
				return tmp.TileIndex;
			}
			return -1;
		}

		public static void setMapTileIndex(ref Map map, int tileX, int tileY, int index, string layer, int whichTileSheet = 0)
		{
			try
			{
				if (map.GetLayer(layer).Tiles[tileX, tileY] != null)
				{
					if (index == -1)
					{
						map.GetLayer(layer).Tiles[tileX, tileY] = null;
					}
					else
					{
						map.GetLayer(layer).Tiles[tileX, tileY].TileIndex = index;
					}
				}
				else
				{
					map.GetLayer(layer).Tiles[tileX, tileY] = new StaticTile(map.GetLayer(layer), map.TileSheets[whichTileSheet], BlendMode.Alpha, index);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}