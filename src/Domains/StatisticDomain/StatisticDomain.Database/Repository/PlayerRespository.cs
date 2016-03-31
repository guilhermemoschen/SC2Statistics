using System.Collections.Generic;
using System.Linq;

using NHibernate.Linq;

using SC2Statistics.StatisticDomain.Model;
using SC2Statistics.Utilities.DataBase;

namespace SC2Statistics.StatisticDomain.Database.Repository
{
    public class PlayerRespository : RepositoryBase<Player>, IPlayerRespository
    {
        public Player FindOrCreate(string playerName)
        {
            var currentPlayer = Session.Query<Player>()
                .FirstOrDefault(x => x.Name.ToUpper() == playerName.ToUpper());

            if (currentPlayer == null)
            {
                currentPlayer = new Player()
                {
                    Name = playerName
                };

                Session.Save(currentPlayer);
            }

            return currentPlayer;
        }

        public int GetBigestPlayerAligulacId()
        {
            if (Session.Query<Player>().Any())
                return Session.Query<Player>()
                    .Max(x => x.AligulacId);

            return 0;
        }

        public IEnumerable<Player> FindByTag(string tag, int pageIndex = 0, int pageSize = 20)
        {
            return Session.Query<Player>()
                .Where(x => x.Tag.ToUpper().Contains(tag.ToUpper()))
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Player FindByAligulacId(int aligulacId)
        {
            return Session.Query<Player>()
                .FirstOrDefault(x => x.AligulacId == aligulacId);
        }
    }
}
