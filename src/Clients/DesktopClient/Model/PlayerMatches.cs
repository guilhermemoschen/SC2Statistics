using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class PlayerMatches : ObservableObject
    {
        private Match match;
        public Match Match
        {
            get
            {
                return match;
            }
            set
            {
                if (match == value)
                    return;

                Set(() => Match, ref match, value);
            }
        }

        private ObservableCollection<PlayerMatches> nextMatches;
        public ObservableCollection<PlayerMatches> NextMatches
        {
            get
            {
                return nextMatches;
            }
            set
            {
                if (nextMatches == value)
                    return;

                Set(() => NextMatches, ref nextMatches, value);
            }
        }

        public Player TargetPlayer { get; set; }

        public bool IsTargetPlayerWinner
        {
            get
            {
                if (Match == null || TargetPlayer == null)
                    return false;

                return Match.Winner.AligulacId == TargetPlayer.AligulacId;
            }
        }

        public PlayerMatches()
        {
            NextMatches = new ObservableCollection<PlayerMatches>();
        }
    }
}
