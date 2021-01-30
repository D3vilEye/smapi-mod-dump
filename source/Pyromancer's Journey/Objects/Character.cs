/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/FireArcadeGame
**
*************************************************/

using FireArcadeGame.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireArcadeGame.Objects
{
    public class Character : BaseObject
    {
        public virtual RectangleF BoundingBox { get; } = new RectangleF( 0, 0, 0.35f, 0.35f );

        public virtual bool Floats { get; } = false;
        public int Health { get; set; } = 1;

        public Character( World world )
        :   base( world )
        {
        }

        public virtual void Hurt( int amt )
        {
            Health -= amt;
        }

        public virtual void DoMovement() { }

        public override void Update()
        {
            var oldPos = Position;

            DoMovement();

            // Lazy implementation - would use something better if using a real engine
            Func< float, float, bool > solidCheck = Floats ? World.map.IsAirSolid : World.map.IsSolid;
            if ( solidCheck( Position.X, Position.Z ) )
            {
                if ( !solidCheck( oldPos.X, Position.Z ) )
                    Position.X = oldPos.X;
                else if ( !solidCheck( Position.X, oldPos.Z ) )
                    Position.Z = oldPos.Z;
                else
                    Position = oldPos;
            }
        }
    }
}
