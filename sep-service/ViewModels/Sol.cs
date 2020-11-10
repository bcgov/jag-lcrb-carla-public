using System;
using SepService.ViewModels;

namespace SepService
{
    public class Sol
    {
        public Applicant Applicant { get; set; }
        public int Capacity { get; set; }
        public string EventDescription { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public Location Location { get; set; }
        public ResponsibleIndividual ResponsibleIndividual { get; set; }
        public string SolLicenceNumber { get; set; }
        public string SolNote { get; set; }
        public string TsAndCs { get; set; }
    }
}
