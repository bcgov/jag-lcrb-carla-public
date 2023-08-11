using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public enum OneStopMessageStatus
    {
        [Display(Name = "Sent")] Sent = 845280000,
        [Display(Name = "Failed")] Failed = 845280001,
        [Display(Name = "Ready To Send")] ReadyToSend = 845280002,
        [Display(Name = "Donot Send")] DonotSend = 845280003,
    }
}
