using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.Utilities.DataBase;
using SC2LiquipediaStatistics.Utilities.Domain;

using SC2Statistics.SC2Domain.Service;

using SC2DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class AddEventViewModel : ModernViewModelBase
    {
        public ISC2Service SC2Service { get; protected set; }

        public IParseService ParseService { get; protected set; }

        public IModernNavigationService NavigationService { get; protected set; }

        public IMapper Mapper { get; protected set; }

        public ILoadingService LoadingService { get; protected set; }

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
                if (value == null || eventUrl == value)
                    return;

                Set(() => EventUrl, ref eventUrl, value, true);
            }
        }

        public AddEventViewModel(ISC2Service sc2Service, IParseService parseService, IModernNavigationService navigationService, ILoadingService loadingService, IMapper mapper)
        {
            SC2Service = sc2Service;
            ParseService = parseService;
            NavigationService = navigationService;
            LoadingService = loadingService;
            Mapper = mapper;

            AddNewCommand = new RelayCommand(AddNewEvent);
            NavigatedToCommand = new RelayCommand<object>(LoadView);
        }

        private void LoadView(object o)
        {
            EventUrl = string.Empty;
        }

        private void AddNewEvent()
        {
            Event sc2Event = null;
            ValidationException validationException = null;

            LoadingService.ShowAndExecuteAction(delegate
            {
                using (new NHibernateSessionContext())
                {
                    try
                    {
                        var domainEvent = ParseService.GetSC2EventWithSubEvents(EventUrl);
                        domainEvent = SC2Service.CreateEvent(domainEvent);
                        sc2Event = Mapper.Map<SC2DomainEntities.Event, Event>(domainEvent);
                    }
                    catch (ValidationException ex)
                    {
                        validationException = ex;
                    }
                }
            });

            if (validationException != null)
            {
                ModernDialog.ShowMessage(validationException.GetFormatedMessage(), "Validation Message", MessageBoxButton.OK);
                return;
            }

            NavigationService.NavigateTo(ViewLocator.EditEventView, sc2Event);
        }
    }
}