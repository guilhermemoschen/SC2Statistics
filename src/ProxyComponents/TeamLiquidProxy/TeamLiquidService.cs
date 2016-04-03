using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

using SC2Statistics.StatisticDomain.Model;
using SC2Statistics.Utilities.Web;

namespace SC2Statistics.Proxy.TeamLiquied
{
    public class TeamLiquidService : ITeamLiquidService
    {
        public const string StarcraftTeamLiquidWikiURL = "http://wiki.teamliquid.net/starcraft2";

        public IDownloader Downloader { get; private set; }

        public TeamLiquidService(IDownloader downloader)
        {
            Downloader = downloader;
        }

        public Uri GetPlayerImage(Player player)
        {
            if (player == null || string.IsNullOrEmpty(player.LiquipediaName))
                return null;

            var htmlContent = Downloader.GetContent(new Uri($"{StarcraftTeamLiquidWikiURL}/{player.LiquipediaName}"));

            var html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            var imgNode = html.DocumentNode.SelectSingleNode(@"//div[@class='infobox-image']//img");
            if (imgNode == null)
                return null;
            return new Uri(imgNode.Attributes["src"].Value);
        }
    }
}
