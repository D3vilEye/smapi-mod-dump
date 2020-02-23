﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace LimitedCampfireCooking
{
    class ModEntry : Mod
    {
        internal static ModConfig Config;
        internal static IMonitor monitor;
        public override void Entry(IModHelper helper)
        {
            monitor = Monitor;
            Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady &&
                Game1.currentLocation != null &&
                Game1.activeClickableMenu == null &&
                e.Button.IsUseToolButton())
            {
                GameLocation loc = Game1.currentLocation;

                Vector2 tile = Helper.Input.GetCursorPosition().GrabTile;
                loc.Objects.TryGetValue(tile, out StardewValley.Object obj);

                if (obj != null && obj.Name == "Campfire" && obj.IsOn)
                {
                    Helper.Input.Suppress(e.Button);
                    Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, 0, 0);
                    Game1.activeClickableMenu = new CookingMenu((int)centeringOnScreen.X, (int)centeringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
                }
            }
        }
    }

    class ModConfig
    {
        public bool EnableAllCookingRecipies { get; set; } = false;
        public string[] Recipes { get; set; } = {
                    "Fried Egg",
                    "Baked Fish",
                    "Parsnip Soup",
                    "Vegetable Stew",
                    "Bean Hotpot",
                    "Glazed Yams",
                    "Carp Surprise",
                    "Fish Taco",
                    "Tom Kha Soup",
                    "Trout Soup",
                    "Pumpkin Soup",
                    "Algae Soup",
                    "Pale Broth",
                    "Roasted Hazelnuts",
                    "Chowder",
                    "Lobster Bisque",
                    "Fish Stew"
                };
    }
}
