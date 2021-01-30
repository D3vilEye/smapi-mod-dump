/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System.IO;
using System.Text;

namespace ChatCommands
{
    /// <summary>
    ///     A <see cref="TextWriter" /> that intercepts another <see cref="TextWriter" /> and notifes
    ///     anything written to it.
    /// </summary>
    /// <remarks>
    ///     Borrowed heavily from
    ///     https://github.com/Pathoschild/SMAPI/blob/develop/src/SMAPI/Framework/Logging/InterceptingTextWriter.cs
    /// </remarks>
    internal class NotifyingTextWriter : TextWriter
    {
        public delegate void OnLineWritten(char[] buffer, int index, int count);

        private readonly OnLineWritten callback;

        private readonly TextWriter original;

        public bool ForceNotify { get; set; }

        public bool Notify { get; set; }

        public NotifyingTextWriter(TextWriter original, OnLineWritten callback)
        {
            this.original = original;
            this.callback = callback;
        }

        public override Encoding Encoding => this.original.Encoding;

        /// <summary>When a write is invoked, send the information over to the callback.</summary>
        public override void Write(char[] buffer, int index, int count)
        {
            if (this.Notify || this.ForceNotify)
                this.callback(buffer, index, count);
            this.original.Write(buffer, index, count);
        }

        public override void Write(char ch)
        {
            this.original.Write(ch);
        }

    }
}
