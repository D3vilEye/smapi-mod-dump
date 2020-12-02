/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace ItemBags
{
    public static class CraftingHandler
    {
        private static IModHelper Helper { get; set; }

        /// <summary>Adds functionality that allows you to use items within your bags as input materials for crafting from the main GameMenu's CraftingPage, or from a cooking CraftingPage.</summary>
        internal static void OnModEntry(IModHelper Helper)
        {
            CraftingHandler.Helper = Helper;

            Helper.Events.Display.MenuChanged += Display_MenuChanged;
            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

            //  Possible TODO: Add support for the Cooking menu used by the "Cooking Skill" mod (https://www.nexusmods.com/stardewvalley/mods/522)
            //  That mod does NOT use a "CraftingPage" as its menu, so it currently isn't detected by Display_MenuChanged event handler.
            //  I haven't tested it yet, but it might not have a "List<Chest> _materialContainers" field for me to add bag contents to, so might not be feasible.
        }

        private static HashSet<ItemBag> BagsInUse = null;
        internal static bool IsUsingForCrafting(ItemBag Bag) { return BagsInUse != null && BagsInUse.Contains(Bag); }
        private static Dictionary<Object, List<Object>> SplitStacks = null;

        /// <summary>Initializes extra data for the Crafting Page so it can search for and use materials within bags in your inventory.</summary>
        private static void OnCraftingPageActivated(CraftingPage CraftingMenu)
        {
            //  Allow the CraftingPage to search for and use items inside of bags
            bool AllowUsingBundleBagItemsForCrafting = false;
            List<ItemBag> BagsInInventory = Game1.player.Items.Where(x => x != null && x is ItemBag).Cast<ItemBag>().ToList();
            if (BagsInInventory.Any())
            {
                BagsInUse = new HashSet<ItemBag>(BagsInInventory);

                //  Get the "_materialContainers" protected field that defines additional item containers to search for when using up materials during crafting
                IReflectedField<List<Chest>> ReflectionResult = Helper.Reflection.GetField<List<Chest>>(CraftingMenu, "_materialContainers", true);
                List<Chest> MaterialContainers = ReflectionResult.GetValue();
                if (MaterialContainers == null)
                {
                    MaterialContainers = new List<Chest>();
                    ReflectionResult.SetValue(MaterialContainers);
                }

                //  Create a temporary chest from the items of each bag, and add the chest to _materialContainers
                foreach (ItemBag IB in BagsInInventory.Where(x => AllowUsingBundleBagItemsForCrafting || !(x is BundleBag)))
                {
                    //  Note that if the item inside the bag has Stack > 999, it must be split up into chunks with Stacks <= 999
                    //  Because the Game truncates the actual stack down to 999 anytime it modifies a stack value
                    if (IB is OmniBag OB)
                    {
                        foreach (ItemBag NestedBag in OB.NestedBags.Where(x => AllowUsingBundleBagItemsForCrafting || !(x is BundleBag)))
                        {
                            List<Item> TemporaryChestContents = new List<Item>();

                            foreach (Object Item in NestedBag.Contents)
                            {
                                if (Item.Stack > 999)
                                {
                                    if (SplitStacks == null)
                                        SplitStacks = new Dictionary<Object, List<Object>>();

                                    List<Object> Chunks = new List<Object>();

                                    int TotalStack = Item.Stack;
                                    int DistributedAmt = 0;
                                    while (DistributedAmt < TotalStack)
                                    {
                                        int CurrentStackAmt = Math.Min(999, TotalStack - DistributedAmt);
                                        Object Chunk = ItemBag.CreateCopy(Item);
                                        ItemBag.ForceSetQuantity(Chunk, CurrentStackAmt);
                                        DistributedAmt += CurrentStackAmt;
                                        Chunks.Add(Chunk);
                                    }

                                    SplitStacks.Add(Item, Chunks);
                                    TemporaryChestContents.AddRange(Chunks);
                                }
                                else
                                {
                                    TemporaryChestContents.Add(Item);
                                }
                            }

                            Chest TempChest = new Chest(0, TemporaryChestContents, Vector2.Zero, false, 0);
                            MaterialContainers.Add(TempChest);
                        }
                    }
                    else
                    {
                        List<Item> TemporaryChestContents = new List<Item>();

                        foreach (Object Item in IB.Contents)
                        {
                            if (Item.Stack > 999)
                            {
                                if (SplitStacks == null)
                                    SplitStacks = new Dictionary<Object, List<Object>>();

                                List<Object> Chunks = new List<Object>();

                                int TotalStack = Item.Stack;
                                int DistributedAmt = 0;
                                while (DistributedAmt < TotalStack)
                                {
                                    int CurrentStackAmt = Math.Min(999, TotalStack - DistributedAmt);
                                    Object Chunk = ItemBag.CreateCopy(Item);
                                    ItemBag.ForceSetQuantity(Chunk, CurrentStackAmt);
                                    DistributedAmt += CurrentStackAmt;
                                    Chunks.Add(Chunk);
                                }

                                SplitStacks.Add(Item, Chunks);
                                TemporaryChestContents.AddRange(Chunks);
                            }
                            else
                            {
                                TemporaryChestContents.Add(Item);
                            }
                        }

                        Chest TempChest = new Chest(0, TemporaryChestContents, Vector2.Zero, false, 0);
                        MaterialContainers.Add(TempChest);
                    }
                }
            }
        }

        private static void OnCraftingPageDeactivated()
        {
            try
            {
                //  If the player had the CraftingPage of the GameMenu opened, they may have crafted items using materials that were inside their bags,
                //  so we must Resynchronize the bag contents in multiplayer
                if (BagsInUse != null && BagsInUse.Any())
                {
                    //  Recombine item Stacks that were split to avoid having > 999
                    if (SplitStacks != null && SplitStacks.Any())
                    {
                        foreach (KeyValuePair<Object, List<Object>> KVP in SplitStacks)
                        {
                            int NewQuantity = KVP.Value.Sum(x => x.Stack);
                            ItemBag.ForceSetQuantity(KVP.Key, NewQuantity);
                        }
                    }

                    foreach (ItemBag Bag in BagsInUse)
                    {
                        if (Bag is OmniBag OB)
                        {
                            foreach (ItemBag NestedBag in OB.NestedBags)
                                NestedBag.Contents.RemoveAll(x => x == null || x.Stack <= 0);
                        }
                        else
                        {
                            Bag.Contents.RemoveAll(x => x == null || x.Stack <= 0);
                        }

                        Bag.Resync();
                    }
                }
            }
            finally
            {
                BagsInUse = null;
                SplitStacks = null;
            }
        }

        private static void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (IsViewingGameMenu && Game1.activeClickableMenu is GameMenu GM)
            {
                int CurrentTab = GM.currentTab;
                if (!PreviousGameMenuTab.HasValue || PreviousGameMenuTab.Value != CurrentTab)
                {
                    OnGameMenuTabChanged(PreviousGameMenuTab, CurrentTab);
                }
            }
        }

        private static int? PreviousGameMenuTab { get; set; }

        private static void OnGameMenuTabChanged(int? PreviousTab, int? CurrentTab)
        {
            try
            {
                GameMenu GM = Game1.activeClickableMenu as GameMenu;
                if (CurrentTab.HasValue && CurrentTab.Value == GameMenu.craftingTab)
                {
                    OnCraftingPageActivated(GM.pages.First(x => x is CraftingPage) as CraftingPage);
                }
                else if (PreviousTab.HasValue && PreviousTab.Value == GameMenu.craftingTab)
                {
                    OnCraftingPageDeactivated();
                }

            }
            finally { PreviousGameMenuTab = CurrentTab; }
        }

        private static bool IsViewingGameMenu { get; set; } = false;

        private static void OnGameMenuOpened()
        {
            IsViewingGameMenu = true;
            GameMenu GM = Game1.activeClickableMenu as GameMenu;
            OnGameMenuTabChanged(null, GM.currentTab);
        }

        private static void OnGameMenuClosed()
        {
            OnGameMenuTabChanged(PreviousGameMenuTab, null);
            IsViewingGameMenu = false;
        }

        private static void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is GameMenu)
            {
                OnGameMenuOpened();
            }
            else if (e.OldMenu is GameMenu)
            {
                OnGameMenuClosed();
            }

            if (e.NewMenu is CraftingPage CP)
            {
                OnCraftingPageActivated(CP);
            }
            else if (e.OldMenu is CraftingPage)
            {
                OnCraftingPageDeactivated();
            }
        }
    }
}