using Newtonsoft.Json;

namespace AvtoPro.Rest.Models
{
    public class Category
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Image")]
        public Image Image { get; set; }

        [JsonProperty("Uri")]
        public object Uri { get; set; }

        [JsonProperty("Link")]
        public string Link { get; set; }
    }
}
