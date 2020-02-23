﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Magic.Schools;
using StardewValley;
using SpaceShared;

namespace Magic.Spells
{
    public abstract class Spell
    {
        public string ParentSchoolId { get; }
        public School ParentSchool { get { return School.getSchool(ParentSchoolId); } }
        public string Id { get; }
        public string FullId { get { return ParentSchoolId + ":" + Id; } }
        public Texture2D[] Icons
        {
            get;
            protected set;
        }

        protected Spell(string school, string id )
        {
            ParentSchoolId = school;
            Id = id;
        }

        public virtual int getMaxCastingLevel()
        {
            return 3;
        }

        public abstract int getManaCost(Farmer player, int level);

        public virtual bool canCast(Farmer player, int level)
        {
            return player.knowsSpell(FullId, level) && player.getCurrentMana() >= getManaCost( player, level );
        }

        public virtual string getTranslatedName()
        {
            return Mod.instance.Helper.Translation.Get("spell." + FullId + ".name");
        }
        public virtual string getTranslatedDescription()
        {
            return Mod.instance.Helper.Translation.Get("spell." + FullId + ".desc");
        }
        
        public abstract IActiveEffect onCast(Farmer player, int level, int targetX, int targetY);

        public virtual void loadIcon()
        {
            try
            {
                Icons = new Texture2D[getMaxCastingLevel()];
                for (int i = 1; i <= getMaxCastingLevel(); ++i)
                {
                    Icons[ i - 1 ] = Content.loadTexture("magic/" + ParentSchool.Id + "/" + Id + "/" + i + ".png");
                }
            }
            catch ( ContentLoadException e )
            {
                Log.warn("Failed to load icon for spell " + FullId + ": " + e);
            }
        }
    }
}
