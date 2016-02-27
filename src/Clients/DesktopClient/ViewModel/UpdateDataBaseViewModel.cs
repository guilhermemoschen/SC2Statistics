using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.LiquipediaDomain.Service;
using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class UpdateDataBaseViewModel : ViewModelBase
    {
        protected string message;
        public string Message
        {
            get
            {
                return message;
                
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                Set(() => Message, ref message, value, true);
            }
        }

        public ILiquipediaService LiquipediaService { get; protected set; }

        public IEventsListService EventsListService { get; protected set; }


        public UpdateDataBaseViewModel(ILiquipediaService liquipediaService, IEventsListService eventsListService)
        {
            LiquipediaService = liquipediaService;
            EventsListService = eventsListService;

            Initialize();
        }

        public UpdateDataBaseViewModel()
        {
            if (IsInDesignMode)
                return;

            LiquipediaService = Container.Resolve<ILiquipediaService>();
            EventsListService = Container.Resolve<IEventsListService>();

            Initialize();
        }

        private void Initialize()
        {
            Message = string.Empty;
            Messenger.Default.Register<LogMessage>(this, AddMessage);
        }

        private void AddMessage(LogMessage message)
        {
            Message += string.Format("{0}{1}", message.Info, Environment.NewLine);
        }

        public async void OnLoad()
        {
            if (IsInDesignMode)
                return;

            var events = await EventsListService.GetEventItems();

            foreach (var eventUrl in events.Where(x => x.Selected).Select(x => x.URL))
            {
                var newEvent = await LiquipediaService.GetEvent(eventUrl);
            }
        }
    }
}
