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
    /// <summary>The mod settings and player data.</summary>
    internal class ModConfig
    {
        /// <summary>The XP points needed to reach the next level.</summary>
        public double ExpToNextLevel { get; set; } = 20;

        /// <summary>The player's current XP points.</summary>
        public double CurrentExp { get; set; }

        /// <summary>The player's current level.</summary>
        public int CurrentLevel { get; set; }

        /// <summary>The initial health bonus to apply regardless of the player's level, from the config file.</summary>
        public int BaseHealthBonus { get; set; }

        /// <summary>The health points to add to the player's base health due to their current level.</summary>
        public int CurrentLevelHealthBonus { get; set; }

        /// <summary>The multiplier for the experience points to need to reach an endurance level relative to the previous one.</summary>
        public double ExpCurve { get; set; } = 1.15;

        /// <summary>The maximum endurance level the player can reach.</summary>
        public int MaxLevel { get; set; } = 100;

        /// <summary>The amount of stamina the player should gain for each endurance level.</summary>
        public int HealthIncreasePerLevel { get; set; } = 1;

        /// <summary>The experience points to gain for using a tool.</summary>
        public int ExpForToolUse { get; set; } = 1;

        /// <summary>The experience points to gain for eating or drinking.</summary>
        public int ExpForEating { get; set; } = 2;

        /// <summary>The experience points to gain for sleeping.</summary>
        public int ExpForSleeping { get; set; } = 10;

        /// <summary>The experience points to gain for collapsing for the day.</summary>
        public int ExpForCollapsing { get; set; }
    }
}
