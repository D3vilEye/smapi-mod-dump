/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System.Collections.Generic;

namespace CJBCheatsMenu.Framework.Models
{
    /// <summary>The model for the data file.</summary>
    internal class ModData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The order in which to list warp sections in the menu. Any other sections will appear alphabetically after these.</summary>
        public string[] SectionOrder { get; set; }

        /// <summary>The warps to show in the cheats menu.</summary>
        public IDictionary<string, ModDataWarp[]> Warps { get; set; }
    }
}
