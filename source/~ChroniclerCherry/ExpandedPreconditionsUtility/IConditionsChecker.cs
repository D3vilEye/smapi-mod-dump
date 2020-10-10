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

namespace ExpandedPreconditionsUtility
{
    public interface IConditionsChecker
    {
        /// <summary>
        /// Must be called before any condition checking is done. Verbose mode will turn on logging for every step of the condition checking process
        /// </summary>
        /// <param name="verbose">Turning verbose mode true will log every step of the condition checking process. Useful for debugging but spams the debug log. It is recommended to have this false during release, or provided in a config set to a default of false.</param>
        /// <param name="uniqueId">The unique ID of your mod. Will be prepended to all logs so it is clear which mod called the condition checking</param>
        void Initialize(bool verbose, string uniqueId);

        /// <summary>
        /// Checks an array of condition strings. Each string will be evaluated as true if every single condition provided is true. All the strings together will evaluate as true if any string is true
        /// </summary>
        /// <param name="conditions">An array of condition strings.</param>
        /// <returns></returns>
        bool CheckConditions(string[] conditions);

        /// <summary>
        /// Checks a single condition string. The string will be evaluated as true if every single condition provided is true.
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        bool CheckConditions(string conditions);
    }

    public class ConditionsChecker : IConditionsChecker
    {
        private ConditionChecker _conditionChecker;

        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        internal ConditionsChecker(IMonitor monitor, IModHelper helper)
        {
            _helper = helper;
            _monitor = monitor;
        }

        public void Initialize(bool verbose, string uniqueId)
        {
            _conditionChecker = new ConditionChecker(_helper, _monitor, verbose, uniqueId);
        }
        public bool CheckConditions(string[] conditions)
        {
            return _conditionChecker.CheckConditions(conditions);
        }

        public bool CheckConditions(string conditions)
        {
            return _conditionChecker.CheckConditions(new[] { conditions });
        }
    }
}
