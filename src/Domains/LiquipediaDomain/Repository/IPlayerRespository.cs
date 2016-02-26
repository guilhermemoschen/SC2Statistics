using SC2LiquipediaStatistics.LiquipediaDomain.Model;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Repository
{
    public interface IPlayerRespository
    {
        Player FindOrCreate(string playerName);
    }
}