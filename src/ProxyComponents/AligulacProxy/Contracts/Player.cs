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
    public class Player
    {
        [JsonProperty("aliases")]
        public IEnumerable<string> Aliases { get; set; }

        [JsonProperty("birthday")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? Birthday { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        //[JsonProperty("current_rating")]
        //[JsonConverter(typeof(Newtonsoft.Json.Converters.))]
        //public string CurrentRating { get; set; }

        //[JsonProperty("current_teams")]
        //public string CurrentTeams { get; set; }

        [JsonProperty("form")]
        public Form Form { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("lp_name")]
        public string LiquipediaName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        //[JsonProperty("past_teams")]
        //public IEnumerable<string> PastTeams { get; set; }

        [JsonProperty("race")]
        public string Race { get; set; }

        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }

        //"mcnum":{  },
        //"romanized_name":{  },
        //"sc2e_id":{  },
        //"tlpd_db":{  },
        //"tlpd_id":{  },
        //"total_earnings":{  }
        //"dom_end":{  },
        //"dom_start":{  },
        //"dom_val":{  },
    }
}
