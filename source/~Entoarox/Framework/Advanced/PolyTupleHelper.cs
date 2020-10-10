/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework.Advanced
{
    internal class PolyTupleHelper
    {
        /*********
        ** Public methods
        *********/
        public static int CreateHashCode(params object[] objects)
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            int i = 0;
            foreach (object obj in objects)
            {
                int hashCode = obj.GetHashCode();
                if (i % 2 == 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ hashCode;
                else
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ hashCode;
                ++i;
            }

            return hash1 + hash2 * 1566083941;
        }
    }
}
