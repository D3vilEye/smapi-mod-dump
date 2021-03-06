/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/demiacle/QualityOfLifeMods
**
*************************************************/

using Microsoft.Xna.Framework.Input;

namespace Demiacle.ImprovedQualityOfLife {
    public class ModConfig {

        // Configurable keys
        public string summonHorseKey { get; set; } = Keys.Z.ToString();
        public string waitOneHourKey { get; set; } = Keys.V.ToString();
        public string alterTenMinuteKey { get; set; } = Keys.G.ToString();

        // Enables or disables mod
        public bool enableAlterTenMinute { get; set; } = true;
        public bool enableFasterSpeedOnRoad { get; set; } = true;
        public bool enableHorsePassThroughSingleTiles { get; set; } = true;
        public bool enableAutoOpenGate { get; set; } = true;
        public bool showFishBeforeCaught { get; set; } = true;
        public bool enableSummonHorseAnywhere { get; set; } = true;
        public bool enableGrassDropsWithoutSilo { get; set; } = true;
        public bool enableQuickFishing { get; set; } = true;
        public bool enableFastForwardHourOnKeyPress { get; set; } = true;
        public bool showToolInventory { get; set; } = true;
    }
}