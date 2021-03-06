/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AbbyNode/StardewValley-CoalRegen
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoalRegen {
	public class ModEntry : Mod {
		private MineShaft ms;

		public override void Entry(IModHelper helper) {
			// ModConfig config = helper.ReadConfig<ModConfig>();

			LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;
			MineEvents.MineLevelChanged += MineEvents_MineLEvelChanged;
		}

		public void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e) {
			if (e.NewLocation is MineShaft) {
				ms = e.NewLocation as MineShaft;
			}
		}

		public void MineEvents_MineLEvelChanged(object sender, EventArgsMineLevelChanged e) {
			if (ms.permanentMineChanges.ContainsKey(ms.mineLevel) && ms.permanentMineChanges[ms.mineLevel].coalCartsLeft <= 0) {
				ms.updateMineLevelData(2, 1);
			}
		}
	}
}