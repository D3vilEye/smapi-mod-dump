/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ConvenientChests {
    public abstract class Module {
        public bool     IsActive { get; protected set; } = false;
        public ModEntry ModEntry { get; }
        public Config   Config   => ModEntry.Config;
        public IMonitor Monitor  => ModEntry.Monitor;
        public IModEvents Events => ModEntry.Helper.Events;

        public Module(ModEntry modEntry) => ModEntry = modEntry;

        public abstract void Activate();
        public abstract void Deactivate();
    }
}
