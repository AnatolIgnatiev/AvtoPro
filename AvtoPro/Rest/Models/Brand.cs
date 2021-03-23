using Newtonsoft.Json;

namespace AvtoPro.Rest.Models
{
    public class Brand
    {
        [JsonProperty("Id")]
        public long? Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Link")]
        public string Link { get; set; }

        [JsonProperty("ViewName")]
        public string ViewName { get; set; }

        [JsonProperty("Country")]
        public object Country { get; set; }

        [JsonProperty("Site")]
        public object Site { get; set; }

        [JsonProperty("AverangeRating")]
        public long AverangeRating { get; set; }

        [JsonProperty("CountFeedback")]
        public long CountFeedback { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        [JsonProperty("FoundationYear")]
        public long FoundationYear { get; set; }

        [JsonProperty("Description")]
        public object Description { get; set; }

        [JsonProperty("CountUniquePart")]
        public long CountUniquePart { get; set; }

        [JsonProperty("CountSubType")]
        public long CountSubType { get; set; }

        [JsonProperty("Logo")]
        public object Logo { get; set; }

        [JsonProperty("Concern")]
        public object Concern { get; set; }

        [JsonProperty("ConcernParticipants")]
        public object ConcernParticipants { get; set; }

        [JsonProperty("IsOriginal")]
        public bool IsOriginal { get; set; }
    }
}
