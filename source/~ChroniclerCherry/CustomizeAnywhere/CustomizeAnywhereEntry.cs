/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;

namespace CustomizeAnywhere
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        internal static IMonitor monitor;
        internal static IModHelper helper;

        public override void Entry(IModHelper h)
        {
            helper = h;
            monitor = Monitor;
            if (helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                new DresserAndMirror(helper);
            } else
            {
                monitor.Log("JSon Assets is not installed. Customization Mirror and Clothing Catalogue will not be added.", LogLevel.Info);
            }
            
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if ((Game1.activeClickableMenu is CustomizeAnywhereMenu) && (e.Button == SButton.Escape))
            {
                resetAppearance();
                Game1.exitActiveMenu();
                helper.Input.Suppress(e.Button);
            }

            // ignore if player isn't free to move or direct access is turned off in the config
            if (!Context.CanPlayerMove || !Config.canAccessMenusAnywhere)
                return;

            var input = this.Helper.Input;
            if (input.IsDown(this.Config.ActivateButton)) {
                if (input.IsDown(this.Config.customizeButton))
                {
                    Game1.activeClickableMenu = new CustomizeAnywhereMenu();
                } else if (input.IsDown(this.Config.dresserButton))
                {
                    Game1.activeClickableMenu = new DresserMenu();
                }

                    if (!Game1.player.eventsSeen.Contains(992559) && !this.Config.canTailorWithoutEvent)
                {
                    return;
                }

                if (input.IsDown(this.Config.dyeButton))
                {
                    Game1.activeClickableMenu = new DyeMenu();
                }
                else if (input.IsDown(this.Config.tailoringButton))
                {
                    Game1.activeClickableMenu = new TailoringMenu();
                }

            }

        }//end buttonPressed event

        private static int shirt;
        private static Clothing shirtItem;
        private static Color shirtColour;
        private static int pants;
        private static Clothing pantsItem;
        private static Color pantsColour;
        private static int hair;
        private static int accessory;
        private static Color hairColour;
        private static Color eyeColour;
        private static NetInt skinColour;
        private static bool isMale;

        internal static void saveCurrentAppearance()
        {
            shirt = Game1.player.GetShirtIndex();
            shirtItem = Game1.player.shirtItem.Get();
            pants = Game1.player.GetPantsIndex();
            pantsItem = Game1.player.pantsItem.Get();
            pantsColour = Game1.player.GetPantsColor();
            hair = Game1.player.getHair();
            accessory = Game1.player.accessory.Get();
            hairColour = Game1.player.hairstyleColor.Get();
            eyeColour = helper.Reflection.GetField<NetColor>(Game1.player.FarmerRenderer, "eyes").GetValue().Get();
            skinColour = Game1.player.skin;
            isMale = Game1.player.IsMale;
        }

        private static void resetAppearance()
        {
            Game1.player.changeShirt(shirt);
            Game1.player.shirtItem.Set(shirtItem);
            Game1.player.changePantStyle(pants);
            Game1.player.pantsItem.Set(pantsItem);
            Game1.player.changePants(pantsColour);
            Game1.player.changeHairStyle(hair);
            Game1.player.changeHairColor(hairColour);
            Game1.player.changeEyeColor(eyeColour);
            Game1.player.changeSkinColor(skinColour);
            Game1.player.accessory.Set(accessory);
            Game1.player.changeGender(isMale);
            Game1.player.UpdateClothing();
            Game1.player.ConvertClothingOverrideToClothesItems();
        }
    }//end ModEntry

    class ModConfig
    {
        public bool canAccessMenusAnywhere { get; set; }
        public SButton ActivateButton { get; set; }
        public SButton customizeButton { get; set; }
        public SButton dyeButton { get; set; }
        public SButton tailoringButton { get; set; }
        public SButton dresserButton { get; set; }
        public bool canTailorWithoutEvent { get; set; }

        public ModConfig()
        {
            canAccessMenusAnywhere = true;
            ActivateButton = SButton.LeftShift;
            customizeButton = SButton.D1;
            dyeButton = SButton.D2;
            tailoringButton = SButton.D3;
            dresserButton = SButton.D4;
            canTailorWithoutEvent = false;
        }
    }
    
}