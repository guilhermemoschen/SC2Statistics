using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SC2Statistics.Proxy.Aligulac.Contracts
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Event
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }
    }

    /*
          "category":{  },
      "children":{  },
      "earliest":{  },
      "earnings":{  },
      "homepage":{  },
      "idx":{  },
      "latest":{  },
      "lp_name":{  },
      "name":{  },
      "parent":{  },
      "prizepool":{  },
      "tl_thread":{  },
      "tlpd_db":{  },
      "tlpd_id":{  },
      "type":{  },
      "wcs_tier":{  },
      "wcs_year":{  }

    */
}
