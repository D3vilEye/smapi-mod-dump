/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ShopCore
{
    class ShopLogic
    {

        public static void openSeedShopMenu()
        {
            Game1.activeClickableMenu = new StardewValley.Menus.ShopMenu(UtilityCore.SeedCropUtility.sortSeedListByUtility(getGeneralStoreSeedStock(true)));
        }

        public static List<Item> getGeneralStoreSeedStock(bool removeExpensiveItems){
            List<Item> buyableSeeds = new List<Item>();
            foreach (var location in Game1.locations)
            {
                if (location.name == "SeedShop")
                {
                    List<Item> stock=(location as StardewValley.Locations.SeedShop).shopStock();
                    foreach(var item in stock)
                    {
                        if (item.getCategoryName() == "Seed")
                        {
                            if (removeExpensiveItems)
                            {
                                if (item.salePrice() > Game1.player.money) continue;
                            }
                            buyableSeeds.Add(item);
                        }
                    }
                    return buyableSeeds;
                }
            }
            return buyableSeeds;
        }
    }
}
