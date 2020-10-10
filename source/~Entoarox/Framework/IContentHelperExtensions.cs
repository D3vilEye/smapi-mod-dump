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
using System.IO;
using Entoarox.Framework.Core;
using Entoarox.Framework.Core.AssetHandlers;
using Entoarox.Framework.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Entoarox.Framework
{
    public static class IContentHelperExtensions
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Allows you to add a new type to the serializer, provided the serializer has not yet been initialized.</summary>
        /// <typeparam name="T">The Type to add</typeparam>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to</param>
        [Obsolete("Deprecated, use save events to add/remove custom content instead.")]
        public static void RegisterSerializerType<T>(this IContentHelper helper)
        {
            EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(helper)}` mod uses deprecated serializer injection, this will be removed in a future update.", LogLevel.Alert);
            if (EntoaroxFrameworkMod.SerializerInjected)
                EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(helper)}` mod failed to augment the serializer, serializer has already been created.", LogLevel.Error);
            else if (!EntoaroxFrameworkMod.SerializerTypes.Contains(typeof(T)))
                EntoaroxFrameworkMod.SerializerTypes.Add(typeof(T));
            else
                EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(helper)}` mod failed to augment the serializer, the `{typeof(T).FullName}` type has already been injected before.", LogLevel.Warn);
        }

        /// <summary>Lets you replace a region of pixels in one texture with the contents of another texture. The texture asset referenced by patchAssetName has to be in xnb format.</summary>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetName">The texture asset (Relative to Content and without extension) that you wish to modify.</param>
        /// <param name="patchAssetName">The texture asset (Relative to your mod directory and without extension) used for the modification.</param>
        /// <param name="destination">The area you wish to replace.</param>
        /// <param name="source">The area you wish to use for replacement, if omitted the full patch texture is used.</param>
        public static void RegisterTexturePatch(this IContentHelper helper, string assetName, string patchAssetName, Rectangle? destination = null, Rectangle? source = null)
        {
            helper.RegisterTexturePatch(assetName, helper.Load<Texture2D>(patchAssetName), destination, source);
        }

        /// <summary>Lets you replace a region of pixels in one texture with the contents of another texture.</summary>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetName">The texture asset (Relative to Content and without extension) that you wish to modify.</param>
        /// <param name="patchAsset">The texture used for the modification.</param>
        /// <param name="destination">The area you wish to replace.</param>
        /// <param name="source">The area you wish to use for replacement, if omitted the full patch texture is used.</param>
        public static void RegisterTexturePatch(this IContentHelper helper, string assetName, Texture2D patchAsset, Rectangle? destination = null, Rectangle? source = null)
        {
            assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
            if (!TextureInjector.Map.ContainsKey(assetName))
                TextureInjector.Map.Add(assetName, new List<TextureInjectorInfo>());
            TextureInjector.Map[assetName].Add(new TextureInjectorInfo(patchAsset, source, destination));
            helper.InvalidateCache(assetName);
        }

        /// <summary>Lets you add and replace keys in a content dictionary. The dictionary asset referenced by patchAssetName has to be in xnb format.</summary>
        /// <typeparam name="TKey">The type used for keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type used for values in the dictionary.</typeparam>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetName">The dictionary asset (Relative to Content and without extension) that you wish to modify.</param>
        /// <param name="patchAssetName">The dictionary asset (Relative to your mod directory and without extension) used for the modification.</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RegisterDictionaryPatch<TKey, TValue>(this IContentHelper helper, string assetName, string patchAssetName)
        {
            try
            {
                helper.RegisterDictionaryPatch(assetName, helper.Load<Dictionary<TKey, TValue>>(patchAssetName));
            }
            catch
            {
                EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(helper)}` mod\'s attempt to inject data into the `{assetName}` asset failed, as the TKey and TValue given do not match those of the data to inject", LogLevel.Error);
            }
        }

        /// <summary>Lets you add and replace keys in a content dictionary.</summary>
        /// <typeparam name="TKey">The type used for keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type used for values in the dictionary.</typeparam>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetName">The dictionary asset (Relative to Content and without extension) that you wish to modify.</param>
        /// <param name="patchAsset">The dictionary used for the modification.</param>
        [Obsolete("This API member is not yet functional in the current development build.")]
        public static void RegisterDictionaryPatch<TKey, TValue>(this IContentHelper helper, string assetName, Dictionary<TKey, TValue> patchAsset)
        {
            try
            {
                //Dictionary<TKey, TValue> check = Game1.content.Load<Dictionary<TKey, TValue>>(assetName);
                assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
                if (!DictionaryInjector.Map.ContainsKey(assetName))
                    DictionaryInjector.Map.Add(assetName, new DictionaryWrapper<TKey, TValue>());
                (DictionaryInjector.Map[assetName] as DictionaryWrapper<TKey, TValue>).Register(helper, patchAsset);
                helper.InvalidateCache(assetName);
            }
            catch
            {
                EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(helper)}` mod\'s attempt to inject data into the `{assetName}` asset failed, as the TKey and TValue of the injected asset do not match the original.", LogLevel.Error);
            }
        }

        /// <summary>Lets you define a xnb file to completely replace with another.</summary>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetName">The asset (Relative to Content and without extension) to replace.</param>
        /// <param name="replacementAssetName">The asset (Relative to your mod directory and without extension) to use instead.</param>
        public static void RegisterXnbReplacement(this IContentHelper helper, string assetName, string replacementAssetName)
        {
            assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
            replacementAssetName = helper.GetActualAssetKey(replacementAssetName, ContentSource.GameContent);
            if (XnbLoader.Map.ContainsKey(assetName))
                EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(helper)}` mod\'s attempt to register a replacement asset for the `{assetName}` asset failed, as another mod has already done so.", LogLevel.Error);
            else
            {
                XnbLoader.Map.Add(assetName, Tuple.Create(helper, replacementAssetName));
                helper.InvalidateCache(assetName);
            }
        }

        /// <summary>Lets you define a xnb file to completely replace with another.</summary>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetMapping">Dictionary with the asset (Relative to Content and without extension) to replace as key, and the asset (Relative to your mod directory and without extension) to use instead as value.</param>
        public static void RegisterXnbReplacements(this IContentHelper helper, Dictionary<string, string> assetMapping)
        {
            List<string> matchedAssets = new List<string>();
            foreach (KeyValuePair<string, string> pair in assetMapping)
            {
                string asset = helper.GetActualAssetKey(pair.Key, ContentSource.GameContent);
                string replacement = helper.GetActualAssetKey(pair.Value, ContentSource.GameContent);
                if (XnbLoader.Map.ContainsKey(asset))
                    EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(helper)}` mod\'s attempt to register a replacement asset for the `{pair.Key}` asset failed, as another mod has already done so.", LogLevel.Error);
                else
                {
                    XnbLoader.Map.Add(asset, Tuple.Create(helper, replacement));
                    matchedAssets.Add(asset);
                }
            }

            helper.InvalidateCache(assetInfo => matchedAssets.Contains(assetInfo.AssetName));
        }

        /// <summary>If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the loading for one specific asset.</summary>
        /// <typeparam name="T">The Type the asset is loaded as.</typeparam>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetName">The asset (Relative to Content and without extension) to handle.</param>
        /// <param name="assetLoader">The delegate assigned to handle loading for this asset.</param>
        public static void RegisterLoader<T>(this IContentHelper helper, string assetName, AssetLoader<T> assetLoader)
        {
            assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
            if (DeferredAssetHandler.LoadMap.ContainsKey(assetName))
                EntoaroxFrameworkMod.Logger.Log($"[IContentHelper] The `{Globals.GetModName(helper)}` mod\'s attempt to register a replacement asset for the `{assetName}` asset of type `{typeof(T).FullName}` failed, as another mod has already done so.", LogLevel.Error);
            else
            {
                DeferredAssetHandler.LoadMap.Add(assetName, new DeferredAssetInfo(typeof(T), assetLoader));
                helper.InvalidateCache(assetName);
            }
        }

        /// <summary>If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for one specific asset.</summary>
        /// <typeparam name="T">The Type the asset is loaded as.</typeparam>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetName">The asset (Relative to Content and without extension) to handle.</param>
        /// <param name="assetInjector">The delegate assigned to handle injection for this asset.</param>
        public static void RegisterInjector<T>(this IContentHelper helper, string assetName, AssetInjector<T> assetInjector)
        {
            assetName = helper.GetActualAssetKey(assetName, ContentSource.GameContent);
            if (!DeferredAssetHandler.EditMap.ContainsKey(assetName))
                DeferredAssetHandler.EditMap.Add(assetName, new List<DeferredAssetInfo>());
            DeferredAssetHandler.EditMap[assetName].Add(new DeferredAssetInfo(typeof(T), assetInjector));
            helper.InvalidateCache(assetName);
        }

        /// <summary>If none of the build in content handlers are sufficient, and making a custom one is overkill, this method lets you handle the injection for a specific type of asset.</summary>
        /// <typeparam name="T">The Type the asset is loaded as.</typeparam>
        /// <param name="helper">The <see cref="IContentHelper" /> this extension method is attached to.</param>
        /// <param name="assetInjector">The delegate assigned to handle loading for this type.</param>
        public static void RegisterInjector<T>(this IContentHelper helper, AssetInjector<T> assetInjector)
        {
            if (DeferredTypeHandler.EditMap.ContainsKey(typeof(T)))
                DeferredTypeHandler.EditMap.Add(typeof(T), new List<Delegate>());
            DeferredTypeHandler.EditMap[typeof(T)].Add(assetInjector);
            helper.InvalidateCache<T>();
        }

        public static string GetPlatformRelativeContent(this IContentHelper helper)
        {
            return File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory, "XACT", "FarmerSounds.xgs"))
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Resources", Game1.content.RootDirectory)
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
        }
    }
}
