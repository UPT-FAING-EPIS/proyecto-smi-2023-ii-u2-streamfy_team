using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamfy.Models
{
    public class DeezerSearchResponse
    {
        [JsonProperty("data")]
        public List<DeezerData> Data { get; set; }
    }
}
