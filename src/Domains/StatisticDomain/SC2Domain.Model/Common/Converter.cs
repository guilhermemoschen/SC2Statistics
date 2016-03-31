using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Common
{
    /// <summary>
    /// Domain converter helper.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Convert a string to a Race.
        /// </summary>
        /// <param name="s">string</param>
        public static Race ToRace(string s)
        {
            if (string.IsNullOrEmpty(s))
                return Race.Undefined;

            if (s.Equals("p", StringComparison.InvariantCultureIgnoreCase) || s.Equals("protoss", StringComparison.InvariantCultureIgnoreCase))
                return Race.Protoss;

            if (s.Equals("t", StringComparison.InvariantCultureIgnoreCase) || s.Equals("terran", StringComparison.InvariantCultureIgnoreCase))
                return Race.Terran;

            if (s.Equals("z", StringComparison.InvariantCultureIgnoreCase) || s.Equals("zerg", StringComparison.InvariantCultureIgnoreCase))
                return Race.Zerg;

            if (s.Equals("r", StringComparison.InvariantCultureIgnoreCase) || s.Equals("random", StringComparison.InvariantCultureIgnoreCase))
                return Race.Random;

            return Race.Undefined;
        }

        public static Expansion ToExpansion(string s)
        {
            if (string.IsNullOrEmpty(s))
                return Expansion.Undefined;

            if (s.Equals("lotv", StringComparison.InvariantCultureIgnoreCase))
                return Expansion.LegacyOfTheVoid;

            if (s.Equals("hots", StringComparison.InvariantCultureIgnoreCase))
                return Expansion.HeartOfTheSwarm;

            if (s.Equals("wol", StringComparison.InvariantCultureIgnoreCase))
                return Expansion.WingsOfLiberty;

            return Expansion.Undefined;
        }
    }
}
