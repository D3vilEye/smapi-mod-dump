/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/RushOrders
**
*************************************************/

using SpaceShared;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RushOrders
{
    public interface IApi
    {
        event EventHandler<Tool> ToolRushed;
        event EventHandler BuildingRushed;
    }

    public class Api : IApi
    {
        public event EventHandler<Tool> ToolRushed;
        internal void InvokeToolRushed(Tool tool)
        {
            Log.trace("Event: ToolRushed");
            if (ToolRushed == null)
                return;
            Util.invokeEvent("RushOrders.Api.ToolRushed", ToolRushed.GetInvocationList(), null, tool);
        }

        public event EventHandler BuildingRushed;
        internal void InvokeBuildingRushed()
        {
            Log.trace("Event: BuildingRushed");
            if (BuildingRushed == null)
                return;
            Util.invokeEvent("RushOrders.Api.BuildingRushed", BuildingRushed.GetInvocationList(), null);
        }
}
}
