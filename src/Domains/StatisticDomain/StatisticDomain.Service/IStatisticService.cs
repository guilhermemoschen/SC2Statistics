using System.Collections.Generic;

using SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.StatisticDomain.Service
{
    public interface IStatisticService
    {
        SoloPlayerStatistics UpdateDataAndGenerateSoloPlayerStatistics(int aligulacPlayerId, Expansion expansion);
        SoloPlayerStatistics GenerateSoloPlayerStatistics(Player player, Expansion expansion);

        PlayerXPlayerStatistics UpdateDataAndGeneratePlayerXPlayerStatistics(int aligulacPlayer1Id, int aligulacPlayer2Id, Expansion expansion);
        PlayerXPlayerStatistics GeneratePlayerXPlayerStatistics(Player player1, Player player2, Expansion expansion);

        Event CreateEvent(Event sc2Event);

        IEnumerable<Player> FindPlayers(string tag, int pageIndex = 0, int pageSize = 20);

        IEnumerable<Player> FindAllPlayers(int pageIndex, int pageSize);

        IEnumerable<Player> FindAllPlayers();

        Event LoadEvent(long eventId);

        IList<Event> FindEventsByPlayer(long playerId);

        void UpdateAllPlayers();

        void UpdateDateAndMatchesByPlayer(int aligulacPlayerId, Expansion expansion);
        void UpdateMatchesByPlayer(Player player, Expansion expansion);
    }
}