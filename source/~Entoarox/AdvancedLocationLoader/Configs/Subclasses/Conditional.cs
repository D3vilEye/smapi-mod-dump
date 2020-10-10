/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.ComponentModel;
using Newtonsoft.Json;

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Conditional
    {
#pragma warning disable CS0649
        /*********
        ** Accessors
        *********/
        public int Amount;
        public int Item;
        public string Name;
        public string Question;

        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string Success;
#pragma warning restore CS0649


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Conditional({this.Name}[{this.Item}{':'}{this.Amount}] = `{this.Question}`)";
        }
    }
}
