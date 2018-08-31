using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class Alias
    {
        public string Id { get; set; }
        public string firstname { get; set; }
        public string middleName { get; set; }
        public string lastname { get; set; }
        public ViewModels.Worker worker { get; set; }
        public ViewModels.Contact contact { get; set; }
    }
}
