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
using Microsoft.Xna.Framework;

namespace Entoarox.Framework.UI
{
    internal class ClickableCollectionComponent : GenericCollectionComponent
    {
        /*********
        ** Accessors
        *********/
        public event ClickHandler Handler;


        /*********
        ** Public methods
        *********/
        public ClickableCollectionComponent(Point size, ClickHandler handler = null, List<IMenuComponent> components = null)
            : base(size, components)
        {
            if (handler != null)
                this.Handler += handler;
        }

        public ClickableCollectionComponent(Rectangle area, ClickHandler handler = null, List<IMenuComponent> components = null)
            : base(area, components)
        {
            if (handler != null)
                this.Handler += handler;
        }

        public override void LeftHeld(Point p, Point o) { }

        public override void LeftUp(Point p, Point o) { }

        public override void LeftClick(Point p, Point o)
        {
            this.Handler?.Invoke(this, this.Parent, this.Parent.GetAttachedMenu());
        }

        public override void RightClick(Point p, Point o) { }
    }
}
