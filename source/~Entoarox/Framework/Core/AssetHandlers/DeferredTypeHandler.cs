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

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class DeferredTypeHandler : IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        internal static Dictionary<Type, List<Delegate>> EditMap = new Dictionary<Type, List<Delegate>>();


        /*********
        ** Public methods
        *********/
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return DeferredTypeHandler.EditMap.ContainsKey(typeof(T));
        }

        public void Edit<T>(IAssetData assetData)
        {
            T asset = assetData.GetData<T>();
            foreach (AssetInjector<T> injector in DeferredTypeHandler.EditMap[typeof(T)])
                injector(assetData.AssetName, ref asset);
            assetData.ReplaceWith(asset);
        }
    }
}
