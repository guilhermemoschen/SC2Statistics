using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2LiquipediaStatistics.Utilities.Log;

namespace SC2LiquipediaStatistics.ImportTool.Log
{
    public class ConsoleLogger : ILogger
    {
        public void Info(string text)
        {
            Console.WriteLine(text);
        }

        public void Info(string text, params object[] args)
        {
            Console.WriteLine(text, args);
        }
    }
}
