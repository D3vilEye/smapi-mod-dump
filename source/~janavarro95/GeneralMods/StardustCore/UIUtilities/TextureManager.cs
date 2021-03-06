/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace StardustCore.UIUtilities
{
    public class TextureManager
    {
        public Dictionary<string, Texture2DExtended> textures;

        public TextureManager()
        {
            this.textures = new Dictionary<string, Texture2DExtended>();
        }

        public void addTexture(string name, Texture2DExtended texture)
        {
            this.textures.Add(name, texture);
        }

        /// <summary>Returns a Texture2DExtended held by the manager.</summary>
        public Texture2DExtended getTexture(string name)
        {
            foreach (var v in this.textures)
            {
                if (v.Key == name)
                    return v.Value.Copy();
            }
            throw new Exception("Error, texture name not found!!!");
        }
    }
}
