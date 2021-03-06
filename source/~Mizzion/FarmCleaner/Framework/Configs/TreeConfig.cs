/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace FarmCleaner.Framework.Configs
{
    internal class TreeConfig
    {
        public bool ClearTrees { get; set; } = true; //Whether or not to clear any trees
        public bool LeaveRandomTrees { get; set; } = true; //Whether or not to randomize tree selection
        public int RandomTreeChance { get; set; } = 75;
        public bool ClearSeeds { get; set; } = false; // seedStage = 0;
        public bool ClearSprout { get; set; } = false; // sproutStage = 1;
        public bool ClearSapling { get; set; } = false; // saplingStage = 2;
        public bool ClearBush { get; set; } = true; // bushStage = 3;
        public bool ClearSmallTree { get; set; } = true; //treeStage = 4;
        public bool ClearFullTree { get; set; } = true; //treeStage = 5;
    }
}
