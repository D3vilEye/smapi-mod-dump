/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework
{
    public interface IInterModHelper
    {
        /*********
        ** Methods
        *********/
        /// <summary>Subscribe for any messages posted to the given channel.</summary>
        /// <param name="channel">The channel to subscribe to</param>
        /// <param name="handler">The handler to receive any messages with</param>
        void Subscribe(string channel, ReceiveMessage handler);

        /// <summary>Subscribe for any messages posted to a channel with this mods unique ID as its channel name.</summary>
        /// <param name="handler"></param>
        void Subscribe(ReceiveMessage handler);

        /// <summary>Publish any given message to the given channel.</summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="message">The message to publish</param>
        void Publish(string channel, string message);
    }
}
