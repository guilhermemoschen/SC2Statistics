using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using NHibernate.Linq;
using NHibernate.Type;

using SC2LiquipediaStatistics.ImportTool.Log;
using SC2LiquipediaStatistics.Utilities.DataBase;
using SC2LiquipediaStatistics.Utilities.Web;

using SC2Statistics.SC2Domain.Model;
using SC2Statistics.SC2Domain.Repository;
using SC2Statistics.SC2Domain.Service;

namespace SC2LiquipediaStatistics.ImportTool
{
    class Program
    {
        static void Main(string[] args)
        {

            InitializeDataBase();

            var eventsList = new string[]
            {
                "http://wiki.teamliquid.net/starcraft2/Gold_Series_International_2016",
                "http://wiki.teamliquid.net/starcraft2/Ting_Open",
                "http://wiki.teamliquid.net/starcraft2/Kung_Fu_Cup/2016/1",
                "http://wiki.teamliquid.net/starcraft2/Copa_America_2016/Season_1",
                "http://wiki.teamliquid.net/starcraft2/IEM_Season_X_-_Taipei",
                "http://wiki.teamliquid.net/starcraft2/GPL_2015/Grand_Finals",
            };

            var downloader = new Downloader();
            var parseService = new ParseService(new PlayerRespository());
            var logger = new ConsoleLogger();

            var liquipediService = new LiquipediaService(parseService, downloader, logger);

            var events = liquipediService.GetEvents(eventsList);

            foreach (var sc2Event in events)
            {
                var noWinners = sc2Event.Matches.Where(x => x.Winner == null).ToList();
                if (noWinners.Any())
                {

                }


                var noPlayers = sc2Event.Matches.Where(x => x.Player1 == null || x.Player2 == null || x.Player1.Id == 0 || x.Player2.Id == 0).ToList();
                if (noPlayers.Any())
                {

                }

                var samePlayers = sc2Event.Matches.Where(x => x.Player1.Name == x.Player2.Name).ToList();
                if (samePlayers.Any())
                {

                }
            }

            var eventRepository = new EventRepository();

            foreach (var sc2Event in events)
                eventRepository.Save(sc2Event);
            
        }

        private static void InitializeDataBase()
        {
            const string DatabaseName = "SC2LiquipediaStatistics.db";
            NHibernateAndSQLiteConfiguration.SetupDatabase(typeof(Player).Assembly, DatabaseName);
        }
    }
}
