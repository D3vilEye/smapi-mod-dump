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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Entoarox.Framework.UI
{
    public class FormCollectionComponent : BaseFormComponent, IComponentCollection
    {
        /*********
        ** Fields
        *********/
        protected List<IMenuComponent> DrawOrder;
        protected List<IInteractiveMenuComponent> EventOrder;
        protected List<IMenuComponent> _StaticComponents = new List<IMenuComponent>();
        protected List<IInteractiveMenuComponent> _InteractiveComponents = new List<IInteractiveMenuComponent>();
        protected IInteractiveMenuComponent HoverElement;
        protected IInteractiveMenuComponent FocusElement;
        protected bool Hold;
        protected bool Center;


        /*********
        ** Accessors
        *********/
        public List<IMenuComponent> StaticComponents => new List<IMenuComponent>(this._StaticComponents);
        public List<IInteractiveMenuComponent> InteractiveComponents => new List<IInteractiveMenuComponent>(this._InteractiveComponents);

        public override bool Disabled
        {
            get => base.Disabled;
            set
            {
                base.Disabled = value;
                foreach (IInteractiveMenuComponent c in this._InteractiveComponents)
                {
                    if (c is BaseFormComponent component)
                        component.Disabled = value;
                }
            }
        }

        public Rectangle EventRegion => this.Area;

        public Rectangle ZoomEventRegion => new Rectangle(this.Area.X / Game1.pixelZoom, this.Area.Y / Game1.pixelZoom, this.Area.Width / Game1.pixelZoom, this.Area.Height / Game1.pixelZoom);


        /*********
        ** Public methods
        *********/
        public FormCollectionComponent(Point size, List<IMenuComponent> components = null)
            : this(components)
        {
            this.Center = true;
            this.SetScaledArea(new Rectangle(0, 0, size.X, size.Y));
        }

        public FormCollectionComponent(Rectangle area, List<IMenuComponent> components = null)
            : this(components)
        {
            this.SetScaledArea(area);
        }

        public override void OnAttach(IComponentContainer parent)
        {
            if (!this.Center)
                return;
            this.Area.X = (parent.EventRegion.Width - this.Area.Width) / 2;
            this.Area.Y = (parent.EventRegion.Height - this.Area.Height) / 2;
        }

        public FrameworkMenu GetAttachedMenu()
        {
            return this.Parent.GetAttachedMenu();
        }

        public void ResetFocus()
        {
            if (this.FocusElement == null)
                return;
            this.FocusElement.FocusLost();
            if (this.FocusElement is IKeyboardComponent)
            {
                Game1.keyboardDispatcher.Subscriber.Selected = false;
                Game1.keyboardDispatcher.Subscriber = null;
            }

            this.FocusElement = null;
        }

        public void GiveFocus(IInteractiveMenuComponent component)
        {
            if (!this._InteractiveComponents.Contains(component) || component == this.FocusElement)
                return;
            this.Parent.GiveFocus(this);
            this.ResetFocus();
            this.FocusElement = component;
            if (this.FocusElement is IKeyboardComponent keyboardComponent)
                Game1.keyboardDispatcher.Subscriber = new KeyboardSubscriberProxy(keyboardComponent);
            component.FocusGained();
        }

        public void AddComponent(IMenuComponent component)
        {
            if (component is IInteractiveMenuComponent menuComponent)
                this._InteractiveComponents.Add(menuComponent);
            else
                this._StaticComponents.Add(component);
            component.Attach(this);
            this.UpdateDrawOrder();
        }

        public void RemoveComponent(IMenuComponent component)
        {
            bool removed = false;
            this.RemoveComponents(a =>
            {
                bool b = a == component && !removed;
                if (b)
                {
                    removed = true;
                    a.Detach(this);
                }

                return b;
            });
        }

        public void RemoveComponents<T>() where T : IMenuComponent
        {
            this.RemoveComponents(a => a.GetType() == typeof(T));
        }

        public void RemoveComponents(Predicate<IMenuComponent> filter)
        {
            this._InteractiveComponents.RemoveAll(a =>
            {
                bool b = filter(a);
                if (b)
                    a.Detach(this);
                return b;
            });
            this._StaticComponents.RemoveAll(a =>
            {
                bool b = filter(a);
                if (b)
                    a.Detach(this);
                return b;
            });
            this.UpdateDrawOrder();
        }

        public void ClearComponents()
        {
            this._InteractiveComponents.TrueForAll(a =>
            {
                a.Detach(this);
                return true;
            });
            this._StaticComponents.TrueForAll(a =>
            {
                a.Detach(this);
                return true;
            });
            this._InteractiveComponents.Clear();
            this._StaticComponents.Clear();
            this.UpdateDrawOrder();
        }

        public bool AcceptsComponent(IMenuComponent component)
        {
            return true;
        }

        // IInteractiveMenuComponent
        public override void FocusLost()
        {
            this.ResetFocus();
        }

        public override void LeftUp(Point p, Point o)
        {
            if (!this.Visible)
                return;
            if (this.HoverElement == null)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            this.HoverElement.LeftUp(p, o2);
            this.Hold = false;
            if (this.HoverElement.InBounds(p, o2))
                return;
            this.HoverElement.HoverOut(p, o2);
            this.HoverElement = null;
        }

        public override void LeftHeld(Point p, Point o)
        {
            if (!this.Visible)
                return;
            if (this.HoverElement == null)
                return;
            this.Hold = true;
            this.HoverElement.LeftHeld(p, new Point(this.Area.X + o.X, this.Area.Y + o.Y));
        }

        public override void LeftClick(Point p, Point o)
        {
            if (!this.Visible)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o2))
                {
                    this.GiveFocus(el);
                    el.LeftClick(p, o2);
                    return;
                }
            }

            this.ResetFocus();
        }

        public override void RightClick(Point p, Point o)
        {
            if (!this.Visible)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o2))
                {
                    this.GiveFocus(el);
                    this.FocusElement = el;
                    el.RightClick(p, o2);
                    return;
                }
            }

            this.ResetFocus();
        }

        public override void HoverOver(Point p, Point o)
        {
            if (!this.Visible || this.Hold)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            if (this.HoverElement != null && !this.HoverElement.InBounds(p, o2))
            {
                this.HoverElement.HoverOut(p, o2);
                this.HoverElement = null;
            }

            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o2))
                {
                    if (this.HoverElement == null)
                    {
                        this.HoverElement = el;
                        el.HoverIn(p, o2);
                    }

                    el.HoverOver(p, o2);
                    break;
                }
            }
        }

        public override bool Scroll(int d, Point p, Point o)
        {
            if (!this.Visible)
                return false;
            foreach (IInteractiveMenuComponent el in this.EventOrder)
            {
                if (el.InBounds(p, o) && el.Scroll(d, p, o))
                    return true;
            }

            return false;
        }

        public override void Update(GameTime t)
        {
            if (!this.Visible)
                return;
            foreach (IMenuComponent el in this.DrawOrder)
                el.Update(t);
        }

        public override void Draw(SpriteBatch b, Point o)
        {
            if (!this.Visible)
                return;
            Point o2 = new Point(this.Area.X + o.X, this.Area.Y + o.Y);
            foreach (IMenuComponent el in this.DrawOrder)
                el.Draw(b, o2);
        }


        /*********
        ** Protected methods
        *********/
        protected FormCollectionComponent() { }

        protected FormCollectionComponent(List<IMenuComponent> components = null)
        {
            if (components != null)
            {
                foreach (IMenuComponent c in components)
                    this.AddComponent(c);
            }
        }

        // IComponentCollection
        protected void UpdateDrawOrder()
        {
            KeyValuePair<List<IInteractiveMenuComponent>, List<IMenuComponent>> sorted = FrameworkMenu.GetOrderedLists(this._StaticComponents, this._InteractiveComponents);
            this.DrawOrder = sorted.Value;
            this.EventOrder = sorted.Key;
        }
    }
}
