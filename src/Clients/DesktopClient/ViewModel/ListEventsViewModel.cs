using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

using AutoMapper;

using FirstFloor.ModernUI.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.Utilities.DataBase;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.StatisticDomain.Service;

using DomainEntities = SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class ListEventsViewModel : ModernViewModelBase
    {
        private ObservableCollection<Event> events;
        public ObservableCollection<Event> Events
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

        private Event selectedEvent;

        public Event SelectedEvent
        {
            get
            {
                return selectedEvent;
            }
            set
            {
                if (selectedEvent == value || value == null)
                    return;

                Set(() => SelectedEvent, ref selectedEvent, value, true);
            }
        }

        public ICommand EditEventCommand { get; private set; }
        public ICommand DeleteEventCommand { get; private set; }

        public IStatisticService StatisticService { get; private set; }

        public IModernNavigationService NavigationService { get; private set; }

        public IMapper Mapper { get; protected set; }

        public ListEventsViewModel(IStatisticService statisticService, IModernNavigationService navigationService, IMapper mapper)
        {
            StatisticService = statisticService;
            NavigationService = navigationService;
            Mapper = mapper;

            EditEventCommand = new RelayCommand(NavigateToEditEvent);
            NavigatedToCommand = new RelayCommand<object>(LoadGrid);
        }

        private void NavigateToEditEvent()
        {
            NavigationService.NavigateTo(ViewLocator.EditEventView, SelectedEvent);
        }

        public void LoadGrid(object parameter)
        {
            //IList<Event> events;

            //using (new NHibernateSessionContext())
            //{
            //    var domainEvents = SC2Service.FindMainEvents()
            //        .OrderByDescending(x => x.EndDate)
            //        .ToList();
            //    events = Mapper.Map<IList<DomainEntities.Event>, IList<Event>>(domainEvents);
            //}

            //Events = new ObservableCollection<Event>(events);
        }
    }
}
