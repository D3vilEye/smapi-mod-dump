/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace MobileCatalogues
{
    public class Catalogues
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IModHelper helper, IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }
        internal static void OpenCatalogue(string id)
        {
            switch (id)
            {
                case "catalogue":
                    OpenCatalogue();
                    break;
                case "furniture-catalogue":
                    OpenFurnitureCatalogue();
                    break;
                case "seed-catalogue":
                    OpenSeedCatalogue();
                    break;
                case "travel-catalogue":
                    OpenTravelingCatalogue();
                    break;
                case "desert-catalogue":
                    OpenDesertCatalogue();
                    break;
                case "hat-catalogue":
                    OpenHatMouseCatalogue();
                    break;
            }
        }
        public static void OpenCatalogue()
        {
            Monitor.Log("Opening catalogue");
            DelayedOpen(new ShopMenu(GetAllWallpapersAndFloors(), 0, null, null, null, "Catalogue"));
            
        }

        public static void OpenFurnitureCatalogue()
        {
            Monitor.Log("Opening furniture catalogue");
            DelayedOpen(new ShopMenu(GetAllFurnitures(), 0, null, null, null, "Furniture Catalogue"));
        }

        public static void OpenSeedCatalogue()
        {
            Monitor.Log("Opening seed catalogue");
            DelayedOpen(new ShopMenu(GetAllSeeds(), 0, null, null, null, "Seed Catalogue"));
        }

        public static void OpenTravelingCatalogue()
        {
            if (Config.LimitTravelingCatalogToInTown && Game1.getLocationFromName("Forest") != null && !(Game1.getLocationFromName("Forest") as Forest).travelingMerchantDay)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("traveling-merchant-not-here"));
                return;
            }

            var dict = Utility.getTravelingMerchantStock((int)(Game1.uniqueIDForThisGame + (ulong)Game1.stats.DaysPlayed));

            AdjustPrices(ref dict, Config.FreeTravelingCatalogue);

            Monitor.Log("Opening traveling catalogue");
            DelayedOpen(new ShopMenu(dict, 0, "Traveler", new Func<ISalable, Farmer, int, bool>(Utility.onTravelingMerchantShopPurchase), null, null));
        }

        public static void OpenDesertCatalogue()
        {
            if (Config.LimitDesertCatalogToBusFixed && !Game1.player.mailReceived.Contains("ccVault"))
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("desert-merchant-cannot-ship"));
                return;
            }
            var dict = Desert.getDesertMerchantTradeStock(Game1.player);
            AdjustPrices(ref dict, Config.FreeDesertCatalogue);

            Monitor.Log("Opening desert catalogue");
            DelayedOpen(new ShopMenu(dict, 0, "DesertTrade", new Func<ISalable, Farmer, int, bool>(boughtTraderItem), null, null));
        }


        public static void OpenHatMouseCatalogue()
        {
            if (Game1.player.achievements.Count == 0)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("catalog-not-available"));
                return;
            }
            var dict = Utility.getHatStock();
            AdjustPrices(ref dict, Config.FreeHatCatalogue);

            Monitor.Log("Opening hat catalogue");
            DelayedOpen(new ShopMenu(dict, 0, "HatMouse", null, null, null));
        }

        private static async void DelayedOpen(ShopMenu menu)
        {
            await Task.Delay(100);
            Monitor.Log("Really opening catalogue");
            Game1.activeClickableMenu = menu;
        }

        private static Dictionary<ISalable, int[]> GetAllWallpapersAndFloors()
        {
            Dictionary<ISalable, int[]> decors = new Dictionary<ISalable, int[]>();
            Wallpaper f;
            for (int i = 0; i < 112; i++)
            {
                f = new Wallpaper(i, false);
                decors.Add(new Wallpaper(i, false)
                {
                    Stack = int.MaxValue
                }, new int[]
                {
                    Config.FreeCatalogue  ? 0 : (int)Math.Round(f.salePrice() * Config.PriceMult),
                    int.MaxValue
                });
            }
            for (int j = 0; j < 56; j++)
            {
                f = new Wallpaper(j, false);
                decors.Add(new Wallpaper(j, true)
                {
                    Stack = int.MaxValue
                }, new int[]
                {
                    Config.FreeCatalogue  ? 0 : (int)Math.Round(f.salePrice() * Config.PriceMult),
                    int.MaxValue
                });
            }
            return decors;
        }

        private static Dictionary<ISalable, int[]> GetAllFurnitures()
        {
            Dictionary<ISalable, int[]> decors = new Dictionary<ISalable, int[]>();
            Furniture f;
            foreach (KeyValuePair<int, string> v in Game1.content.Load<Dictionary<int, string>>("Data\\Furniture"))
            {
                if (true)
                {
                    f = new Furniture(v.Key, Vector2.Zero);
                    decors.Add(f, new int[]
                    {
                        Config.FreeFurnitureCatalogue ? 0 : (int)Math.Round(f.salePrice() * Config.PriceMult),
                        int.MaxValue
                    });
                }
            }
            f = new Furniture(1402, Vector2.Zero);
            decors.Add(f, new int[]
            {
                Config.FreeFurnitureCatalogue  ? 0 :  (int)Math.Round(f.salePrice() * Config.PriceMult),
                int.MaxValue
            });
            f = new TV(1680, Vector2.Zero);
            decors.Add(f, new int[]
            {
                Config.FreeFurnitureCatalogue  ? 0 : (int)Math.Round(f.salePrice() * Config.PriceMult),
                int.MaxValue
            });
            f = new TV(1466, Vector2.Zero);
            decors.Add(f, new int[]
            {
                Config.FreeFurnitureCatalogue  ? 0 :  (int)Math.Round(f.salePrice() * Config.PriceMult),
                int.MaxValue
            });
            f = new TV(1468, Vector2.Zero);
            decors.Add(f, new int[]
            {
                Config.FreeFurnitureCatalogue  ? 0 :  (int)Math.Round(f.salePrice() * Config.PriceMult),
                int.MaxValue
            });
            return decors;
        }
        private static Dictionary<ISalable, int[]> GetAllSeeds()
        {
            Dictionary<ISalable, int[]> items = new Dictionary<ISalable, int[]>();
            Dictionary<int, string> cropData = Helper.Content.Load<Dictionary<int, string>>("Data\\Crops", 0);
            Dictionary<int, string> fruitTreeData = Helper.Content.Load<Dictionary<int, string>>("Data\\fruitTrees", 0);

            Dictionary<int, int> seedProducts = new Dictionary<int, int>();

            foreach (KeyValuePair<int, string> kvp in cropData)
            {
                string[] values = kvp.Value.Split('/');
                if (!int.TryParse(values[3], out int product))
                    continue;
                seedProducts.Add(kvp.Key, product);
            }
            foreach (KeyValuePair<int, string> kvp in fruitTreeData)
            {
                string[] values = kvp.Value.Split('/');
                if (!int.TryParse(values[2], out int product))
                    continue;
                seedProducts.Add(kvp.Key, product);
            }

            foreach (KeyValuePair<int, int> crop in seedProducts)
            {
                bool include = true;
                if(Config.SeedsToInclude.ToLower() == "shipped")
                {
                    include = Game1.player.basicShipped.ContainsKey(crop.Value);
                }
                else if (Config.SeedsToInclude.ToLower() == "season")
                {
                    include = new Crop(crop.Key, 0, 0).seasonsToGrowIn.Contains(Game1.currentSeason);
                }
                if (include)
                {
                    Object item = new Object(crop.Key, int.MaxValue, false, -1, 0);
                    if (!item.bigCraftable.Value && item.ParentSheetIndex == 745)
                    {
                        item.Price = (int)Math.Round(50 * Config.PriceMult);
                    }
                    items.Add(item, new int[]
                    {
                        Config.FreeSeedCatalogue ? 0 :  (int)Math.Round(item.salePrice() * Config.PriceMult),
                        int.MaxValue
                    });
                }
            }
            return items;
        }

        private static void AdjustPrices(ref Dictionary<ISalable, int[]> dict, bool free)
        {
            ISalable[] keys = dict.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                dict[keys[i]][0] = free ? 0 : (int)Math.Round(dict[keys[i]][0] * Config.PriceMult);
            }
        }
        public static bool boughtTraderItem(ISalable s, Farmer arg2, int arg3)
        {
            if (s.Name == "Magic Rock Candy")
            {
                Desert.boughtMagicRockCandy = true;
            }
            return false;
        }
    }
}
