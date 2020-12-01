using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Rest;
using SepService.ViewModels;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace SepService.Controllers
{
    [ApiController]
    [Route("sol")]
    public class SolController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDynamicsClient _dynamicsClient;

        public SolController(IDynamicsClient dynamicsClient)
        {
            _logger = Log.Logger;
            _dynamicsClient = dynamicsClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("echo")]
        public string Get([FromBody] string value)
        {
            return value;
        }

        /// <summary>
        /// Create a Sol record
        /// </summary>
        /// <param name="sol">New or Updated Special Event Record</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateOrUpdate([FromBody] Sol sol)
        {
            if (sol == null || string.IsNullOrEmpty(sol.SolLicenceNumber) || !sol.SolLicenceNumber.Contains("-"))
            {
                return BadRequest();
            }

            // format of the "SolLicenceNumber" field is {EventLicenceNumber}-{LocationReference}
            string sepLicenceNumber = sol.SolLicenceNumber.Substring(0, sol.SolLicenceNumber.IndexOf("-"));
            // string locationReference = sol.SolLicenceNumber.Substring(sepLicenceNumber.Length);

            // determine if the record is new.
            MicrosoftDynamicsCRMadoxioSpecialevent existingRecord =
                _dynamicsClient.GetSpecialEventByLicenceNumber(sepLicenceNumber);

            if (existingRecord == null) // new record
            {

                MicrosoftDynamicsCRMadoxioSpecialevent newRecord = new MicrosoftDynamicsCRMadoxioSpecialevent()
                {
                    AdoxioCapacity = sol.Capacity,
                    AdoxioSpecialeventdescripton = sol.EventDescription,
                    AdoxioEventname = sol.EventName,
                    AdoxioSeplicencenumber = sepLicenceNumber,
                    // applicant
                    AdoxioSpecialeventapplicant = sol.Applicant?.ApplicantName,
                    AdoxioSpecialeventapplicantemail = sol.Applicant?.EmailAddress,
                    AdoxioSpecialeventapplicantphone = sol.Applicant?.PhoneNumber,
                    // location
                    AdoxioSpecialeventstreet1 = sol.Applicant?.Address?.Address1,
                    AdoxioSpecialeventstreet2 = sol.Applicant?.Address?.Address2,
                    AdoxioSpecialeventcity = sol.Applicant?.Address?.City,
                    AdoxioSpecialeventpostalcode = sol.Applicant?.Address?.PostalCode,
                    AdoxioSpecialeventprovince = sol.Applicant?.Address?.Province,
                    // responsible individual
                    AdoxioResponsibleindividualfirstname = sol.ResponsibleIndividual?.FirstName,
                    AdoxioResponsibleindividuallastname = sol.ResponsibleIndividual?.LastName,
                    AdoxioResponsibleindividualmiddleinitial = sol.ResponsibleIndividual?.MiddleInitial,
                    AdoxioResponsibleindividualposition = sol.ResponsibleIndividual?.Position,
                    AdoxioResponsibleindividualsir = sol.ResponsibleIndividual?.SirNumber,
                    // tasting event
                    AdoxioTastingevent = sol.TastingEvent
                };

                if (sol.Location?.EventDates != null)
                {
                    // event date
                    newRecord.AdoxioSpecialeventSchedule = new List<MicrosoftDynamicsCRMadoxioSpecialeventschedule>();

                    foreach (var item in sol.Location.EventDates)
                    {
                        newRecord.AdoxioSpecialeventSchedule.Add(new MicrosoftDynamicsCRMadoxioSpecialeventschedule()
                        {
                            AdoxioServicestart = item.LiquorServiceStartTime,
                            AdoxioServiceend = item.LiquorServiceEndTime,
                            AdoxioEventstart = item.EventStartDateTime,
                            AdoxioEventend = item.EventEndDateTime
                        });
                    }
                }


                // licensed area
                if (sol.Location?.LicencedArea != null)
                {
                    newRecord.AdoxioSpecialeventLicencedarea = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                    var newItem = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea()
                    {
                        AdoxioDescription = sol.Location.LicencedArea.Description,
                        AdoxioMinorpresent = sol.Location.LicencedArea.MinorsPresent,
                        AdoxioSetting = (int?)sol.Location.LicencedArea.Setting,
                        AdoxioMaximumnumberofguests = sol.Location.LicencedArea.MaxGuests.ToString(),
                        AdoxioNumberofminors = sol.Location?.LicencedArea?.NumberOfMinors.ToString()
                    };

                }


                // note
                if (sol.SolNotes != null)
                {
                    newRecord.AdoxioSpecialeventSpecialeventnotes = new List<MicrosoftDynamicsCRMadoxioSpecialeventnote>();

                    foreach (var item in sol.SolNotes)
                    {
                        newRecord.AdoxioSpecialeventSpecialeventnotes.Add(new MicrosoftDynamicsCRMadoxioSpecialeventnote()
                        {
                            AdoxioNote = item.Text,
                            AdoxioAuthor = item.Author
                        });
                    }
                }

                // terms and conditions
                if (sol.TsAndCs != null)
                {
                    newRecord.AdoxioSpecialeventSpecialeventtsacs = new List<MicrosoftDynamicsCRMadoxioSpecialeventtandc>();
                    foreach (var item in sol.TsAndCs)
                    {
                        var newTandC = new MicrosoftDynamicsCRMadoxioSpecialeventtandc()
                        {
                            AdoxioTermsandcondition = item.Text,
                            AdoxioOrganizer = item.Originator
                        };

                        if (item.TandcType == TandcType.Term)
                        {
                            newTandC.AdoxioTermsandconditiontype = true;
                        }

                        newRecord.AdoxioSpecialeventSpecialeventtsacs.Add(newTandC);
                    }

                }

                try
                {
                    newRecord = _dynamicsClient.Specialevents.Create(newRecord);
                    _logger.Information($"Created special event {newRecord.AdoxioSpecialeventid}");
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.Error(httpOperationException, "Error creating special event record");
                    // fail 
                    return StatusCode(500, "Server Error creating record.");
                }

            }
            else // existing record.
            {


                // licensed area
                if (sol.Location?.LicencedArea != null)
                {
                    MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea newLicencedArea = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea()
                    {
                        AdoxioDescription = sol.Location.LicencedArea.Description,
                        AdoxioMinorpresent = sol.Location.LicencedArea.MinorsPresent,
                        // Setting - Indoor, Outdoor or Both
                        AdoxioSetting = (int?)sol.Location.LicencedArea.Setting,
                        AdoxioMaximumnumberofguests = sol.Location.LicencedArea.MaxGuests.ToString(),
                        AdoxioNumberofminors = sol.Location?.LicencedArea?.NumberOfMinors.ToString()
                    };

                    // add the new licenced area to Dynamics
                    try
                    {
                        newLicencedArea = _dynamicsClient.Specialeventlicencedareas.Create(newLicencedArea);
                        _logger.Information($"Created new licenced area {newLicencedArea.AdoxioSpecialeventlicencedareaid }");
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error creating special event record");
                        // fail 
                        return StatusCode(500, "Server Error creating record.");
                    }

                    // bind the new licenced area to the parent.

                    var specialEventLicencedAreasData = _dynamicsClient.GetEntityURI("adoxio_specialeventlicencedareas", newLicencedArea.AdoxioSpecialeventlicencedareaid);

                    var oDataIdLicencedAreas = new Odataid
                    {
                        OdataidProperty = specialEventLicencedAreasData
                    };
                    try
                    {
                        _dynamicsClient.Specialevents.AddReference(existingRecord.AdoxioSpecialeventid, "adoxio_specialevent_licencedarea", oDataIdLicencedAreas);
                    }
                    catch (HttpOperationException odee)
                    {
                        Log.Error(odee, "Error adding reference to adoxio_specialeventlicencedareas");
                    }

                }

                if (sol.Location?.EventDates != null)
                {
                    foreach (var item in sol.Location.EventDates)
                    {
                        var newEventSchedule = new MicrosoftDynamicsCRMadoxioSpecialeventschedule()
                        {
                            AdoxioServicestart = item.LiquorServiceStartTime,
                            AdoxioServiceend = item.LiquorServiceEndTime,
                            AdoxioEventstart = item.EventStartDateTime,
                            AdoxioEventend = item.EventEndDateTime
                        };

                        try
                        {
                            newEventSchedule = _dynamicsClient.Specialeventschedules.Create(newEventSchedule);
                            _logger.Information($"Created new special event schedule {newEventSchedule.AdoxioSpecialeventscheduleid}");
                        }
                        catch (HttpOperationException httpOperationException)
                        {
                            _logger.Error(httpOperationException, "Error creating special event schedule");
                            // fail 
                            return StatusCode(500, "Server Error creating record.");
                        }

                        // now bind the two new records to the parent.

                        var specialEventScheduleData = _dynamicsClient.GetEntityURI("adoxio_specialeventschedules", newEventSchedule.AdoxioSpecialeventscheduleid);

                        var oDataIdEventSchedule = new Odataid
                        {
                            OdataidProperty = specialEventScheduleData
                        };
                        try
                        {
                            _dynamicsClient.Specialevents.AddReference(existingRecord.AdoxioSpecialeventid, "adoxio_specialevent_schedule", oDataIdEventSchedule);
                        }
                        catch (HttpOperationException odee)
                        {
                            Log.Error(odee, "Error adding reference to adoxio_specialeventschedules");
                        }
                    }
                }

            }

            return Ok();
        }

        /// <summary>
        /// Cancel a Sol Event
        /// </summary>
        /// <param name="solId">Sol Id</param>
        /// <param name="cancelReason">Reason for cancellation</param>
        /// <returns></returns>
        [HttpPost("cancel/{solId}")]
        public ActionResult Cancel([FromRoute] string solId, [FromBody] string cancelReason)
        {
            if (string.IsNullOrEmpty(solId))
            {
                return BadRequest();
            }

            string solIdEscaped = solId.Replace("'", "''");
            // get the given sol record.
            string filter = $"adoxio_seplicencenumber eq '{solIdEscaped}'";
            MicrosoftDynamicsCRMadoxioSpecialevent record;

            try
            {
                record =
                    _dynamicsClient.Specialevents.Get(filter: filter).Value.FirstOrDefault();
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.Error(httpOperationException, "Error getting special event record");
                // fail 
                record = null;
            }

            if (record == null)
            {
                return NotFound();
            }
            else
            {
                // cancel the record.
                MicrosoftDynamicsCRMadoxioSpecialevent patchRecord = new MicrosoftDynamicsCRMadoxioSpecialevent()
                {
                    Statuscode = 845280000 // Cancel
                };
                try
                {
                    _dynamicsClient.Specialevents.Update(record.AdoxioSpecialeventid, patchRecord);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.Error(httpOperationException, "Error updating special event record");
                    // fail 
                    return StatusCode(500, "Error updating record.");
                }

            }

            return Ok();

        }
    }
}
