/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace Entoarox.Framework.Core.Utilities
{
    internal class DictionaryAssetInfo<TKey, TValue>
    {
        /*********
        ** Accessors
        *********/
        public IContentHelper ContentHelper { get; }
        public IDictionary<TKey, TValue> Data { get; }


        /*********
        ** Public methods
        *********/
        public DictionaryAssetInfo(IContentHelper contentHelper, IDictionary<TKey, TValue> data)
        {
            this.ContentHelper = contentHelper;
            this.Data = data;
        }
    }
}
