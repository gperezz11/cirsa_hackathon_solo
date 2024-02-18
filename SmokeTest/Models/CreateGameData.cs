using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameStatistics.Test.Models
{

    public class GameData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("gameName")]
        public string GameName { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("totalBets")]
        public int TotalBets { get; set; }

    }

}
