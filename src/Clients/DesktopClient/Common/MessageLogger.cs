using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Messaging;

using SC2LiquipediaStatistics.Utilities.Log;

namespace SC2LiquipediaStatistics.DesktopClient.Common
{
    public class MessageLogger : ILogger
    {
        public void Info(string text)
        {
            Messenger.Default.Send(new LogMessage() { Info = text});
        }

        public void Info(string text, params object[] args)
        {
            Messenger.Default.Send(new LogMessage() { Info = string.Format(text, args) });
        }
    }
}
