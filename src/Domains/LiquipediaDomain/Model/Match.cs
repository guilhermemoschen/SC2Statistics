using System;
using System.Collections.Generic;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Model
{
    public class Match
    {
        public virtual long Id { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual Event Event { get; set; }
        public virtual IList<Game> Games { get; set; }
        public virtual Player Player1 { get; set; }
        public virtual Player Player2 { get; set; }
        public virtual Player Winner { get; set; }
        public virtual int Player1Score { get; set; }
        public virtual int Player2Score { get; set; }
        public virtual Race Player1Race { get; set; }
        public virtual Race Player2Race { get; set; }
        public virtual MatchType Type { get; set; }
        public virtual string LiquipediaReference { get; set; }

        public Match() { }

        public Match(Event eventPlayed, DateTime? date, Player player1, string player1Score, Race player1Race, Player player2, string player2Score, Race player2Race)
        {
            Event = eventPlayed;
            Date = date;
            Player1 = player1;
            Player1Race = player1Race;

            Player2 = player2;
            Player2Race = player2Race;

            int score1;
            if (int.TryParse(player1Score, out score1))
            {
                Player1Score = score1;
            }
            else if (player1Score.Equals("w", StringComparison.CurrentCultureIgnoreCase))
                Player1Score = 1;


            int score2;
            if (int.TryParse(player2Score, out score2))
            {
                Player2Score = score2;
            }
            else if (player2Score.Equals("w", StringComparison.CurrentCultureIgnoreCase))
                Player2Score = 1;

            DefineWinner();

            Games = new List<Game>();
            // Only 1x1 is available right now
            Type = MatchType.OneXOne;
        }

        public void DefineWinner()
        {
            if (Player1Score > Player2Score)
                Winner = Player1;
            else if (Player2Score > Player1Score)
                Winner = Player2;
            else
                Winner = null;
        }
    }
}
