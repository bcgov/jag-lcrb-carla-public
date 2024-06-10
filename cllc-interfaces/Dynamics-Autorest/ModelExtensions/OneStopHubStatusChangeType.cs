using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public enum OneStopHubStatusChange
    {
        [Display(Name = "Issued")] Issued = 845280000,
        [Display(Name = "Updated - Change of Address")] ChangeOfAddress = 845280001,
        [Display(Name = "Updated - Change of Name")] ChangeOfName = 845280002,
        [Display(Name = "Updated - Entered Dormancy")] EnteredDormancy = 845280003,
        [Display(Name = "Updated - Dormancy Ended")] DormancyEnded = 845280004,
        [Display(Name = "Updated - Suspended")] Suspended = 845280005,
        [Display(Name = "Updated - Suspension Ended")] SuspensionEnded = 845280006,
        [Display(Name = "Updated - Cancelled")] Cancelled = 845280007,
        [Display(Name = "Updated - Cancellation Removed")] CancellationRemoved = 845280008,
        [Display(Name = "Updated - Licence Deemed at Transfer")] LicenceDeemedAtTransfer = 845280009,
        [Display(Name = "Updated - Transfer Complete")] TransferComplete = 845280010,
        [Display(Name = "Updated - Licensee BN9 Changed")] LicenseeBn9Changed = 845280013,
        [Display(Name = "Updated - Licensee BN9 Added")] LicenseeBn9Added = 845280014,
        [Display(Name = "Updated - Licensee BN9 Removed")] LicenseeBn9Removed = 845280015,
        [Display(Name = "Expired")] Expired = 845280011,
        [Display(Name = "Renewed")] Renewed = 845280012,
        [Display(Name = "Updated - Endorsement Approved")] EndorsementApproved = 845280016,
    }
}
