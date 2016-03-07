﻿using System;
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

using AutoMapper;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.SC2Domain.Service;

using DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class ListEventsViewModel : ViewModelBase
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

        public ICommand SelectEvent { get; private set; }

        public ISC2Service SC2Service { get; private set; }

        public IModernNavigationService NavigationService { get; private set; }

        public IMapper Mapper { get; protected set; }

        public ListEventsViewModel(ISC2Service sc2Service, IModernNavigationService navigationService, IMapper mapper)
        {
            SC2Service = sc2Service;
            NavigationService = navigationService;
            Mapper = mapper;

            Initialize();
        }

        /// <summary>
        /// Constructor only for Designner
        /// </summary>
        public ListEventsViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            Events = new ObservableCollection<Event>();
            SelectEvent = new RelayCommand(NavigateToEditEvent);
        }

        private void NavigateToEditEvent()
        {
            NavigationService.NavigateTo(ViewLocator.EditEventView, SelectedEvent);
        }

        public void OnLoad()
        {
            if (IsInDesignMode)
                return;

            var domainEvents = SC2Service.FindAllEvents();
            Events = new ObservableCollection<Event>(Mapper.Map<IList<DomainEntities.Event>, IList<Event>>(domainEvents));
        }
    }
}