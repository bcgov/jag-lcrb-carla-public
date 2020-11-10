using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SepService.ViewModels
{
    public class EventDate
	{
        public DateTime EventEndDateTime { get; set; }
        public DateTime EventStartDateTime { get; set; }
        public DateTime LiquorServiceEndTime { get; set; }
        public DateTime LiquorServiceStartTime { get; set; }
    }
}
