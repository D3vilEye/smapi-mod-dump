/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewValley;

namespace Pathoschild.Stardew.SmallBeachFarm.Framework.Config
{
    /// <summary>An available farm map to replace.</summary>
    internal class ModFarmMapData
    {
        /// <summary>The internal farm ID (e.g. <see cref="Farm.riverlands_layout"/>).</summary>
        public int ID { get; set; }

        /// <summary>A human-readable name to show in error messages.</summary>
        public string Name { get; set; }

        /// <summary>The farm map asset name.</summary>
        public string Map { get; set; }
    }
}
