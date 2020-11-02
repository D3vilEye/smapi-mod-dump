/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.Common.UI
{
    /// <summary>A dropdown UI component which lets the player choose from a list of values.</summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    internal class DropdownList<TItem> : ClickableComponent
    {
        /*********
        ** Fields
        *********/
        /****
        ** Constants
        ****/
        /// <summary>The padding applied to dropdown lists.</summary>
        private const int DropdownPadding = 5;

        /****
        ** Items
        ****/
        /// <summary>The selected entry.</summary>
        private DropListItem Selected;

        /// <summary>The items in the list.</summary>
        private readonly DropListItem[] Items;

        /// <summary>The clickable components representing the list items.</summary>
        private readonly List<ClickableComponent> ItemComponents = new List<ClickableComponent>();

        /// <summary>The item index shown at the top of the list.</summary>
        private int FirstVisibleIndex;

        /// <summary>The maximum items to display.</summary>
        private int MaxItems;

        /// <summary>The maximum index for the first item.</summary>
        private int MaxFirstVisibleIndex;

        /// <summary>Get the display name for a value.</summary>
        private readonly Func<TItem, string> GetLabel;


        /****
        ** Rendering
        ****/
        /// <summary>The font with which to render text.</summary>
        private readonly SpriteFont Font;


        /*********
        ** Accessors
        *********/
        /// <summary>The selected item.</summary>
        public TItem SelectedItem => this.Selected.Value;

        /// <summary>The display label for the selected item.</summary>
        public string SelectedLabel => this.GetLabel(this.SelectedItem);

        /// <summary>The maximum height for the possible labels.</summary>
        public int MaxLabelHeight { get; }

        /// <summary>The maximum width for the possible labels.</summary>
        public int MaxLabelWidth { get; private set; }

        /// <summary>The <see cref="ClickableComponent.myID"/> value for the top entry in the dropdown.</summary>
        public int TopComponentId => this.ItemComponents[0].myID;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="selectedItem">The selected item.</param>
        /// <param name="items">The items in the list.</param>
        /// <param name="getLabel">Get the display label for an item.</param>
        /// <param name="x">The X-position from which to render the list.</param>
        /// <param name="y">The Y-position from which to render the list.</param>
        /// <param name="font">The font with which to render text.</param>
        public DropdownList(TItem selectedItem, TItem[] items, Func<TItem, string> getLabel, int x, int y, SpriteFont font)
            : base(new Rectangle(), nameof(DropdownList<TItem>))
        {
            // save values
            this.Items = items
                .Select((item, index) => new DropListItem(index, getLabel(item), item))
                .ToArray();
            this.Font = font;
            this.MaxLabelHeight = (int)font.MeasureString("abcdefghijklmnopqrstuvwxyz").Y;
            this.GetLabel = getLabel;

            // set initial selection
            int selectedIndex = Array.IndexOf(items, selectedItem);
            this.Selected = selectedIndex >= 0
                ? this.Items[selectedIndex]
                : this.Items.First();

            // initialize UI
            this.bounds.X = x;
            this.bounds.Y = y;
            this.ReinitializeComponents();
        }

        /// <summary>A method invoked when the player scrolls the dropdown using the mouse wheel.</summary>
        /// <param name="direction">The scroll direction.</param>
        public void ReceiveScrollWheelAction(int direction)
        {
            this.Scroll(direction > 0 ? -1 : 1); // scrolling down moves first item up
        }

        /// <summary>Select an item in the list if it's under the cursor.</summary>
        /// <param name="x">The X-position of the item in the UI.</param>
        /// <param name="y">The Y-position of the item in the UI.</param>
        /// <param name="selected">The selected item, if found.</param>
        /// <returns>Returns whether an item was selected.</returns>
        public bool TrySelect(int x, int y, out TItem selected)
        {
            for (int i = 0; i < this.ItemComponents.Count; i++)
            {
                var component = this.ItemComponents[i];
                if (component.containsPoint(x, y))
                {
                    this.Selected = this.Items[i];
                    selected = this.SelectedItem;
                    return true;
                }
            }

            selected = default;
            return false;
        }

        /// <summary>Select an item in the list matching the given value.</summary>
        /// <param name="value">The value to search.</param>
        /// <returns>Returns whether an item was selected.</returns>
        public bool TrySelect(TItem value)
        {
            var entry = this.Items.FirstOrDefault(p =>
                (p.Value == null && value == null)
                || p.Value?.Equals(value) == true
            );

            if (entry == null)
                return false;

            this.Selected = entry;
            return true;
        }

        /// <summary>Render the UI.</summary>
        /// <param name="sprites">The sprites to render.</param>
        /// <param name="opacity">The opacity at which to draw.</param>
        public void Draw(SpriteBatch sprites, float opacity = 1)
        {
            // draw dropdown items
            for (int i = 0; i < this.ItemComponents.Count; i++)
            {
                var component = this.ItemComponents[i];

                // draw background
                if (component.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                    sprites.Draw(CommonSprites.DropDown.Sheet, component.bounds, CommonSprites.DropDown.HoverBackground, Color.White * opacity);
                else if (i == this.Selected.Index)
                    sprites.Draw(CommonSprites.DropDown.Sheet, component.bounds, CommonSprites.DropDown.ActiveBackground, Color.White * opacity);
                else
                    sprites.Draw(CommonSprites.DropDown.Sheet, component.bounds, CommonSprites.DropDown.InactiveBackground, Color.White * opacity);

                // draw text
                DropListItem item = this.Items.First(p => p.Index == int.Parse(component.name));
                Vector2 position = new Vector2(component.bounds.X + DropdownList<TItem>.DropdownPadding, component.bounds.Y + Game1.tileSize / 16);
                sprites.DrawString(this.Font, item.Name, position, Color.Black * opacity);
            }

            // draw up/down arrows
            if (this.FirstVisibleIndex > 0)
                sprites.Draw(CommonSprites.Icons.Sheet, new Vector2(this.bounds.X - CommonSprites.Icons.UpArrow.Width, this.bounds.Y), CommonSprites.Icons.UpArrow, Color.White * opacity);
            if (this.FirstVisibleIndex < this.MaxFirstVisibleIndex)
                sprites.Draw(CommonSprites.Icons.Sheet, new Vector2(this.bounds.X - CommonSprites.Icons.UpArrow.Width, this.bounds.Y + this.bounds.Height - CommonSprites.Icons.DownArrow.Height), CommonSprites.Icons.DownArrow, Color.White * opacity);
        }

        /// <summary>Recalculate dimensions and components for rendering.</summary>
        public void ReinitializeComponents()
        {
            // get item size
            int minItemWidth = Game1.tileSize * 2;
            this.MaxLabelWidth = Math.Max((int)this.Items.Max(p => this.Font.MeasureString(p.Name).X), minItemWidth) + DropdownList<TItem>.DropdownPadding * 2;
            int itemHeight = this.MaxLabelHeight;

            // get pagination
            int itemCount = this.Items.Length;
            this.MaxItems = Math.Min((Game1.viewport.Height - (int)this.bounds.Y) / itemHeight, itemCount);
            this.MaxFirstVisibleIndex = this.Items.Length - this.MaxItems;
            this.FirstVisibleIndex = this.GetValidFirstItem(this.FirstVisibleIndex, this.MaxFirstVisibleIndex);

            // get dropdown size
            this.bounds.Width = this.MaxLabelWidth;
            this.bounds.Height = itemHeight * this.MaxItems;

            // generate components
            this.ItemComponents.Clear();
            int x = this.bounds.X;
            int y = this.bounds.Y;
            for (int i = this.FirstVisibleIndex; i < this.MaxItems; i++)
            {
                this.ItemComponents.Add(new ClickableComponent(new Rectangle(x, y, this.MaxLabelWidth, itemHeight), i.ToString()));
                y += this.MaxLabelHeight;
            }

            // update controller flow
            this.ReinitializeControllerFlow();
        }

        /// <summary>Set the fields to support controller snapping.</summary>
        public void ReinitializeControllerFlow()
        {
            int initialId = 1_100_000;
            for (int last = this.ItemComponents.Count - 1, i = last; i >= 0; i--)
            {
                var component = this.ItemComponents[i];

                component.myID = initialId + i;
                component.upNeighborID = i != 0
                    ? initialId + i - 1
                    : -99999;
                component.downNeighborID = i != last
                    ? initialId + i + 1
                    : -1;
            }
        }

        /// <summary>Get the nested components for controller snapping.</summary>
        public IEnumerable<ClickableComponent> GetChildComponents()
        {
            return this.ItemComponents;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Scroll the dropdown by the specified amount.</summary>
        /// <param name="amount">The number of items to scroll.</param>
        private void Scroll(int amount)
        {
            // recalculate first item
            int firstItem = this.GetValidFirstItem(this.FirstVisibleIndex + amount, this.MaxFirstVisibleIndex);
            if (firstItem == this.FirstVisibleIndex)
                return;
            this.FirstVisibleIndex = firstItem;

            // update displayed items
            int itemIndex = firstItem;
            foreach (ClickableComponent current in this.ItemComponents)
            {
                current.name = itemIndex.ToString();
                itemIndex++;
            }
        }

        /// <summary>Calculate a valid index for the first displayed item in the list.</summary>
        /// <param name="value">The initial value, which may not be valid.</param>
        /// <param name="maxIndex">The maximum first index.</param>
        private int GetValidFirstItem(int value, int maxIndex)
        {
            return Math.Max(Math.Min(value, maxIndex), 0);
        }


        /*********
        ** Private models
        *********/
        /// <summary>An item in a drop list.</summary>
        private class DropListItem
        {
            /*********
            ** Accessors
            *********/
            /// <summary>The item's index in the list.</summary>
            public int Index { get; }

            /// <summary>The display name.</summary>
            public string Name { get; }

            /// <summary>The item value.</summary>
            public TItem Value { get; }


            /*********
            ** Public methods
            *********/
            /// <summary>Construct an instance.</summary>
            /// <param name="index">The item's index in the list.</param>
            /// <param name="name">The display name.</param>
            /// <param name="value">The item value.</param>
            public DropListItem(int index, string name, TItem value)
            {
                this.Index = index;
                this.Name = name;
                this.Value = value;
            }
        }
    }
}
