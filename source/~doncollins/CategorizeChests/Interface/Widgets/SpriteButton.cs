/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/doncollins/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace StardewValleyMods.CategorizeChests.Interface.Widgets
{
    /// <summary>
    /// A button that uses a single TextureRegion to display itself.
    /// </summary>
    class SpriteButton : Button
    {
        private readonly TextureRegion TextureRegion;

        public SpriteButton(TextureRegion textureRegion)
        {
            TextureRegion = textureRegion;
            Width = TextureRegion.Width;
            Height = TextureRegion.Height;
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(TextureRegion.Texture, TextureRegion.Region, GlobalPosition.X, GlobalPosition.Y,
                TextureRegion.Width, TextureRegion.Height);
        }
    }
}