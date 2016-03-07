using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using AutoMapper;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;

using SC2Statistics.SC2Domain.Service;

using SC2DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class EditEventViewModel : ViewModelBase
    {
        public ISC2Service SC2Service { get; private set; }

        public IModernNavigationService NavigationService { get; private set; }

        public IMapper Mapper { get; protected set; }

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

        public IEnumerable<SC2DomainEntities.Expansion> Expansions
        {
            get
            {
                return Enum.GetValues(typeof(SC2DomainEntities.Expansion))
                    .Cast<SC2DomainEntities.Expansion>();
            }
        }

        public IEnumerable<SC2DomainEntities.LiquipediaTier> LiquipediaTiers
        {
            get
            {
                return Enum.GetValues(typeof(SC2DomainEntities.LiquipediaTier))
                    .Cast<SC2DomainEntities.LiquipediaTier>();
            }
        }

        public ICommand SaveCommand { get; private set; }

        public EditEventViewModel(ISC2Service sc2Service, IModernNavigationService navigationService, IMapper mapper)
        {
            SC2Service = sc2Service;
            NavigationService = navigationService;
            Mapper = mapper;
            SaveCommand = new RelayCommand(SaveEvent);
        }

        private void SaveEvent()
        {
            var domainEvent = SC2Service.LoadEvent(SelectedEvent.Id);
            domainEvent = Mapper.Map(SelectedEvent, domainEvent);
            SC2Service.UpdateEvent(domainEvent);
            NavigationService.NavigateTo(ViewLocator.ListEventsView);
        }

        public void View_OnLoad()
        {
            SelectedEvent = NavigationService.Parameter as Event;
        }
    }
}
