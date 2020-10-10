/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using StardewValley;

namespace Entoarox.Framework.Core
{
    public class ConditionHelper : IConditionHelper
    {
        /*********
        ** Fields
        *********/
        private static readonly string[] WeatherTypes = { "weatherWedding", "weatherFestival", "weatherSun", "weatherSummerDebris", "weatherRain", "weatherStorm", "weatherFallDebris", "weatherSnow" };
        private static readonly string[] Weekdays = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };


        /*********
        ** Public methods
        *********/
        public bool ValidateCondition(string condition)
        {
            // Check if this is a negated condition
            if (condition.StartsWith("!"))
            {
                // Check if it is negated multiple times, as this risks a infinite loop situation.
                // If we find multiple negations, we forcibly return false.
                // If we don't find multiple negations, we negate the result of a positive condition lookup.
                return !condition.StartsWith("!!") && !this.ValidateCondition(condition.Substring(1));
            }

            // If it is not a negated condition, we perform a positive condition lookup
            switch (condition)
            {
                // Conditions for the possible weather states
                case "weatherSun":
                case "weatherRain":
                case "weatherSnow":
                case "weatherStorm":
                case "weatherFestival":
                case "weatherWedding":
                case "weatherFallDebris":
                case "weatherSummerDebris":
                    return condition == ConditionHelper.WeatherTypes[Game1.weatherIcon];
                // Special condition for "Any debris weather"
                case "weatherDebris":
                    return Game1.weatherIcon == 3 || Game1.weatherIcon == 6;
                // Special condition for "Weather only possible in the current season"
                case "weatherSeasonal":
                    switch (Game1.currentSeason)
                    {
                        case "summer":
                            return Game1.weatherIcon == 3;
                        case "fall":
                            return Game1.weatherIcon == 6;
                        case "winter":
                            return Game1.weatherIcon == 7;
                        default:
                            return false;
                    }
                // Condition for if one of the two special weather states is the current state
                case "weatherSpecial":
                    return Game1.weatherIcon < 2;
                // Conditions for each of the 7 days
                case "sunday":
                case "monday":
                case "tuesday":
                case "wednesday":
                case "thursday":
                case "friday":
                case "saturday":
                    return condition == ConditionHelper.Weekdays[Game1.dayOfMonth % 7];
                // Special condition for either saturday or sunday
                case "weekend":
                    int day = Game1.dayOfMonth % 7;
                    return day == 0 || day == 6;
                // Conditions for each of the 4 seasons
                case "spring":
                case "summer":
                case "fall":
                case "winter":
                    return condition == Game1.currentSeason;
                // Generic condition that is true whenever the player is married, irrelevant of the person they are married to
                case "married":
                    return Game1.player.spouse != null && !Game1.player.spouse.Contains("engaged");
                // Generic condition that is true whenever the player is engaged to be married, irrelevant of the person they are engaged to
                case "engaged":
                    return Game1.player.spouse != null && Game1.player.spouse.Contains("engaged");
                // Condition that is only true after the earthquake event has happened
                case "earthquake":
                    return Game1.stats.daysPlayed > 30;
                // Condition that is only true after the player has received the rusty key
                case "rustyKey":
                    return Game1.player.hasRustyKey;
                // Condition that is only true after the player has received the skull key
                case "skullKey":
                    return Game1.player.hasSkullKey;
                // Condition that is only true after the player has received the club card
                case "clubMember":
                    return Game1.player.hasClubCard;
                // Condition that is only true after the player has learned to speak the dwarven language
                case "dwarfSpeak":
                    return Game1.player.canUnderstandDwarves;
                // New and updated condition handling for more complex conditions
                default:
                    // Handle marriage for any NPC instead of just the default bachelors (Backwards compatible)
                    if (condition.StartsWith("married"))
                        return Game1.player.spouse != null && Game1.player.spouse.Equals(condition.Substring(7));
                    // Handle engagement for any NPC instead of just the default bachelors *new*
                    if (condition.StartsWith("engaged"))
                        return Game1.player.spouse != null && Game1.player.spouse.Contains(condition.Substring(7)) && Game1.player.spouse.Contains("engaged");
                    // Allow year-based conditions for during any specific year
                    if (condition.StartsWith("year="))
                        return Game1.year == Convert.ToInt32(condition.Substring(5));
                    // Allow year-based conditions for after any specific year
                    if (condition.StartsWith("year>"))
                        return Game1.year > Convert.ToInt32(condition.Substring(5));
                    // House upgrade level conditions
                    if (condition.StartsWith("houseLevel="))
                        return Game1.player.HouseUpgradeLevel == Convert.ToInt32(condition.Substring(11));
                    // Farm type conditions
                    if (condition.StartsWith("farmType="))
                        return Game1.whichFarm == Convert.ToInt32(condition.Substring(9));
                    // Time conditions *new*
                    if (condition.StartsWith("time>"))
                        return Game1.timeOfDay > Convert.ToInt32(condition.Substring(5));
                    // Check for mail flags
                    return Game1.player.mailReceived.Contains(condition);
            }
        }

        /// <summary>
        /// <para>Checks a character-separated list of conditions.</para>
        ///
        /// <para>A conflict check is performed the first time a condition string is asked to be checked. The overhead of this conflict check is offset due to most improperly formatted condition lists never being processed. Further, this conflict check enables clear and obvious answers to be given as to why the condition list is improperly formatted. The result of a conflict check is put into a cache for the remainder of the current session. Thus, the second call and after have little to no extra overhead, no matter if correctly formatted or conflicting.</para>
        ///
        /// <para>To decrease unique combinations, condition lists are sorted before any other logic is used on them. By sorting, the total amount of possible permutations is brought down greatly. This means that the cache for a condition list to have been evaluated previously increases proportionally.</para>
        ///
        /// <para>It should be noted that strict mode should be used during development. Strict mode causes extra checks to be used to find performance-causing issues on top of any conflict-related ones.</para>
        /// </summary>
        public bool ValidateConditions(string conditions, char separator = ',')
        {
            return this.ValidateConditions(conditions.Split(separator));
        }

        public bool ValidateConditions(string[] conditions)
        {
            Array.Sort(conditions);
            if (conditions.Length > 5)
                return false;
            foreach (string condition in conditions)
                if (!this.ValidateCondition(condition))
                    return false;
            return true;
        }
    }
}
