/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCraftingStation.src
{
    public class Config
    {
        public bool GlobalCraftFromChest { get; set; } = false;
        public bool CraftFromFridgeWhenInHouse { get; set; } = true;
        public int CraftingFromChestsRadius { get; set; } = 0;
    }
}
