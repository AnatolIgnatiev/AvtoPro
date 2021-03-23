using Newtonsoft.Json;

namespace AvtoPro.Rest.Models
{
    public class TitleBlock
    {
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Str")]
        public string Str { get; set; }
    }
}
