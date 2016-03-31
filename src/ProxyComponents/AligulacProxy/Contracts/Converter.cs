using System;

using SC2LiquipediaStatistics.Utilities.Domain;

using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.Proxy.Aligulac.Contracts
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
        public static string ExpansionToString(Expansion expansion)
        {
            if (expansion == Expansion.Undefined)
                throw new ValidationException("Undefined SC2 expansion.");

            switch (expansion)
            {
                case Expansion.WingsOfLiberty:
                    return "WoL";
                case Expansion.HeartOfTheSwarm:
                    return "HotS";
                case Expansion.LegacyOfTheVoid:
                    return "LotV";
            }

            throw new ValidationException("Unsupported expansion.");
        }
    }
}
