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
using EntoEvents = Entoarox.Framework.Core.Utilities.Events;

namespace Entoarox.Framework.Events
{
    public static class MoreEvents
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Triggered whenever a action tile is activated by the player.</summary>
        public static event EventHandler<EventArgsActionTriggered> ActionTriggered;

        /// <summary>Triggered whenever a action tile is first walked onto by the player.</summary>
        public static event EventHandler<EventArgsActionTriggered> TouchActionTriggered;


        /*********
        ** Public methods
        *********/
        internal static void FireActionTriggered(EventArgsActionTriggered eventArgs)
        {
            EntoEvents.FireEventSafely("ActionTriggered", ActionTriggered, eventArgs);
        }

        internal static void FireTouchActionTriggered(EventArgsActionTriggered eventArgs)
        {
            EntoEvents.FireEventSafely("TouchActionTriggered", TouchActionTriggered, eventArgs);
        }
    }
}
