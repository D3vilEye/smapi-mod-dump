/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Magic
**
*************************************************/

using SpaceShared;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic
{
    public interface IApi
    {
        event EventHandler OnAnalyzeCast;
    }

    public class Api : IApi
    {
        public event EventHandler OnAnalyzeCast;
        internal void InvokeOnAnalyzeCast(Farmer farmer)
        {
            Log.trace("Event: OnAnalyzeCast");
            if (OnAnalyzeCast == null)
                return;
            Util.invokeEvent("Magic.Api.OnAnalyzeCast", OnAnalyzeCast.GetInvocationList(), farmer);
        }
    }
}
