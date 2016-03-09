using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using FirstFloor.ModernUI.Windows.Controls;

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
    public class AddEventViewModel : ViewModelBase
    {
        public ISC2Service SC2Service { get; protected set; }

        public IParseService ParseService { get; protected set; }

        public IModernNavigationService NavigationService { get; protected set; }

        public IMapper Mapper { get; protected set; }

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

        public AddEventViewModel(ISC2Service sc2Service, IParseService parseService, IModernNavigationService navigationService, IMapper mapper)
        {
            SC2Service = sc2Service;
            ParseService = parseService;
            NavigationService = navigationService;
            Mapper = mapper;

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
            LogMessage = string.Empty;
            Event sc2Event;

            using (var context = new NHibernateSessionContext())
            {
                try
                {
                    var domainEvent = await ParseService.ParseEvent(EventUrl);
                    SC2Service.CreateEvent(domainEvent);
                    sc2Event = Mapper.Map<SC2DomainEntities.Event, Event>(domainEvent);
                }
                catch (ValidationException ex)
                {
                    ModernDialog.ShowMessage(ex.GetFormatedMessage(), "Validation Message", MessageBoxButton.OK);
                    return;
                }
            }

            NavigationService.NavigateTo(ViewLocator.EditEventView, sc2Event);
        }
    }
}
