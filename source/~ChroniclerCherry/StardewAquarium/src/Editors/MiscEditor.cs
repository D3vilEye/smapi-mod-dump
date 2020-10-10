/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;

namespace StardewAquarium.Editors
{
    class MiscEditor : IAssetEditor
    {
        private const string LocationsPath = "Data\\Locations";
        private const string UIPath = "Strings\\UI";
        private readonly IModHelper _helper;

        public MiscEditor(IModHelper helper)
        {
            this._helper = helper;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(LocationsPath) || asset.AssetNameEquals(UIPath);
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals(LocationsPath))
            {
                var data = asset.AsDictionary<string, string>().Data;
                data[ModEntry.Data.ExteriorMapName] = data["Beach"];
            } else if (asset.AssetNameEquals(UIPath))
            {
                var data = asset.AsDictionary<string, string>().Data;
                data.Add("Chat_StardewAquarium.FishDonated", _helper.Translation.Get("FishDonatedMP"));
                data.Add("Chat_StardewAquarium.AchievementUnlocked", _helper.Translation.Get("AchievementUnlockedMP"));
            }
        }
    }
}
