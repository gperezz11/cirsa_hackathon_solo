using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AverageLoadTest.Models
{

    public class GameData
    {
        [JsonProperty("id", Required = Required.AllowNull)]
        public string Id { get; set; }

        [JsonProperty("gameName", Required = Required.AllowNull)]
        public string GameName { get; set; }

        [JsonProperty("category", Required = Required.AllowNull)]
        public string Category { get; set; }

        [JsonProperty("totalBets", Required = Required.AllowNull)]
        public int TotalBets { get; set; }

    }

}
