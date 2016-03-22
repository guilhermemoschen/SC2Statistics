using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2Statistics.SC2Domain.Model
{
    public enum BracketRound
    {
        Undefined,
        RoundOf128,
        RoundOf64,
        RoundOf32,
        RoundOf16,
        RoundOf8,
        RoundOf4,
        WinnersFinals,
        LosersFinals,
        GrandFinals,
        LoeserRound1,
        LoeserRound2,
        LoeserRound3,
        LoeserRound4,
        LoeserRound5,
        LoeserRound6,
        LoeserRound7,
        LoeserRound8,
        LoeserRound9,
        LoeserRound10,
        LoserSemifinals,
        FromWinnerBracket,
        FromLoserBracket,
    }
}
