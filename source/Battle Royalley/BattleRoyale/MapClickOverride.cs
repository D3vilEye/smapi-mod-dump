/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace BattleRoyale
{
	class MapClickOverride : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(MapPage), "receiveLeftClick");

		public static bool Prefix(MapPage __instance, int x, int y)
		{
			foreach (var point in __instance.points)
			{
				if (point.containsPoint(x, y))
				{
					bool r = SpectatorMode.OnClickMapPoint(point);
					if (!r)
						Game1.activeClickableMenu.exitThisMenu(false);
					return r;
				}
			}

			return true;
		}
	}
}
