using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using AutoMapper;

using FirstFloor.ModernUI.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.Utilities.DataBase;
using SC2LiquipediaStatistics.Utilities.Domain;

using SC2Statistics.StatisticDomain.Service;

using SC2DomainEntities = SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class EditEventViewModel : ModernViewModelBase
    {
        public IStatisticService StatisticService { get; private set; }

        public IModernNavigationService NavigationService { get; private set; }

        public IMapper Mapper { get; protected set; }

        public ILoadingService LoadingService { get; protected set; }

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

        private SubEvent selectedSubEvent;
        public SubEvent SelectedSubEvent
        {
            get
            {
                return selectedSubEvent;
            }
            set
            {
                if (selectedSubEvent == value || value == null)
                    return;

                Set(() => SelectedSubEvent, ref selectedSubEvent, value, true);
            }
        }

        public EventInput subEventInput;
        public EventInput SubEventInput
        {
            get
            {
                return subEventInput;
            }
            set
            {
                if (subEventInput == value || value == null)
                    return;

                Set(() => SubEventInput, ref subEventInput, value, true);
            }
        }

        public IList<KeyValuePair<string, SC2DomainEntities.Expansion>> Expansions { get; private set; }

        protected KeyValuePair<string, SC2DomainEntities.Expansion> selectedExpansion;
        public KeyValuePair<string, SC2DomainEntities.Expansion> SelectedExpansion
        {
            get
            {
                return selectedExpansion;
            }
            set
            {
                if (selectedExpansion.Value == value.Value)
                    return;

                Set(() => SelectedExpansion, ref selectedExpansion, value, true);
            }
        }

        public IEnumerable<SC2DomainEntities.Tier> LiquipediaTiers
        {
            get
            {
                return Enum.GetValues(typeof(SC2DomainEntities.Tier))
                    .Cast<SC2DomainEntities.Tier>();
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand EditSubEventCommand { get; private set; }

        public EditEventViewModel(IStatisticService statisticService, IModernNavigationService navigationService, ILoadingService loadingService, IMapper mapper)
        {
            StatisticService = statisticService;
            NavigationService = navigationService;
            LoadingService = loadingService;
            Mapper = mapper;

            Expansions = new List<KeyValuePair<string, SC2DomainEntities.Expansion>>();
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Hearth of the Swarm", SC2DomainEntities.Expansion.HeartOfTheSwarm));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Legacy of the Void", SC2DomainEntities.Expansion.LegacyOfTheVoid));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Wings of Liberty", SC2DomainEntities.Expansion.WingsOfLiberty));
            Expansions.Add(new KeyValuePair<string, SC2DomainEntities.Expansion>("Undefined", SC2DomainEntities.Expansion.Undefined));

            SaveCommand = new RelayCommand(SaveEvent, CanSaveEvent);
            NavigatedToCommand = new RelayCommand<object>(SelectEvent);
            EditSubEventCommand = new RelayCommand(EditSubEvent);
        }

        private bool CanSaveEvent()
        {
            return SelectedEvent != null && !SelectedEvent.HasErrors;
        }

        private void EditSubEvent()
        {
            using (new NHibernateSessionContext())
            {
                var domainSubEvent = StatisticService.LoadEvent(SelectedSubEvent.Id);
                SelectedEvent = Mapper.Map<SC2DomainEntities.Event, Event>(domainSubEvent);
            }
        }

        private void SelectEvent(object parameter)
        {
            SelectedEvent = parameter as Event;
            if (SelectedEvent != null)
            {
                SelectedExpansion = Expansions.First(x => x.Value == SelectedEvent.Expansion);
            }

            SubEventInput = new EventInput();
        }

        private void SaveEvent()
        {
            if (!SelectedEvent.IsValid)
                return;

            using (new NHibernateSessionContext())
            {
                var domainEvent = StatisticService.LoadEvent(SelectedEvent.Id);
                domainEvent = Mapper.Map(SelectedEvent, domainEvent);
                
                // TODO: Update IsActive in the Grid
                //var eventsIdToActive = SelectedEvent.SubEvents
                //    .Where(x => x.IsActive)
                //    .Select(x => x.Id);

                //var eventsIdToDeactive = SelectedEvent.SubEvents
                //    .Where(x => !x.IsActive)
                //    .Select(x => x.Id);

            }

            if (SelectedEvent.MainEvent != null)
            {
                ReloadSelectedEvent(SelectedEvent.MainEvent.Id);
            }
            else
                NavigationService.GoBack();
        }

        private void ReloadSelectedEvent(long selectedEventId)
        {
            using (new NHibernateSessionContext())
            {
                var domainEvent = StatisticService.LoadEvent(selectedEventId);
                SelectedEvent = Mapper.Map<SC2DomainEntities.Event, Event>(domainEvent);
            }
        }
    }
}
