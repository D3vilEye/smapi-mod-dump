/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace ProducerFrameworkMod.ContentPack
{
    public class ContentPackConfig
    {

        private LogLevel _defaultWarningsLogLevel = LogLevel.Warn;
        public LogLevel DefaultWarningsLogLevel
        {
            get => _defaultWarningsLogLevel;
            set => _defaultWarningsLogLevel = value < LogLevel.Warn ? value : LogLevel.Warn;
        }
    }
}
