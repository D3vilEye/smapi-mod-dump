/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.ExecutionCore.TaskPrerequisites
{
    public class LocationPrerequisite:GenericPrerequisite 
    {
        public GameLocation location;

        public LocationPrerequisite(GameLocation Location)
        {
            this.location = Location;
        }

        public bool isPlayerAtLocation()
        {
            if (this.location == null)
            {
                ModCore.CoreMonitor.Log("Location calculation is null");
                return true;
            }
            else
            {
                ModCore.CoreMonitor.Log("THIS IS THE LOCATION"+this.location);

            }
            if (Game1.player.currentLocation == this.location || Game1.player.currentLocation.name == this.location.name) return true;
            else return false;
        }

        public override bool checkAllPrerequisites()
        {
            if (this.location == null) return true;
            if (isPlayerAtLocation()) return true;
            return false;
        }

    }
}
