using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DomainEntities = SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.Proxy.Aligulac
{
    public interface IAligulacService
    {
        IEnumerable<DomainEntities.Player> FindAllPlayers(int lastAligulacIdAdded = 0);

        DomainEntities.Player GetPlayer(int aligulacPlayerId);

        IEnumerable<DomainEntities.Match> FindMatches(int aligulacPlayerId, DomainEntities.Expansion expansion, DateTime? lastestedSync = null);
    }
}