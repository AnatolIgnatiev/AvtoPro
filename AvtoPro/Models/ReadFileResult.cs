using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvtoPro.Models
{
    public class ReadFileResult
    {
        public string FileName { get; set; }

        public bool Success { get; set; }

        public List<SearchRequest> Requests { get; set; }
    }
}
