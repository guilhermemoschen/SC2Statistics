using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Linq;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;
using SC2LiquipediaStatistics.Utilities.DataBase;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Repository
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
