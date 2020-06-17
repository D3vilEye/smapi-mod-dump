﻿using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultipleSpouses
{
	public static class EventPatches
	{
		private static IMonitor Monitor;
		private static IModHelper Helper;
		private static List<int[]> weddingPositions = new List<int[]>
		{
			new int[]{26,63,1},
			new int[]{29,63,3},
			new int[]{25,63,1},
			new int[]{30,63,3}
		};

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, IModHelper helper)
		{
			Monitor = monitor;
			Helper = helper;
		}
		public static bool Event_answerDialogueQuestion_Prefix(Event __instance, NPC who, string answerKey)
		{
			try
			{

				if (answerKey == "danceAsk" && !who.HasPartnerForDance && Game1.player.friendshipData[who.Name].IsMarried())
				{
					string accept = "";
					int gender = who.Gender;
					if (gender != 0)
					{
						if (gender == 1)
						{
							accept = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1634");
						}
					}
					else
					{
						accept = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1633");
					}
					try
					{
						Game1.player.changeFriendship(250, Game1.getCharacterFromName(who.Name, true));
					}
					catch
					{
					}
					Game1.player.dancePartner.Value = who;
					who.setNewDialogue(accept, false, false);
					using (List<NPC>.Enumerator enumerator = __instance.actors.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							NPC j = enumerator.Current;
							if (j.CurrentDialogue != null && j.CurrentDialogue.Count > 0 && j.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
							{
								j.CurrentDialogue.Clear();
							}
						}
					}
					Game1.drawDialogue(who);
					who.immediateSpeak = true;
					who.facePlayer(Game1.player);
					who.Halt();
					return false;
				}
			}

			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(Event_answerDialogueQuestion_Prefix)}:\n{ex}", LogLevel.Error);
			}
			return true;
		}

        public static void Event_setUpCharacters_Postfix(Event __instance, GameLocation location)
        {
			try
			{
				foreach(NPC actor in __instance.actors)
                {
					if (__instance.isWedding)
					{
						if (ModEntry.spouses.ContainsKey(actor.Name))
						{
							int idx = ModEntry.spouses.Keys.ToList().IndexOf(actor.Name);
							Vector2 pos;
							if (idx < weddingPositions.Count)
                            {
								pos = new Vector2(weddingPositions[idx][0]*64,weddingPositions[idx][1]*64);
							}
							else
                            {
								int x = 25 + ((idx - 4) % 6);
								int y = 62 - ((idx - 4) / 6);
								pos = new Vector2(x * 64, y * 64);
							}
							actor.position.Value = pos;
							Utility.facePlayerEndBehavior(actor, location);
						}
					}
				}
			}

			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(Event_answerDialogueQuestion_Prefix)}:\n{ex}", LogLevel.Error);
			}
		}
    }
}