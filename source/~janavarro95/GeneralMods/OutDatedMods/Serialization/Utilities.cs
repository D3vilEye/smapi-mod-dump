/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace Serialization
{
    class Utilities
    {

        public static Microsoft.Xna.Framework.Rectangle parseRectFromJson(string s)
        {



            s = s.Replace('{', ' ');
            s = s.Replace('}', ' ');
            s = s.Replace('^', ' ');
            s = s.Replace(':', ' ');
            string[] parsed = s.Split(' ');
            foreach (var v in parsed)
            {
                //Log.AsyncY(v);
            }
            return new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(parsed[2]), Convert.ToInt32(parsed[4]), Convert.ToInt32(parsed[6]), Convert.ToInt32(parsed[8]));
        }

    }
}
