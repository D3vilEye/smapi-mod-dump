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
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Entoarox.DynamicDungeons
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        private ActionInfo ActionInfo;
        private BookMenu InfoBook;


        /*********
        ** Accessors
        *********/
        internal static IMonitor SMonitor;
        internal static IModHelper SHelper;
        internal static DynamicDungeon Location;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.SMonitor = this.Monitor;
            ModEntry.SHelper = this.Helper;
            helper.Events.GameLoop.UpdateTicked+= this.GameEvents_UpdateTick;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.ConsoleCommands.Add("dd_fromseed", "dd_fromseed <seed> | Generate a dungeon from a specific seed", this.Command_Fromseed);
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var helper = this.Helper;
            this.InfoBook = new BookMenu(new List<Page>
            {
                new TitlePage(helper.Translation.Get("Book_Title"), helper.Translation.Get("Book_Subtitle"), helper.Translation.Get("Book_Introduction")),
                new TextPage(helper.Translation.Get("Book_Page1")),
                new TextPage(helper.Translation.Get("Book_Page2")),
                new PaymentPage(),
                new ImagePage(helper.Content.Load<Texture2D>("assets/book/doodle1.png"), Game1.textColor, true),
                new TitlePage("DickBut", "Dickius Buttius Maximus", "The Dickius Buttius Maximus, better know as the DickBut is a mighty creature of great majesty."),
                new ImagePage(helper.Content.Load<Texture2D>("assets/book/doodle2.png"), Game1.textColor, true),
                new TitlePage("The Lie", "Absolutum Lie-um", "Please turn the page somewhere else, there is nothing to see here."),
                new ImagePage(helper.Content.Load<Texture2D>("assets/book/doodle3.png"), Game1.textColor, true),
                new TitlePage("PufferChick", "Pufferium Chickate", "Adorable abomination, dont you just want to cuddle it?")
            });
        }

        public override object GetApi()
        {
            return new DynamicDungeonsAPI();
        }

        public static void GenerateDungeon(double difficulty, int? seed = null)
        {
            if (ModEntry.Location != null)
                Game1.locations.Remove(ModEntry.Location);
            Stopwatch watch = new Stopwatch();
            ModEntry.SMonitor.Log("Generating dungeon...", LogLevel.Alert);
            watch.Start();
            ModEntry.Location = new DynamicDungeon(difficulty, seed);
            watch.Stop();
            ModEntry.SMonitor.Log("Generation completed in [" + watch.ElapsedMilliseconds + "] miliseconds", LogLevel.Alert);
            Game1.locations.Add(ModEntry.Location);
        }

        public void ResolveAction()
        {
            string action = this.ActionInfo?.Action ?? string.Empty;
            if (action.Equals("DDEntrance"))
            {
                ModEntry.GenerateDungeon(10);
                Game1.warpFarmer("DynamicDungeon", ModEntry.Location.EntryPoint.X, ModEntry.Location.EntryPoint.Y, true);
            }
            else if (action.Equals("DDBook"))
            {
                Game1.playSound("shwip");
                Game1.activeClickableMenu = this.InfoBook;
            }
            else if (action.Equals("DDDoor"))
            {
                if (Game1.player.hasSkullKey)
                {
                    Warp warp = Game1.getLocationFromName("DynamicDungeonEntrance").warps[0];
                    Game1.warpFarmer("DynamicDungeonEntrance", warp.X, warp.Y - 1, false);
                }
                else
                    Game1.drawObjectDialogue(ModEntry.SHelper.Translation.Get("SkullKeyNeeded"));
            }
        }


        /*********
        ** Protected methods
        *********/
        private void CheckForAction()
        {
            if (Game1.activeClickableMenu == null && !Game1.player.UsingTool && !Game1.pickingTool && !Game1.menuUp && (!Game1.eventUp || Game1.currentLocation.currentEvent.playerControlSequence) && !Game1.nameSelectUp && Game1.numberOfSelectedItems == -1 && !Game1.fadeToBlack)
            {
                this.ActionInfo = null;
                Vector2 grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                    grabTile = Game1.player.GetGrabTile();
                xTile.Tiles.Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size) ?? Game1.currentLocation.map.GetLayer("Buildings").PickTile(new Location((int)grabTile.X * Game1.tileSize, (int)(grabTile.Y + 1) * Game1.tileSize), Game1.viewport.Size);
                if (tile != null && tile.Properties.TryGetValue("Action", out PropertyValue propertyValue) && propertyValue != null)
                {
                    string[] split = ((string)propertyValue).Split(' ');
                    string[] args = new string[split.Length - 1];
                    Array.Copy(split, 1, args, 0, args.Length);
                    this.ActionInfo = new ActionInfo(Game1.player, split[0], args, grabTile);
                }
            }
        }

        private void Command_Fromseed(string command, string[] arguments)
        {
            try
            {
                ModEntry.GenerateDungeon(Convert.ToInt32(arguments[0], 16));
            }
            catch
            {
                this.Monitor.Log("Input is not a valid seed!", LogLevel.Error);
            }
        }

        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (Context.IsWorldReady)
            {
                this.Helper.Events.GameLoop.UpdateTicked -= this.GameEvents_UpdateTick;
                this.Helper.Events.Input.ButtonReleased += this.InputEvents_ButtonReleased;
                this.Helper.Events.Player.Warped += this.PlayerEvents_Warped;
                GameLocation loc = Game1.getLocationFromName("WizardHouse");
                TileSheet sheet = new TileSheet("Custom", loc.map, this.Helper.Content.GetActualAssetKey("assets/door.png"), new Size(4, 7), new Size(16, 16));
                loc.map.AddTileSheet(sheet);
                /*
                loc.setMapTileIndex(5, 13, 112, "Back");
                (5, 13, "Buildings", 1, "Custom").ApplyTo(loc.map);
                (5, 12, "Front", 0, "Custom").ApplyTo(loc.map);
                */
                new Tiles.StaticTile(4, 11, "Front", 0, "Custom").Apply(loc.map);
                new Tiles.StaticTile(4, 11, "Front", 0, "Custom").Apply(loc.map);
                new Tiles.StaticTile(5, 11, "Front", 1, "Custom").Apply(loc.map);
                new Tiles.StaticTile(6, 11, "Front", 2, "Custom").Apply(loc.map);

                new Tiles.StaticTile(4, 12, "Buildings", 4, "Custom").Apply(loc.map);
                new Tiles.StaticTile(6, 12, "Buildings", 6, "Custom").Apply(loc.map);

                new Tiles.StaticTile(4, 13, "Buildings", 8, "Custom").Apply(loc.map);
                new Tiles.StaticTile(6, 13, "Buildings", 10, "Custom").Apply(loc.map);

                new Tiles.AnimatedTile(5, 12, "Buildings", new[] { 12, 13, 14, 15, 20, 21, 22, 23 }, "Custom", 250).Apply(loc.map);
                new Tiles.AnimatedTile(5, 13, "Buildings", new[] { 16, 17, 18, 19, 24, 25, 26, 27 }, "Custom", 250).Apply(loc.map);

                new Tiles.PropertyTile(5, 13, "Buildings", "Action", "DDDoor").Apply(loc.map);
                if (Game1.getLocationFromName("DynamicDungeonEntrance") == null)
                {
                    this.Helper.Content.Load<Map>("assets/DynamicDungeonsEntrance.tbin"); // maps need to be loaded before the game can reference it
                    Game1.locations.Add(new GameLocation(this.Helper.Content.GetActualAssetKey("assets/DynamicDungeonsEntrance.tbin"), "DynamicDungeonEntrance"));
                }
            }
        }

        private void PlayerEvents_Warped(object s, WarpedEventArgs e)
        {
            if (e.OldLocation != null && (e.OldLocation.Name == "DynamicDungeonEntrance" || e.OldLocation.Name == "WizardHouse"))
            {
                this.Helper.Events.Input.ButtonPressed -= this.ControlEvents_ControllerButtonPressed;
                this.Helper.Events.Input.ButtonReleased -= this.ControlEvents_ControllerButtonReleased;
                if (e.OldLocation.Name == "DynamicDungeonEntrance")
                    this.Helper.Events.Display.RenderingHud-= this.GraphicsEvents_OnPreRenderHudEvent;
            }

            if (e.NewLocation.Name == "DynamicDungeonEntrance" || e.NewLocation.Name == "WizardHouse")
            {
                this.Helper.Events.Input.ButtonPressed += this.ControlEvents_ControllerButtonPressed;
                this.Helper.Events.Input.ButtonReleased += this.ControlEvents_ControllerButtonReleased;
                if (e.NewLocation.Name == "DynamicDungeonEntrance")
                    this.Helper.Events.Display.RenderingHud += this.GraphicsEvents_OnPreRenderHudEvent;
            }
        }

        private void GraphicsEvents_OnPreRenderHudEvent(object s, EventArgs e)
        {
            void Glow(float x, float y)
            {
                Game1.spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize + Game1.tileSize / 2 + Game1.pixelZoom / 2, y * Game1.tileSize + Game1.tileSize / 4)), new Rectangle(88, 1779, 30, 30), Color.PaleGoldenrod * (Game1.currentLocation.IsOutdoors ? 0.35f : 0.43f), 0f, new Vector2(15f, 15f), Game1.pixelZoom + (float)(Game1.tileSize * Math.Sin((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + x * Game1.tileSize * 777 + y * Game1.tileSize * 9746) % 3140.0 / 1000.0) / 50.0), SpriteEffects.None, 1f);
            }

            Glow(1.30f, 4.50f);
            Glow(5.00f, 2.50f);
            Glow(8.70f, 4.50f);
            Glow(2.25f, 8.50f);
            Glow(7.75f, 8.50f);
        }

        private void InputEvents_ButtonReleased(object s, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady || e.Button != SButton.F5 && e.Button != SButton.F6 && e.Button != SButton.F7)
                return;
            if (e.Button == SButton.F5)
                Game1.warpFarmer("WizardHouse", 5, 14, false);
        }

        private void ControlEvents_ControllerButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.IsActionButton())
                this.CheckForAction();
        }

        private void ControlEvents_ControllerButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (this.ActionInfo != null && e.Button.IsActionButton())
            {
                this.ResolveAction();
                this.ActionInfo = null;
            }
        }
    }
}
