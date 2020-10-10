/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public interface IComponentContainer
    {
        /*********
        ** Accessors
        *********/
#pragma warning disable CS0618 // Type or member is obsolete
        /// <summary>The menu this container is a part of.</summary>
        /// <exception cref="System.NullReferenceException">Container is not attached to a menu.</exception>
        InterfaceMenu Menu { get; }
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>The outer bounds of the container, scaled using <see cref="StardewValley.Game1.pixelZoom" />. These bounds should be relative to the viewport.</summary>
        Rectangle OuterBounds { get; }

        /// <summary>The inner bounds of the container, scaled using <see cref="StardewValley.Game1.pixelZoom" />. These bounds should be relative to the outer bounds. At the same time, they represent the drawable region for any contained components.</summary>
        Rectangle InnerBounds { get; }

        /// <summary>If the given component has focus, returns false for components that are not owned by this container.</summary>
        IDynamicComponent FocusComponent { get; }


        /*********
        ** Methods
        *********/
        /// <summary>Attempt to switch Focus to the next element in the container. If false is returned, then the parent should switch its focus instead.</summary>
        /// <returns>If further tabbing inside this container is possible.</returns>
        bool TabNext();

        /// <summary>Attempt to switch Focus to the previous element in the container. If false is returned, then the parent should switch its focus instead.</summary>
        /// <returns>If further tabbing inside this container is possible.</returns>
        bool TabBack();

        /// <summary>If the given component has focus, returns false for components that are not owned by this container.</summary>
        /// <param name="component">The component to check for focus.</param>
        /// <returns>If the component currently has focus.</returns>
        bool HasFocus(IDynamicComponent component);
    }
}
