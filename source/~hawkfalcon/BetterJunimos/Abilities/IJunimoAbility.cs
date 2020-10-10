/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;

namespace BetterJunimos.Abilities {
    /* 
     * Provides abilities for Junimos 
     */    
    public interface IJunimoAbility {
        /*
         * What is the name of this ability 
         */
        String AbilityName();

        /*
         * Is the action available at the position? E.g. is the crop ready to harvest
         */
        bool IsActionAvailable(Farm farm, Vector2 pos);

        /*
         * Action to take if it is available, return false if action failed
         */
        bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Chest chest);

        /*
         * Does this action require an item (SObject.SeedsCategory, etc)?
         * Return 0 if no item needed        
         */
        int RequiredItem();
    }
}
