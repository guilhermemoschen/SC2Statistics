using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2LiquipediaStatistics.Utilities.Log
{
    public interface ILogger
    {
        void Info(string text);
        void Info(string text, params object[] args);
    }
}
