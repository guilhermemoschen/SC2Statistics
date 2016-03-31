using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SC2Statistics.Proxy.Aligulac.Contracts
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Form
    {
        [JsonProperty("P")]
        public IEnumerable<int> P { get; set; }
        [JsonProperty("Z")]
        public IEnumerable<int> Z { get; set; }
        [JsonProperty("T")]
        public IEnumerable<int> T { get; set; }
        [JsonProperty("total")]
        public IEnumerable<int> Total { get; set; }
    }
}
