/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AdvizeGH/FarmExpansion
**
*************************************************/

using StardewValley;

namespace FarmExpansion.Framework
{
    /// <summary>An API provided by Farm Expansion for other mods to use.</summary>
    public interface IModApi
    {
        /// <summary>Add a blueprint to all future carpenter menus for the farm area.</summary>
        /// <param name="blueprint">The blueprint to add.</param>
        void AddFarmBluePrint(BluePrint blueprint);

        /// <summary>Add a blueprint to all future carpenter menus for the expansion area.</summary>
        /// <param name="blueprint">The blueprint to add.</param>
        void AddExpansionBluePrint(BluePrint blueprint);
    }
}