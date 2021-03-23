using System.Collections.Generic;
using Newtonsoft.Json;

namespace AvtoPro.Rest.Models
{    
    public class Suggestion
    {
        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("TitleBlocks")]
        public List<TitleBlock> TitleBlocks { get; set; }

        [JsonProperty("NextQueryString")]
        public string NextQueryString { get; set; }

        [JsonProperty("Uri")]
        public string Uri { get; set; }

        [JsonProperty("FoundMake")]
        public object FoundMake { get; set; }

        [JsonProperty("FoundModel")]
        public object FoundModel { get; set; }

        [JsonProperty("FoundEngine")]
        public object FoundEngine { get; set; }

        [JsonProperty("FoundCategory")]
        public object FoundCategory { get; set; }

        [JsonProperty("FoundCrossgroupApplicability")]
        public object FoundCrossgroupApplicability { get; set; }

        [JsonProperty("FoundPart")]
        public FoundPart FoundPart { get; set; }

        [JsonProperty("PartFitsVehicle")]
        public object PartFitsVehicle { get; set; }

        [JsonProperty("PartFitsEngine")]
        public object PartFitsEngine { get; set; }
    }
}
