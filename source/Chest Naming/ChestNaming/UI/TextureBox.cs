/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/ChestNaming
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace ChestNaming.UI
{
    /// <summary>
    /// The default background texture, usually used to render UI or Tooltips
    /// </summary>
    public class TextureBox : IFrameDrawable
    {
        Color color;
        public int SizeX => 0;
        public int SizeY => 0;
        public TextureBox(Color c)
        {
            color = c;
        }
        public void Draw(SpriteBatch b, int x, int y, Frame parentFrame)
        {
            IClickableMenu.drawTextureBox(b, x, y, parentFrame.Width, parentFrame.Height, color);
        }
    }
}
