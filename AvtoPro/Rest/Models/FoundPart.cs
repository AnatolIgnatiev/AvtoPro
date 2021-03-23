using Newtonsoft.Json;

namespace AvtoPro.Rest.Models
{
    public class FoundPart
    {
        [JsonProperty("Part")]
        public Part Part { get; set; }

        [JsonProperty("SellersCount")]
        public object SellersCount { get; set; }

        [JsonProperty("PriceMin")]
        public object PriceMin { get; set; }

        [JsonProperty("PriceMax")]
        public object PriceMax { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }
}
