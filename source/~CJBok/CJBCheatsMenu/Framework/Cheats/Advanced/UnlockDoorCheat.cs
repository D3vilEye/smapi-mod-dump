/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using CJBCheatsMenu.Framework.Components;
using StardewValley;
using StardewValley.Menus;

namespace CJBCheatsMenu.Framework.Cheats.Advanced
{
    /// <summary>A cheat which unlocks a locked door.</summary>
    internal class UnlockDoorCheat : BaseCheat
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(CheatContext context)
        {
            // adventurer's guild
            yield return new CheatsOptionsCheckbox(
                label: I18n.Flags_Unlocked_Guild(),
                value: this.HasFlag("guildMember"),
                setValue: value => this.SetFlag(value, "guildMember")
            );

            // NPC rooms
            foreach (NPC npc in this.GetSocialCharacters().OrderBy(p => p.displayName))
            {
                yield return new CheatsOptionsCheckbox(
                    label: I18n.Flags_Unlocked_Room(name: npc.displayName),
                    value: this.HasFlag($"doorUnlock{npc.Name}"),
                    setValue: value => this.SetFlag(value, $"doorUnlock{npc.Name}")
                );
            }
        }
    }
}
