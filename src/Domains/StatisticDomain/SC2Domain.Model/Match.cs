using System;
using System.Collections.Generic;

using SC2LiquipediaStatistics.Utilities.Domain;

namespace SC2Statistics.SC2Domain.Model
{
    public class Match : EntityBase
    {
        public virtual long Id { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual Event Event { get; set; }
        public virtual Player Player1 { get; set; }
        public virtual Player Player2 { get; set; }
        public virtual int Player1Score { get; set; }
        public virtual int Player2Score { get; set; }
        public virtual Race Player1Race { get; set; }
        public virtual Race Player2Race { get; set; }
        public virtual string AligulacReference { get; set; }
        public virtual Expansion Expansion { get; set; }

        public Match()
        {
            Expansion = Expansion.Undefined;
        }

        public virtual Player Winner 
        {
            get
            {
                if (Player1Score > Player2Score)
                    return Player1;
                if (Player1Score < Player2Score)
                    return Player2;

                return null;
            }
        }

        public override string ToString()
        {
            return $"{Player1.Tag} {Player1Score} x {Player2Score} {Player2.Tag}";
        }
    }
}
