using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using SC2LiquipediaStatistics.DesktopClient.Model;
using SC2LiquipediaStatistics.Utilities.Unity;

using SC2Statistics.SC2Domain.Service;

using WpfControls.Editors;

using SC2DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Common
{
    public class PlayerSuggestionProvider : ISuggestionProvider
    {
        public ISC2Service SC2Service { get; set; }
        public IMapper Mapper { get; set; }

        public PlayerSuggestionProvider(ISC2Service sc2Service, IMapper mapper)
        {
            SC2Service = sc2Service;
            Mapper = mapper;
        }

        public IEnumerable GetSuggestions(string filter)
        {
            var domainPlayers = SC2Service.FindPlayers(filter);
            return Mapper.Map<IEnumerable<SC2DomainEntities.Player>, IEnumerable<Player>>(domainPlayers);
        }
    }
}
