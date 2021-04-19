using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ldb_orders_service.Models
{
    public class LdbOrderCsv
    {
        public int Licence { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public decimal? OrderAmount { get; set; }
    }
}
