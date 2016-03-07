using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using SC2LiquipediaStatistics.DesktopClient.Common;

using SC2Statistics.SC2Domain.Service;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class AddEventViewModel : ViewModelBase
    {
        public ISC2Service SC2Service { get; protected set; }

        public ICommand AddNewCommand { get; protected set; }

        private string eventUrl;
        public string EventUrl
        {
            get
            {
                return eventUrl;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || eventUrl == value)
                    return;

                Set(() => EventUrl, ref eventUrl, value, true);
            }
        }

        private string logMessage;
        public string LogMessage
        {
            get
            {
                return logMessage;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || logMessage == value)
                    return;

                Set(() => LogMessage, ref logMessage, value, true);
            }
        }

        public AddEventViewModel(ISC2Service sc2Service)
        {
            SC2Service = sc2Service;

            AddNewCommand = new RelayCommand(AddNewEvent);
            EventUrl = "http://wiki.teamliquid.net/starcraft2/Ting_Open";

            Messenger.Default.Register<LogMessage>(this, AddLogMessage);
        }

        private void AddLogMessage(LogMessage message)
        {
            LogMessage = message.Info + Environment.NewLine + LogMessage;
        }

        private async void AddNewEvent()
        {
            var sc2Event = await SC2Service.ParseEvent(EventUrl);
            SC2Service.CreateEvent(sc2Event);
            AddLogMessage(new LogMessage() { Info = "Finished!" });
        }
    }
}
