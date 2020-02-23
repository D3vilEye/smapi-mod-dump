using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace ShopTileFramework
{
    class AnimalShop
    {
        private List<StardewValley.Object> ShopAnimalStock;
        private List<StardewValley.Object> AllAnimalsStock;
        private AnimalShopPack ShopPack;
        public AnimalShop(AnimalShopPack ShopPack, string ShopName)
        {
            this.ShopPack = ShopPack;
        }

        private void UpdateShopAnimalStock()
        {
            //BFAV patches this anyways so it'll automatically work if installed
            AllAnimalsStock = Utility.getPurchaseAnimalStock();

            ShopAnimalStock = new List<StardewValley.Object>();
            foreach (var animal in AllAnimalsStock)
            {
                if (ShopPack.AnimalStock.Contains(animal.Name))
                {
                    ShopAnimalStock.Add(animal);
                }
            }
        }

        internal void DisplayShop()
        {
            if (ConditionChecking.CheckConditions(ShopPack.When))
            {
                //get animal stock each time to refresh requirement checks
                UpdateShopAnimalStock();
                ModEntry.SourceLocation = Game1.currentLocation;
                Game1.activeClickableMenu = new PurchaseAnimalsMenu(ShopAnimalStock);
            }
            else if (ShopPack.ClosedMessage != null)
            {
                Game1.activeClickableMenu = new DialogueBox(ShopPack.ClosedMessage);
            }
        }
    }

    public interface BFAVApi
    {
        bool IsEnabled();
        List<StardewValley.Object> GetAnimalShopStock(Farm farm);
        Dictionary<string, List<string>> GetFarmAnimalCategories();

    }
}