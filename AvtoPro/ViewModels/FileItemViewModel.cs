using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvtoPro.ViewModels
{
    public class FileItemViewModel
    {
        public string FileName { get; set; }

        public ICommand RemoveItem { get; set; }
    }
}
