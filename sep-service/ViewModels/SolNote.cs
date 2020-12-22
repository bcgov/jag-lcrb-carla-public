using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SepService.ViewModels
{
    public class SolNote
    {
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string Text { get; set; }
    }
}
