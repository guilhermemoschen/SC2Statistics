using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.StatisticDomain.Service;
using SC2Statistics.Utilities.Web;

namespace SC2LiquipediaStatistics.DesktopClient.ViewModel
{
    public class ViewModelBase : ModernViewModelBase
    {
        private IModernNavigationService navigationService;
        public IModernNavigationService NavigationService => navigationService ?? (navigationService = Container.Resolve<IModernNavigationService>());

        private ILoadingService loadingService;
        public ILoadingService LoadingService => loadingService ?? (loadingService = Container.Resolve<ILoadingService>());

        private IMapper mapper;
        public IMapper Mapper => mapper ?? (mapper = Container.Resolve<IMapper>());

        private IStatisticService statisticService;
        public IStatisticService StatisticService => statisticService ?? (statisticService = Container.Resolve<IStatisticService>());

        public ViewModelBase()
        {
            
        }

        public ViewModelBase(IStatisticService statisticService, IModernNavigationService navigationService, ILoadingService loadingService, IMapper mapper)
        {
            this.statisticService = statisticService;
            this.navigationService = navigationService;
            this.loadingService = loadingService;
            this.mapper = mapper;
        }
    }
}
