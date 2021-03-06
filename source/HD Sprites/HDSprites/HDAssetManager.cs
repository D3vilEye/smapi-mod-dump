/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ninthworld/HDSprites
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;

namespace HDSprites
{
    public class HDAssetManager
    {
        private Dictionary<string, string> HDAssetFiles { get; set; }
        private Dictionary<string, Texture2D> HDTextures { get; set; }
        private IModHelper Helper { get; set; }

        public HDAssetManager(IModHelper helper)
        {
            this.HDAssetFiles = new Dictionary<string, string>();
            this.HDTextures = new Dictionary<string, Texture2D>();
            this.Helper = helper;
        }

        public void AddAssetFile(string assetName, string file)
        {
            assetName = assetName.Replace("/", $"\\");
            if (!this.HDAssetFiles.ContainsKey(assetName))
            {
                this.HDAssetFiles.Add(assetName, file);
            }
        }

        public Texture2D LoadAsset(string assetName)
        {
            if (!this.HDTextures.TryGetValue(assetName, out Texture2D texture))
            {
                if (this.HDAssetFiles.TryGetValue(assetName, out string file))
                {
                    texture = this.Helper.Content.Load<Texture2D>(file, ContentSource.ModFolder);
                    this.HDTextures.Add(assetName, texture);
                }
            }
            return texture;
        }
    }
}
