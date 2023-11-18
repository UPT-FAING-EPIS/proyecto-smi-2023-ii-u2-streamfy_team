using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamfy.Models
{
    public class DeezerData
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }

        [JsonProperty("explicit_lyrics")]
        public bool ExplicitLyrics { get; set; }

        [JsonProperty("artist")]
        public DeezerArtist Artist { get; set; }

        [JsonProperty("album")]
        public DeezerAlbum Album { get; set; }
    }

    public class DeezerArtist
    {
        [JsonProperty("name")]
        public string Name { get; set; }

    }

    public class DeezerAlbum
    {
        [JsonProperty("title")]
        public string Title { get; set; }

    }
}