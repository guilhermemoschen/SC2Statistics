using System;
using System.Collections.Generic;

namespace SC2Statistics.SC2Domain.Model
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
        public virtual BracketRound BracketRound { get; set; }
        public virtual MatchFormat Format { get; set; }
        public virtual Match NextMatch { get; set; }
        public virtual string GroupName { get; set; }

        public Match()
        {
            Games = new List<Game>();
            // Only 1x1 is available right now
            Type = MatchType.OneXOne;
            BracketRound = BracketRound.Undefined;
            Format = MatchFormat.Undefined;
        }

        private static int ParsePlayerScore(string score)
        {
            int playerScore;
            if (int.TryParse(score, out playerScore))
            {
                return playerScore;
            }

            if (score.Equals("w", StringComparison.CurrentCultureIgnoreCase))
                return 1;

            return 0;
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

        public override string ToString()
        {
            return string.Format(
                "{0} {1} x {2} {3}",
                (Player1 != null) ? Player1.Name : "NULL",
                Player1Score,
                Player2Score,
                (Player2 != null) ? Player2.Name : "NULL"
            );
        }

        public void AddGame(Game game)
        {
            if (Games.Contains(game))
                return;

            game.Match = this;
            Games.Add(game);
        }

        public static Match CreateBracketMatch(DateTime? date, Player player1, string player1Score, Race player1Race, Player player2, string player2Score, Race player2Race, BracketRound round, IEnumerable<Game> games)
        {
            var match = new Match()
            {
                Date = null,
                Player1 = player1,
                Player1Race = player1Race,
                Player1Score = ParsePlayerScore(player1Score),
                Player2 = player2,
                Player2Score = ParsePlayerScore(player2Score),
                Player2Race = player2Race,
                Format = MatchFormat.Bracket,
                BracketRound = round,
            };

            if (games != null)
            {
                foreach (var game in games)
                {
                    match.AddGame(game);
                }
            }

            match.DefineWinner();

            return match;
        }

        public static Match CreateGroupMatch(string groupName, DateTime? date, Player player1, string player1Score, Race player1Race, Player player2, string player2Score, Race player2Race, IEnumerable<Game> games)
        {
            var match = new Match()
            {
                GroupName = groupName,
                Date = null,
                Player1 = player1,
                Player1Race = player1Race,
                Player1Score = ParsePlayerScore(player1Score),
                Player2 = player2,
                Player2Score = ParsePlayerScore(player2Score),
                Player2Race = player2Race,
                Format = MatchFormat.Group,
            };

            if (games != null)
            {
                foreach (var game in games)
                {
                    match.AddGame(game);
                }
            }

            match.DefineWinner();

            return match;
        }
    }
}
