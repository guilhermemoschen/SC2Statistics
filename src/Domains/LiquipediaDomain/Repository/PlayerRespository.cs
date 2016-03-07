using System.Collections.Generic;
using System.Linq;

using NHibernate.Linq;

using SC2LiquipediaStatistics.Utilities.DataBase;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository
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
    }
}
