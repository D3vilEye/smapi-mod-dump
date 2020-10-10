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
using System.IO;
using StardewModdingAPI;

namespace Entoarox.AdvancedLocationLoader
{
    internal static class ContentPackExtensions
    {
        /*********
        ** Public methods
        *********/
        public static string GetRelativePath(this IContentPack contentPack, string fromAbsolutePath, string toLocalPath)
        {
            Uri fromUri = new Uri(fromAbsolutePath + Path.DirectorySeparatorChar);
            Uri toUri = new Uri(Path.Combine(contentPack.DirectoryPath, toLocalPath));
            if (fromUri.Scheme != toUri.Scheme)
                throw new InvalidOperationException($"Unable to make path relative to the DLL: {Path.Combine(contentPack.DirectoryPath, toLocalPath)}");
            return Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
        }
    }
}
