/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cdaragorn/Ui-Info-Suite
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIInfoSuite.Extensions
{
    static class StringExtensions
    {


        public static Int32 SafeParseInt32(this String s)
        {
            Int32 result = 0;

            if (!String.IsNullOrWhiteSpace(s))
            {
                Int32.TryParse(s, out result);
            }

            return result;
        }

        public static Int64 SafeParseInt64(this String s)
        {
            Int64 result = 0;

            if (!String.IsNullOrWhiteSpace(s))
                Int64.TryParse(s, out result);

            return result;
        }

        public static bool SafeParseBool(this String s)
        {
            bool result = false;

            if (!String.IsNullOrWhiteSpace(s))
            {
                Boolean.TryParse(s, out result);
            }

            return result;
        }
    }
}
