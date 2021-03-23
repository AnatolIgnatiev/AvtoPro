using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvtoPro.Models
{
    public class SearchResult : SearchRequest
    {
        public SearchResult(SearchRequest source)
        {
            Id = source.Id;
            Brand = source.Brand;
            OriginalPrice = source.OriginalPrice;
        }
        public string FirstPrice{ get; set; }
        
        public string FirstDeliveryText { get; set; }

        public string SecondPrice { get; set; }

        public string SecondDeliveryText { get; set; }

        public bool IsSuccessful { get; set; }
        public bool IsSkiped { get; set; }
        public bool IsCanceled { get; set; }

    }
}
