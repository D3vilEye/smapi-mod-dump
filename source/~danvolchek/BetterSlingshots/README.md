**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/danvolchek/StardewMods**

----

# Better Slingshots


See [This link](http://www.nexusmods.com/stardewvalley/mods/2067?) for the NexusMods mod page, which has a description, screenshots, and a download of the built mod.

## How it works

The mod works by Harmony patching various `Slingshot` methods to achieve the desired features. Unlike the old version of the mod, this completely rewritten approach is multiplayer compatible because it doesn't swap out `Slingshot` instances as the player uses them (which lead to a ton of errors).