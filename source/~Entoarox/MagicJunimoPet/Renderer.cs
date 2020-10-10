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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagicJunimoPet
{
    public abstract class Renderer
    {
        public abstract void Draw(SmartPet pet, SpriteBatch batch);
        public abstract void Update(SmartPet pet, GameTime time);
        public virtual string HappySound => "Silence";
        public virtual string ThudSound => "Silence";
        public virtual string WagSound => "Silence";
    }
}
