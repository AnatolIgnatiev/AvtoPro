using Newtonsoft.Json;

namespace AvtoPro.Rest.Models
{
    public class Part
    {
        [JsonProperty("Brand")]
        public Brand Brand { get; set; }

        [JsonProperty("FullNr")]
        public string FullNr { get; set; }

        [JsonProperty("ShortNr")]
        public string ShortNr { get; set; }

        [JsonProperty("CrossgroupId")]
        public long? CrossgroupId { get; set; }

        [JsonProperty("Crossgroup")]
        public Crossgroup Crossgroup { get; set; }

        [JsonProperty("Images")]
        public object Images { get; set; }

        [JsonProperty("Parameters")]
        public object Parameters { get; set; }

        [JsonProperty("IsRestoration")]
        public bool IsRestoration { get; set; }

        [JsonProperty("PartFeedUri")]
        public object PartFeedUri { get; set; }

        [JsonProperty("IsSyntheticPartNumber")]
        public bool IsSyntheticPartNumber { get; set; }
    }
}
