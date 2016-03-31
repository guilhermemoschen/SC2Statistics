using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.StatisticDomain.Service;

using WpfControls.Editors;

using SC2DomainEntities = SC2Statistics.StatisticDomain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Common
{
    public class PlayerSuggestionProvider : ISuggestionProvider
    {
        public IStatisticService StatisticService { get; set; }
        public IMapper Mapper { get; set; }

        public PlayerSuggestionProvider(IStatisticService statisticService, IMapper mapper)
        {
            StatisticService = statisticService;
            Mapper = mapper;
        }

        public IEnumerable GetSuggestions(string filter)
        {
            var domainPlayers = StatisticService.FindPlayers(filter);
            return Mapper.Map<IEnumerable<SC2DomainEntities.Player>, IEnumerable<Player>>(domainPlayers);
        }
    }
}
