/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewValley;

namespace ExpandedStorage.Framework.Models
{
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    internal class StorageContentData : StorageConfig
    {
        /// <summary>The game sound that will play when the storage is opened.</summary>
        public string OpenSound;
        
        /// <summary>One of the special chest types (None, MiniShippingBin, JunimoChest).</summary>
        public string SpecialChestType;

        /// <summary>The UniqueId of the Content Pack that storage data was loaded from.</summary>
        internal string ModUniqueId;

        /// <summary>True for assets loaded into Game1.bigCraftables outside of JsonAssets.</summary>
        internal bool IsVanilla;

        internal StorageContentData() : this(null)
        {
            OpenSound = "openChest";
            SpecialChestType = "None";
        }
        internal StorageContentData(string storageName) : base(storageName) { }
        internal bool IsAllowed(Item item) => !AllowList.Any() || AllowList.Any(item.HasContextTag);
        internal bool IsBlocked(Item item) => BlockList.Any() && BlockList.Any(item.HasContextTag);
        internal void CopyFrom(StorageConfig config)
        {
            Capacity = config.Capacity;
            CanCarry = config.CanCarry;
            AccessCarried = config.AccessCarried;
            ShowSearchBar = config.ShowSearchBar;
            IsPlaceable = config.IsPlaceable;
            AllowList = config.AllowList;
            BlockList = config.BlockList;
            Tabs = config.Tabs;
        }
    }
}