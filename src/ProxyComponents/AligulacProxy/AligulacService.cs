using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using SC2LiquipediaStatistics.Utilities.Domain;
using SC2LiquipediaStatistics.Utilities.Log;

using SC2Statistics.Proxy.Aligulac.Contracts;

using DomainEntities = SC2Statistics.StatisticDomain.Model;

namespace SC2Statistics.Proxy.Aligulac
{
    public class AligulacService : IAligulacService
    {
        private readonly string apiKey;
        private readonly string aligulacBaseAddress;

        public const string PlayerObjectName = "player";
        public const string MatchObjectName = "match";
        public const string LimitPerRequest = "100";

        public ILogger Logger { get; private set; }
        public IMapper Mapper { get; private set; }

        public AligulacService(ILogger logger, IMapper mapper)
        {
            Logger = logger;
            Mapper = mapper;
            apiKey = ConfigurationManager.AppSettings["AligulacApiKey"];
            aligulacBaseAddress = ConfigurationManager.AppSettings["AligulacApiUrl"];
        }

        public IEnumerable<DomainEntities.Player> FindAllPlayers(int lastAligulacIdAdded = 0)
        {
            var players = new List<Player>();

            Logger.Info("Getting new players...");
            var json = JObject.Parse(GetJson(
                PlayerObjectName,
                null,
                new[] {
                    new KeyValuePair<string, string>("order_by", "id"),
                    new KeyValuePair<string, string>("id__gt", lastAligulacIdAdded.ToString()),
                }
            ));

            players.AddRange(GetAllNextObjects<Player>(json));

            return Mapper.Map<IEnumerable<Player>, IEnumerable<DomainEntities.Player>>(players);
        }

        public DomainEntities.Player GetPlayer(int aligulacPlayerId)
        {
            var aligulacJson = JObject.Parse(GetJson(PlayerObjectName, aligulacPlayerId.ToString(), null));
            var player = JsonConvert.DeserializeObject<Player>(aligulacJson.ToString());
            return Mapper.Map<Player, DomainEntities.Player>(player);
        }

        public IEnumerable<DomainEntities.Match> FindMatches(int aligulacPlayerId, DomainEntities.Expansion expansion, DateTime? lastestedSync = null)
        {
            Logger.Info("Getting matches...");
            var matches = new List<Match>();

            var game = Converter.ExpansionToString(expansion);

            var parameters = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("pla", aligulacPlayerId.ToString()),
                new KeyValuePair<string, string>("game", game)
            };

            if (lastestedSync != null)
            {
                parameters.Add(new KeyValuePair<string, string>("date__gt", lastestedSync.Value.ToString("yyyy-MM-dd")));
            }

            var json = JObject.Parse(GetJson(MatchObjectName, null, parameters));

            matches.AddRange(GetAllNextObjects<Match>(json));

            parameters = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("plb", aligulacPlayerId.ToString()),
                new KeyValuePair<string, string>("game", game)
            };

            if (lastestedSync != null)
            {
                parameters.Add(new KeyValuePair<string, string>("date__gt", lastestedSync.Value.ToString("yyyy-MM-dd")));
            }

            json = JObject.Parse(GetJson(MatchObjectName, null, parameters));

            matches.AddRange(GetAllNextObjects<Match>(json));

            return Mapper.Map<IEnumerable<Match>, IEnumerable<DomainEntities.Match>>(matches);
        }

        private IEnumerable<T> GetAllNextObjects<T>(JObject json)
        {
            var list = new List<T>();

            if (json != null && json["objects"].Any())
            {
                list.AddRange(JsonConvert.DeserializeObject<IEnumerable<T>>(json["objects"].ToString()));

                string nextAddress;
                while ((nextAddress = json["meta"]["next"].Value<string>()) != null)
                {
                    var limit = json["meta"]["limit"].Value<int>();
                    var total = json["meta"]["total_count"].Value<int>();
                    var offset = json["meta"]["offset"].Value<int>();
                    Logger.Info($"Getting next {limit} of {offset + limit}/{total}");
                    json = JObject.Parse(GetJson(nextAddress));
                    list.AddRange(JsonConvert.DeserializeObject<IEnumerable<T>>(json["objects"].ToString()));
                }
            }

            return list;
        }

        private string GetJson(string objectType, string objectSufix, IEnumerable<KeyValuePair<string, string>> extraParameters)
        {
            var address = $"/api/v1/{objectType}/{objectSufix ?? string.Empty}?apikey={apiKey}&limit={LimitPerRequest}";
            if (extraParameters != null && extraParameters.Any())
            {
                foreach (var extraParameter in extraParameters)
                {
                    address += "&" + extraParameter.Key + "=" + extraParameter.Value;
                }
            }

            return GetJson(address);
        }

        private string GetJson(string address)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Clear();
                client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                client.BaseAddress = aligulacBaseAddress;
                return client.DownloadString(address);
            }
        }
    }
}
