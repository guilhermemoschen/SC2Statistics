using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SC2Statistics.Proxy.Aligulac.Contracts
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Match
    {
        [JsonProperty("date")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonProperty("eventobj")]
        public Event Event { get; set; }

        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("offline")]
        public bool Offline { get; set; }

        [JsonProperty("pla")]
        public Player PlayerA { get; set; }

        [JsonProperty("plb")]
        public Player PlayerB { get; set; }

        [JsonProperty("rca")]
        public string RacePlayerA { get; set; }

        [JsonProperty("rcb")]
        public string RacePlayerB { get; set; }

        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }

        [JsonProperty("sca")]
        public int ScorePlayerA { get; set; }

        [JsonProperty("scb")]
        public int ScorePlayerB { get; set; }
    }
}
