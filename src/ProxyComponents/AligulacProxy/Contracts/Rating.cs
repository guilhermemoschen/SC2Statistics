using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SC2Statistics.Proxy.Aligulac.Contracts
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Rating
    {
        /// <summary>
        /// number of periods since last game
        /// 0
        /// </summary>
        [JsonProperty("aliases")]
        public int Decay { get; set; }

        /// <summary>
        /// current RD (Ratings Deviation)
        /// "dev":0.0813668497090874
        /// </summary>
        [JsonProperty("dev")]
        public double Dev { get; set; }

        /// <summary>
        /// current RD vP (Ratings Deviation)
        /// "dev_vp":0.127183282381614,
        /// </summary>
        [JsonProperty("dev_vp")]
        public double DevVersusProtoss { get; set; }

        /// <summary>
        /// current RD vT (Ratings Deviation)
        /// "dev_vt":0.0979341941571942,
        /// </summary>
        [JsonProperty("dev_vt")]
        public double DevVersusTerran { get; set; }

        /// <summary>
        /// current RD vZ (Ratings Deviation)
        /// "dev_vz":0.145057225271041,
        /// </summary>
        [JsonProperty("dev_vz")]
        public double DevVersusZerg { get; set; }

        /// <summary>
        /// primary key
        /// "id":5573464,
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// current rating
        /// "rating":1.03037809670083,
        /// </summary>
        [JsonProperty("rating")]
        public double RatingOverall { get; set; }

        /// <summary>
        /// current rating delta vP
        /// "rating_vp":0.0674222200676453,
        /// </summary>
        [JsonProperty("rating_vp")]
        public double RatingVersusProtoss { get; set; }

        /// <summary>
        /// current rating delta vT
        /// "rating_vt":-0.0540033527624013,
        /// </summary>
        [JsonProperty("rating_vt")]
        public double RatingVersusTerran { get; set; }

        /// <summary>
        /// current rating delta vZ
        /// "rating_vz":-0.013418867305244,
        /// </summary>
        [JsonProperty("rating_vz")]
        public double RatingVersusZerg { get; set; }

        /// <summary>
        /// The reference URL of the current Rating.
        /// "resource_uri":"/api/v1/rating/5573464/",
        /// </summary>
        [JsonProperty("resource_uri")]
        public string ReferenceUri { get; set; }

        /*
          
          "tot_vp":1.0978003167684753,
          "tot_vt":0.9763747439384287,
          "tot_vz":1.016959229395586
        */


    }
}
