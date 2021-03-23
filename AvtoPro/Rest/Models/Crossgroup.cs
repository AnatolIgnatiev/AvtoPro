using Newtonsoft.Json;

namespace AvtoPro.Rest.Models
{
    public class Crossgroup
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Category")]
        public Category Category { get; set; }

        [JsonProperty("Specification")]
        public string Specification { get; set; }

        [JsonProperty("Images")]
        public object Images { get; set; }

        [JsonProperty("Parts")]
        public object Parts { get; set; }

        [JsonProperty("PartsWithSellersCount")]
        public object PartsWithSellersCount { get; set; }

        [JsonProperty("PartsCount")]
        public long PartsCount { get; set; }
    }
}
