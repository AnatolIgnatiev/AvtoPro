using System.Collections.Generic;
using Newtonsoft.Json;

namespace AvtoPro.Rest.Models
{
    public class SearchResult
    {
        [JsonProperty("Suggestions")]
        public List<Suggestion> Suggestions { get; set; }
    }
}
