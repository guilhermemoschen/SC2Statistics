using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class EventViewModel : ViewModelBase
    {
        private ObservableCollection<EventItem> events;
        public ObservableCollection<EventItem> Events
        {
            get
            {
                return events;
            }
            set
            {
                if (value == null)
                    return;

                Set(() => Events, ref events, value, true);
            }
        }

        public IEventsListService EventsListService { get; protected set; }

        public EventViewModel(IEventsListService eventsListService)
        {
            EventsListService = eventsListService;

            Initialize();
        }

        public EventViewModel()
        {
            if (IsInDesignMode)
                return;

            EventsListService = Container.Resolve<IEventsListService>();

            Initialize();
        }

        private void Initialize()
        {
            Events = new ObservableCollection<EventItem>();
        }

        public async void OnLoad()
        {
            if (IsInDesignMode)
                return;

            var eventsItems = await EventsListService.GetEventItems();
            Events = new ObservableCollection<EventItem>(eventsItems);
        }
    }
}
