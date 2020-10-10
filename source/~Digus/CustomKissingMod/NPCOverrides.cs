/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace CustomKissingMod
{
    internal class NPCOverrides
    {
        public static void checkAction(NPC __instance, ref Farmer who, ref bool __result, GameLocation l)
        {
            NpcConfig npcConfig = DataLoader.ModConfig.NpcConfigs.Find(n => n.Name == __instance.Name);

            if (npcConfig != null && __result != true && !__instance.isMarried() && CanKissNpc(who, __instance) && who.IsLocalPlayer)
            {
                int timeOfDay = Game1.timeOfDay;
                    
                if (__instance.Sprite.CurrentAnimation == null && !__instance.hasTemporaryMessageAvailable() &&
                    (__instance.CurrentDialogue.Count == 0 && Game1.timeOfDay < 2200) &&
                    (!__instance.isMoving() && who.ActiveObject == null))
                {
                    __instance.faceDirection(-3);
                    __instance.faceGeneralDirection(who.getStandingPosition(), 0, false);
                    who.faceGeneralDirection(__instance.getStandingPosition(), 0, false);
                    if (__instance.FacingDirection == 3 || __instance.FacingDirection == 1)
                    {
                        int frame = npcConfig.Frame;
                        bool flag = npcConfig.FrameDirectionRight;

                        bool flip = flag && __instance.FacingDirection == 3 || !flag && __instance.FacingDirection == 1;
                        if (HasRequiredFriendshipToKiss(who, __instance))
                        {
                            __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                            {
                                new FarmerSprite.AnimationFrame(frame, Game1.IsMultiplayer ? 1010 : 10, false, flip,
                                    new AnimatedSprite.endOfAnimationBehavior(__instance.haltMe), true)
                            });
                            if (!__instance.hasBeenKissedToday.Value)
                            {
                                who.changeFriendship(DataLoader.ModConfig.KissingFriendshipPoints, __instance);
                                DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1]
                                {
                                    new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                                        new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0,
                                        new Vector2((float) __instance.getTileX(), (float) __instance.getTileY()) *
                                        64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f,
                                        0.0f, 0.0f, false)
                                    {
                                        motion = new Vector2(0.0f, -0.5f),
                                        alphaFade = 0.01f
                                    }
                                });
                                l.playSound("dwop");
                                if (!DataLoader.ModConfig.DisableExhaustionReset)
                                {
                                    who.exhausted.Value = false;
                                }
                                if (!DataLoader.ModConfig.DisableJealousy)
                                {
                                    if (who.spouse != null && !who.spouse.Contains(__instance.Name))
                                    {
                                        NPC characterFromName = Game1.getCharacterFromName(who.spouse, false);
                                        if (l.characters.Contains(characterFromName) || !(Game1.random.NextDouble() >= 0.3 - (double) who.LuckLevel / 100.0 - who.DailyLuck))
                                        {
                                            who.changeFriendship(DataLoader.ModConfig.JealousyFriendshipPoints, characterFromName);
                                            characterFromName.CurrentDialogue.Clear();
                                            characterFromName.CurrentDialogue.Push(new StardewValley.Dialogue(DataLoader.Helper.Translation.Get("Jealousy.Dialog", new { npcName = (object)__instance.displayName }), characterFromName));
                                        }
                                    }
                                }
                            }

                            __instance.hasBeenKissedToday.Value = true;
                            __instance.Sprite.UpdateSourceRect();
                        }
                        else
                        {
                            __instance.faceDirection(Game1.random.NextDouble() < 0.5 ? 2 : 0);
                            __instance.doEmote(12, true);
                        }

                        who.CanMove = false;
                        who.FarmerSprite.PauseForSingleAnimation = false;
                        if (flag && !flip || !flag & flip)
                            who.faceDirection(3);
                        else
                            who.faceDirection(1);
                        who.FarmerSprite.animateOnce(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(101, 1000, 0, false, who.FacingDirection == 3,
                                (AnimatedSprite.endOfAnimationBehavior) null, false, 0),
                            new FarmerSprite.AnimationFrame(6, 1, false, who.FacingDirection == 3,
                                new AnimatedSprite.endOfAnimationBehavior(Farmer.completelyStopAnimating), false)
                        }.ToArray());
                        __result = true;
                    }
                }
            }
            else if (__instance.isMarried())
            {
                NpcConfig defaultNpcConfig = DataLoader.DefaultConfig.NpcConfigs.Find(n => n.Name == __instance.Name);
                if (defaultNpcConfig != null && npcConfig != null && defaultNpcConfig.Frame != npcConfig.Frame)
                {
                    if (__instance.Sprite.CurrentFrame == defaultNpcConfig.Frame)
                    {
                        int frame = npcConfig.Frame;
                        bool flag = npcConfig.FrameDirectionRight;

                        bool flip = flag && __instance.FacingDirection == 3 || !flag && __instance.FacingDirection == 1;
                        __instance.Sprite.currentFrame = frame;
                        var oldAnimationFrame = __instance.Sprite.CurrentAnimation[__instance.Sprite.currentAnimationIndex];
                        __instance.Sprite.CurrentAnimation[__instance.Sprite.currentAnimationIndex] =
                            new FarmerSprite.AnimationFrame(frame, 0, oldAnimationFrame.milliseconds, oldAnimationFrame.secondaryArm,
                                flip, oldAnimationFrame.frameStartBehavior, oldAnimationFrame.frameEndBehavior, 0);
                        __instance.Sprite.UpdateSourceRect();
                    }
                }
            }
        }

        internal static bool CanKissNpc(Farmer who, NPC npc)
        {
            NpcConfig npcConfig = DataLoader.ModConfig.NpcConfigs.Find(n => n.Name == npc.Name);
            return npcConfig != null
                   && (who.friendshipData[npc.Name].IsDating() 
                       || DataLoader.ModConfig.DisableDatingRequirement)
                   && (npcConfig.RequiredEvent == null 
                       || who.eventsSeen.Contains(npcConfig.RequiredEvent.Value) 
                       || DataLoader.ModConfig.DisableEventRequirement);
        }

        internal static bool HasRequiredFriendshipToKiss(Farmer who, NPC npc)
        {
            return who.getFriendshipHeartLevelForNPC(npc.Name) >= DataLoader.ModConfig.RequiredFriendshipLevel;
        }
    }
}
