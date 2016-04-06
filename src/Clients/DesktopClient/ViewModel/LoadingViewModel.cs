using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using SC2LiquipediaStatistics.DesktopClient.Common;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class LoadingViewModel : ViewModelBase
    {
        private string messages;
        public string Messages
        {
            get
            {
                return messages;
            }
            set
            {
                if (value == null || messages == value)
                    return;

                Set(() => Messages, ref messages, value, true);
            }
        }

        public LoadingViewModel()
        {
            Messenger.Default.Register<LogMessage>(this, AddLogMessage);
            LoadedCommand = new RelayCommand(OnLoad);
        }

        private void OnLoad()
        {
            Messages = string.Empty;
        }

        private void AddLogMessage(LogMessage logMessage)
        {
            Messages = logMessage.Info + Environment.NewLine + Messages;
        }
    }
}
