/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace Omegasis.BuildHealth.Framework
{
    /// <summary>The data for the current player.</summary>
    internal class PlayerData
    {
        /// <summary>The player's current endurance level.</summary>
        public int CurrentLevel { get; set; }

        /// <summary>The player's current endurance XP points.</summary>
        public double CurrentExp { get; set; }

        /// <summary>The XP points needed to reach the next endurance level.</summary>
        public double ExpToNextLevel { get; set; } = 20;

        /// <summary>The initial health bonus to apply regardless of the player's endurance level, from the config file.</summary>
        public int BaseHealthBonus { get; set; }

        /// <summary>The health points to add to the player's base health due to their current endurance level.</summary>
        public int CurrentLevelHealthBonus { get; set; }

        /// <summary>Whether to reset all changes by the mod to the default values (i.e. start over).</summary>
        public bool ClearModEffects { get; set; }

        /// <summary>The player's original maximum health value, excluding mod effects.</summary>
        public int OriginalMaxHealth { get; set; }
    }
}
