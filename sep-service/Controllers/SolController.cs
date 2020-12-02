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

                newRecord.AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>();

                newRecord.AdoxioSpecialeventSpecialeventlocations.Add(ExtractLocation(sol));


                // notes
                if (sol.SolNote != null)
                {
                    newRecord.AdoxioSpecialeventSpecialeventnotes =
                        new List<MicrosoftDynamicsCRMadoxioSpecialeventnote>();

                    foreach (var item in sol.SolNote)
                    {
                        newRecord.AdoxioSpecialeventSpecialeventnotes.Add(
                            new MicrosoftDynamicsCRMadoxioSpecialeventnote()
                            {
                                AdoxioNote = item.Text,
                                AdoxioAuthor = item.Author,
                                Createdon = item.CreatedDate,
                                Modifiedon = item.LastUpdatedDate
                            });
                    }
                }

                // terms and conditions
                if (sol.TsAndCs != null)
                {
                    newRecord.AdoxioSpecialeventSpecialeventtsacs =
                        new List<MicrosoftDynamicsCRMadoxioSpecialeventtandc>();
                    foreach (var item in sol.TsAndCs)
                    {
                        var newTandC = new MicrosoftDynamicsCRMadoxioSpecialeventtandc()
                        {
                            AdoxioTermsandcondition = item.Text,
                            AdoxioOrganizer = item.Originator
                        };

                        if (item.TandcType == TandcType.GlobalCondition)
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

                if (sol.Location != null)
                {

                    MicrosoftDynamicsCRMadoxioSpecialeventlocation location = ExtractLocation(sol);

                    try
                    {
                        location = _dynamicsClient.Specialeventlocations.Create(location);
                        _logger.Information(
                            $"Created new special event location {location.AdoxioSpecialeventlocationid}");
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Error creating special event schedule");
                        // fail 
                        return StatusCode(500, "Server Error creating record.");
                    }

                    // now bind the record to the parent.

                    var specialEventLocationData = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations",
                        location.AdoxioSpecialeventlocationid);

                    var oDataIdEventLocation = new Odataid
                    {
                        OdataidProperty = specialEventLocationData
                    };
                    try
                    {
                        _dynamicsClient.Specialevents.AddReference(existingRecord.AdoxioSpecialeventid,
                            "adoxio_specialevent_specialeventlocations", oDataIdEventLocation);
                    }
                    catch (HttpOperationException odee)
                    {
                        Log.Error(odee, "Error adding reference to adoxio_specialevent_specialeventlocations");
                    }
                }


            }


            return Ok();
        }

        private MicrosoftDynamicsCRMadoxioSpecialeventlocation ExtractLocation(Sol sol)
        {
            MicrosoftDynamicsCRMadoxioSpecialeventlocation location =
                new MicrosoftDynamicsCRMadoxioSpecialeventlocation()
                {
                    AdoxioLocationdescription = sol.Location.LocationDescription,
                    AdoxioLocationname = sol.Location.LocationName,
                    AdoxioMaximumnumberofguests = sol.Location.MaximumGuests.ToString()
                };

            // licensed area

            if (sol.Location?.LicencedArea != null)
            {
                location.AdoxioSpecialeventlocationLicemsedarea =
                    new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();

                location.AdoxioSpecialeventlocationLicemsedarea.Add(new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea()
                {
                    AdoxioDescription = sol.Location.LicencedArea.Description,
                    AdoxioMinorpresent = sol.Location.LicencedArea.MinorsPresent,
                    // Setting - Indoor, Outdoor or Both
                    AdoxioSetting = (int?) sol.Location.LicencedArea.Setting,
                    AdoxioMaximumnumberofguests = sol.Location.LicencedArea.MaxGuests.ToString(),
                    AdoxioNumberofminors = sol.Location?.LicencedArea?.NumberOfMinors.ToString()
                });

            }

            if (sol.Location?.EventDates != null)
            {
                location.AdoxioSpecialeventlocationSchedule = new List<MicrosoftDynamicsCRMadoxioSpecialeventschedule>();
                foreach (var item in sol.Location.EventDates)
                {
                    location.AdoxioSpecialeventlocationSchedule.Add(new MicrosoftDynamicsCRMadoxioSpecialeventschedule()
                    {
                        AdoxioServicestart = item.LiquorServiceStartTime,
                        AdoxioServiceend = item.LiquorServiceEndTime,
                        AdoxioEventstart = item.EventStartDateTime,
                        AdoxioEventend = item.EventEndDateTime
                    });
                }
            }

            if (sol.Location?.Address != null)
            {
                location.AdoxioEventlocationstreet1 = sol.Location.Address.Address1;
                location.AdoxioEventlocationstreet2 = sol.Location.Address.Address2;
                location.AdoxioEventlocationcity = sol.Location.Address.City;
                location.AdoxioEventlocationprovince = sol.Location.Address.Province;
                location.AdoxioEventlocationpostalcode = sol.Location.Address.PostalCode;
            }

            return location;

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
