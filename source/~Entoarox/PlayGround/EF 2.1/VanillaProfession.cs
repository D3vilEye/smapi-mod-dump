/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SFarmer = StardewValley.Farmer;
using StardewValley.Menus;

namespace Entoarox.Framework.Core.Skills
{
    [Obsolete("Currently in development",true)]
    class VanillaProfession : IProfession
    {
        private string _Name;
        private string _Parent;
        private int _Level;
        public VanillaProfession(string name, string parent, int level)
        {
            this._Name = name;
            this._Parent = parent;
            this._Level = level;
        }
        public int SkillLevel => this._Level;

        public string Parent => this._Parent;

        public string Name => this._Name;

        public string DisplayName => LevelUpMenu.getProfessionTitleFromNumber(SkillHelper._VanillaProfessions[this._Name]);

        public string Description => string.Join("\n", LevelUpMenu.getProfessionDescription(SkillHelper._VanillaProfessions[this._Name]));

        public Texture2D Icon => throw new NotImplementedException();
    }
}
