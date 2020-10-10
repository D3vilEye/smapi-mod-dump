/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cdaragorn/Ui-Info-Suite
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace UIInfoSuite.UIElements
{
    class SkipIntro
    {
        private readonly IModEvents _events;
        //private bool _skipIntro = false;

        public SkipIntro(IModEvents events)
        {
            _events = events;

            //GameEvents.QuarterSecondTick += CheckForSkip;
            events.Input.ButtonPressed += OnButtonPressed;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
            //MenuEvents.MenuChanged += SkipToTitleButtons;
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, EventArgs e)
        {
            // stop checking for skip key
            _events.Input.ButtonPressed -= OnButtonPressed;
            _events.GameLoop.SaveLoaded -= OnSaveLoaded;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Game1.activeClickableMenu is TitleMenu menu && e.Button == SButton.Escape)
            {
                menu.skipToTitleButtons();
                _events.Input.ButtonPressed -= OnButtonPressed;
            }
        }

        //private void CheckForSkip(object sender, EventArgs e)
        //{
        //    if (Game1.activeClickableMenu is TitleMenu &&
        //        _skipIntro)
        //    {
        //        _skipIntro = false;
        //        (Game1.activeClickableMenu as TitleMenu)?.skipToTitleButtons();
        //    }
        //}

        //private void SkipToTitleButtons(object sender, EventArgsClickableMenuChanged e)
        //{
        //    TitleMenu menu = e.NewMenu as TitleMenu;
        //    menu?.skipToTitleButtons();
        //    //MenuEvents.MenuChanged -= SkipToTitleButtons;
        //}
    }
}
