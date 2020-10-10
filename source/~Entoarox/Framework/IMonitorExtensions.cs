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
using System.Collections.Generic;
using StardewModdingAPI;

namespace Entoarox.Framework
{
    public static class IMonitorExtensions
    {
        /*********
        ** Fields
        *********/
        private static readonly HashSet<string> Cache = new HashSet<string>();


        /*********
        ** Public methods
        *********/
        public static void Log(this IMonitor self, string message, LogLevel level = LogLevel.Debug, Exception error = null)
        {
            if (error != null)
                message += Environment.NewLine + error;
            self.Log(message, level);
        }

        public static void LogOnce(this IMonitor self, string message, LogLevel level = LogLevel.Debug, Exception error = null)
        {
            if (error != null)
                message += Environment.NewLine + error;
            if (IMonitorExtensions.Cache.Add($"{level}:{message}"))
                self.Log(message, level);
        }

        public static void ExitGameImmediately(this IMonitor self, string message, Exception error = null)
        {
            if (error != null)
                message += Environment.NewLine + error;

            self.Log(message, LogLevel.Error);
            self.Log("Forcing game crash", LogLevel.Error);
            void InfiniteRecursiveLoop() => InfiniteRecursiveLoop();
            InfiniteRecursiveLoop();
        }
    }
}
