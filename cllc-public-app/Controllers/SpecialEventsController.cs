using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System.Threading.Tasks;
using System.Web;
using Google.Protobuf.WellKnownTypes;
using Gov.Lclb.Cllb.Public.Extensions;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;
using Gov.Lclb.Cllb.Public.Utils;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/special-events")]
    [ApiController]
    public class SpecialEventsController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IWebHostEnvironment _env;
        private readonly FileManagerClient _fileManagerClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IBCEPService _bcep;
        private readonly IPdfService _pdfClient;

        public SpecialEventsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient, FileManagerClient fileClient, IBCEPService bcep,
            IWebHostEnvironment env, IMemoryCache memoryCache, IPdfService pdfClient)
        {
            _cache = memoryCache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _dynamicsClient = dynamicsClient;
            _logger = loggerFactory.CreateLogger(typeof(ApplicationsController));
            _fileManagerClient = fileClient;
            _env = env;
            _bcep = bcep;
            _pdfClient = pdfClient;
        }

        // get summary list of applications past submission status
        [HttpGet("current/submitted")]
        public IActionResult GetCurrentSubmitted()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            string filter = $"(_adoxio_contactid_value eq {userSettings.ContactId}";

            // accountID will be null if it is a BC Services Card
            if (userSettings.AccountId != null && userSettings.AccountId != "00000000-0000-0000-0000-000000000000")
            {
                filter += $" or _adoxio_accountid_value eq {userSettings.AccountId}";
            }

            filter += $") and statuscode ne {(int)ViewModels.EventStatus.Draft}";
            filter += $" and statuscode ne {(int)ViewModels.EventStatus.Cancelled}";

            var result = GetSepSummaries(filter);

            return new JsonResult(result);
        }

        /// <summary>
        /// GET a special event by id.  Used by the police view event feature.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        [HttpGet("{invoiceId}")]
        public IActionResult GetSpecialEventInvoice(string invoiceId)
        {

            // to do
            string[] expand = new[] { "adoxio_PoliceRepresentativeId",
                "adoxio_PoliceAccountId","adoxio_specialevent_specialeventlocations"
            };

            MicrosoftDynamicsCRMinvoice invoice = null;
            if (!string.IsNullOrEmpty(invoiceId))
            {
                try
                {
                    invoice = _dynamicsClient.Invoices.GetByKey(invoiceId, expand: expand);
                }
                catch (HttpOperationException)
                {
                    //_logger.LogError($"Error retrieving special event: {eventId}.");
                    invoice = null;
                }
            }

            // get the products
            /*
                        if (specialEvent._adoxioContactidValue != null)
                        {
                            specialEvent.AdoxioContactId = _dynamicsClient.GetContactById(specialEvent._adoxioContactidValue).GetAwaiter().GetResult();
                        }
            */
            var result = ""; //invoice.ToViewModel(_dynamicsClient);
            /*
                        if (specialEvent._adoxioLcrbrepresentativeidValue != null)
                        {
                            var lcrbDecisionBy = _dynamicsClient.GetUserAsViewModelContact(specialEvent._adoxioLcrbrepresentativeidValue);
                            result.LcrbApprovalBy = lcrbDecisionBy;
                        }
                        result.LcrbApproval = (ApproverStatus?)specialEvent.AdoxioLcrbapproval;
            */

            return new JsonResult(result);
        }


        /// <summary>
        /// GET a special event by id.  Used by the police view event feature.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("police/{eventId}")]
        public IActionResult GetSpecialEventPolice(string eventId)
        {
            string[] expand = new[] { "adoxio_PoliceRepresentativeId",
                "adoxio_PoliceAccountId",
                "adoxio_Invoice",
                "adoxio_specialevent_licencedarea",
                "adoxio_specialevent_schedule",
                "adoxio_specialevent_specialeventlocations",
                "adoxio_SpecialEventCityDistrictId",
                "adoxio_ContactId",
                "adoxio_specialevent_adoxio_sepdrinksalesforecast_SpecialEvent",
                "adoxio_specialevent_specialeventtsacs"
            };
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = null;
            if (!string.IsNullOrEmpty(eventId))
            {
                try
                {
                    specialEvent = _dynamicsClient.Specialevents.GetByKey(eventId, expand: expand);
                }
                catch (HttpOperationException)
                {
                    //_logger.LogError($"Error retrieving special event: {eventId}.");
                    specialEvent = null;
                }
            }

            if (specialEvent == null)
            {
                return NotFound();
            }
            else
            {
                // get the applicant.

                if (specialEvent._adoxioContactidValue != null)
                {
                    specialEvent.AdoxioContactId = _dynamicsClient.GetContactById(specialEvent._adoxioContactidValue).GetAwaiter().GetResult();
                }

                // get the city

                if (specialEvent._adoxioSpecialeventcitydistrictidValue != null)
                {
                    specialEvent.AdoxioSpecialEventCityDistrictId = _dynamicsClient.GetSepCityById(specialEvent
                        ._adoxioSpecialeventcitydistrictidValue);
                }


                // event locations.

                foreach (var location in specialEvent.AdoxioSpecialeventSpecialeventlocations)
                {
                    // get child elements.
                    string filter = $"_adoxio_specialeventlocationid_value eq {location.AdoxioSpecialeventlocationid}";
                    try
                    {
                        location.AdoxioSpecialeventlocationLicencedareas = _dynamicsClient.Specialeventlicencedareas.Get(filter: filter).Value;

                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error getting location service areas.");
                    }

                    filter = $"_adoxio_specialeventlocationid_value eq {location.AdoxioSpecialeventlocationid}";
                    try
                    {
                        location.AdoxioSpecialeventlocationSchedule = _dynamicsClient.Specialeventschedules.Get(filter: filter).Value;

                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error getting location schedule.");
                    }
                }

            }

            var result = specialEvent.ToViewModel(_dynamicsClient);

            if (specialEvent._adoxioLcrbrepresentativeidValue != null)
            {
                var lcrbDecisionBy = _dynamicsClient.GetUserAsViewModelContact(specialEvent._adoxioLcrbrepresentativeidValue);
                result.LcrbApprovalBy = lcrbDecisionBy;
            }
            result.LcrbApproval = (ApproverStatus?)specialEvent.AdoxioLcrbapproval;

            return new JsonResult(result);
        }


        /// <summary>
        ///     GET a special event by id. The detailed view of the application used by the client before submission and by the summary page
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet("applicant/{eventId}")]
        public IActionResult GetSpecialEventForTheApplicant(string eventId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var specialEvent = this.GetSpecialEventData(eventId);
            if (specialEvent == null)
            {
                return BadRequest();
            }
            if (specialEvent._adoxioContactidValue != userSettings.ContactId && specialEvent._adoxioAccountidValue != userSettings.AccountId)
            {
                return Unauthorized();
            }

            var result = specialEvent.ToViewModel(_dynamicsClient);
            if (result == null)
            {
                return BadRequest();
            }
            return new JsonResult(result);
        }

        /// <summary>
        ///     endpoint for a summary pdf
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("applicant/{eventId}/summary/{filename}")]
        public async Task<IActionResult> GetSummaryPdf(string eventId, string filename)
        {
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = GetSpecialEventData(eventId);

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            var title = "LCRB SPECIAL EVENTS PERMIT";

            string heading = "<h1 class='error'>This is not your special event permit</h1>";

            var appInfo = "<h2 class='info'>General Application Info</h2>";

            appInfo += "<table class='info'>";
            appInfo += $"<tr><th class='heading'>Event Name:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioEventname)}</td></tr>";
            appInfo += $"<tr><th class='heading'>Event Municipality:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioSpecialEventCityDistrictId.AdoxioName)}</td></tr>";
            appInfo += $"<tr><th class='heading'>Applicant Name:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Fullname)}</td></tr>";
            appInfo += $"<tr><th class='heading'>Applicant Info:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Address1Line1)}<br>";
            appInfo += "{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Address1City)},{specialEvent.AdoxioContactId.Address1Stateorprovince}<br>{specialEvent.AdoxioContactId.Address1Postalcode}<br>{specialEvent.AdoxioContactId.Telephone1}<br>{specialEvent.AdoxioContactId.Emailaddress1}</td></tr>";
            appInfo += "</table>";

            var eligibilityInfo = "<h2 class='info'>Eligibility</h2>";
            
            DateTime eventStartDate = DateUtility.FormatDatePacific(specialEvent.AdoxioEventstartdate).Value;
            string eventStartDateParam = eventStartDate.ToString("MMMM dd, yyyy");

            eligibilityInfo += "<table class='info'>";
            eligibilityInfo += $"<tr><th class='heading'>Event Start:</th><td class='field'>{eventStartDateParam}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Organization Type:</th><td class='field'>{(ViewModels.HostOrgCatergory?)specialEvent.AdoxioHostorganisationcategory}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Responsible Beverage Service #:</th><td class='field'>{specialEvent.AdoxioResponsiblebevservicenumber}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Organization Name:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioHostorganisationname)}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Address:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioHostorganisationaddress)}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Occasion of Event:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioSpecialeventdescripton)}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Licence Already Exists At Location?:</th><td class='field'>{(ViewModels.LicensedSEPLocationValue?)specialEvent.AdoxioIslocationlicensedos}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Permit Category:</th><td class='field'>{getPermitCategoryLabel((ViewModels.SEPPublicOrPrivate?)specialEvent.AdoxioPrivateorpublic)}</td></tr>"; // to do
            eligibilityInfo += $"<tr><th class='heading'>Public Property:</th><td class='field'>{specialEvent.AdoxioIsonpublicproperty}</td></tr>";

            eligibilityInfo += "</table>";

            var pageTop = "<div class='page'><div class='page-container'><table style='width:100%; padding:20px 5px;'><tr><td width='20%'>";
            pageTop += "<img width='200' height='189' src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAioAAAILCAYAAADGwLcgAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAVtBJREFUeNrsvXuQZNd933cBAiBA7GIH4lMEUjuIGAAK6ezAFqEqG6ztJU3TlYjepcgSU7Gona04iS3a3MEfIVklpnaQiBWK/2BAhbTyqGxv6KRKNhkMLFcSiiK2t4SUI1ARZl2ERbKgYDYBKZEUiRk8ScLk5n67z6/n9Jn7fnTf7vl8qrpmtx+37z339Dnf83uda65evRoBAAAAdJ";
            pageTop += "FraQIAAABAqAAAAAAgVAAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAAKECAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAAAIFQAAAECoAAAAACBUAAAAAKECAAAAgFABAAAAQKgAAAAAQgUAAAAAoQIAAAAIFQAAAACECgAAAABCBQAAABAqAAAAAAgVAAAAQKgAAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAABCBQAAAAChAgAAAAgVAAAAAIQKAAAAAEIFAAAAECoAAAAACBUAAABAqAAAAAAgVAAAAAAQKgAAAIBQAQAAAECoAAAAAEIFAAAAAKECAAAAgFABAAAAhAoAAAAAQgUAAAAQKgAA";
            pageTop += "AAAIFQAAAACECgAAACBUAAAAABAqAAAAgFABAAAAQKgAAAAAIFQAAAAAoQIAAACAUAEAAACECgAAAABCBQAAABAqAAAAAAgVAAAAAIQKAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAAAgVAAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAAKECAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAAAIFQAAAECoAAAAACBUAAAAAKECAAAAgFABAAAAQKgAAAAAQgUAAAAAoQIAAAAIFQAAAACECgAAACBUaAIAAABAqAAAAAAgVAAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAAKECAAAACBUAAAAAhA";
            pageTop += "oAAAAgVAAAAAAQKgAAAAAIFQAAAJhPrqMJACCJZ373bUvxnxX3X//fxop7vgjb7uEzsH/c/sGvDWhxAEjimqtXr9IKAAdbiKx4QsT+HpnRaV3xRM34gZABQKgAwGKLkp4TIMvu7/E5vIzd+LHlP2IBs8XdBUCoAMB8iRKzkpg4OdbEcV/9+l+IXv2Ge6PnnvzcxPOvWT4Z3XzHqWjniU9Fr+x8Y/z80j0fi65fujv63sUzhY5Tg0tOuAyceNmmFwAsDsSoACyGMOl5j1bcNhIXt7zt16OXv/WVCUFy3c23xa+9Pbr2+sMT75dI0fMht7ztw8PnGxQqx93jrGuPK060DB8IFwCECgBMV5gojuSU";
            pageTop += "EyWnoinFk/ybF781/BsKkiq8svP1Nk/1aPw47R4mXDadaNmkBwEgVACgeXGy7ETJqWhG8SU/MaFywy21jvOqm98cH+vb0zx1CRdZW87G7ag4l4ETLpuxcNmhdwEgVACgnjhZjRqKM2kCuXRe/taj1Qedm2+btlDxkfXppHucj9v4EUQLAEIFAIqLE3PrrHVJnIifvvJ8Y8cyN1IHCEVLH/cQAEIFAPYLFHPrnE57zwxcJhNYAO31t96V+HroEkoKpFXGj5jldeSJFuce6jvRQvozwIyhhD7A7MTJUvxYjx/b8X8fzhIpSgH+2V/6/aFYmfmgcX1yjIpcQvPC60+cj25566+nvSz3kGJanojvzV";
            pageTop += "b8WKW3AswOLCoA0xcoSideyxImIZYlc9Nt74xe+OY/mdm5y/1z7Q3Vs35edfNtw7+zdP1I7MnaUzDzSO43uYU24r969El3Bpjy4ogmAJiaQDkVPwZaqaeJFE2ib/qlLw2LpU0KlW8MJ3cVVpslrzz79VKWk1CQXOeEyk9mKFRe/fqRS+rlZ76yr+3f+J4vjN1TAbKynIsfT8f3sO/EJgAgVAAWQqCseu6dzNRii9248bZ37nvtR9/96lAkXHv9oU5d309feS73errETbe/c2gZ+tH3/nji+cN3fmjYvgWChiUy5RYauK0JAAChAjDXAuV8NKrlMTlhxmJEK/hQeLz09CNDy8NNgVj5oUsJvvG2";
            pageTop += "d81clPixMrKyFMUCcf/NS7MRMGrrm+L2+9F3H9/32mvuODW0APlVd8WhO391GCOUgETnRQQLAEIFYKEEinHN9YeHK/hDd/7axPMvbm+6iXNycrTJVRaBWfHKs6NJ/LrXVAvqtUDcWVlaTOS9/Myj+0SjKu5KJPpIkC3d8/G8NkewACBUAOZCoJxKEyiKe7h+aTKt96XtR4Yr+FCQaBJXoKdW/r615aevvDDcZyd8vouE1zoecG44PNPzspTpH35rMj7F7oGJROPm5VFMUChgTMQgWAAQKgDzIFB6Lkj24SjBgiJR8dp3/PZwZR5ibp7QtfD8Nz7vJtDJ4FnFqYwm3HtnPOHfmyoCRtd8OEXA3N";
            pageTop += "1o4biy3HT7u4YiUKLPFxwSf3ret/Tovh2660NDMRlW4rV08ZTAWxMsfVddGAAQKgAzESjL8UNL8ItRRpCsJsUXYuGhiTyc2F745v80/Hs4nhB9bMV/6M7J518ex6nMxv2TFNtRljIxLY2KFOfeSXL7+OLQuHFouToc36PP7zuWdpGWgAkDcgMUdPu0q5WzxC8GAKECME2Rsh7/UdXSk0XeL0EiS8Ktv/jJfSLmxac3h5YGX8SYm0fWFt+NMnYL3f6umV5/lcJzae6gUgNWDZeXibuXA7ePicHQHSQxonv20tOT7iBZU3Rfnvva54p+tdKaKRwHgFABmIpA6bk4FE0+R4p+zqwqSW4eW7EfCqwq";
            pageTop += "FhcRWlXk/tFK/6YaVpW6FW6tHko5kXHYtcVzlb/Xd9mURRatMKtH4lDXIvHiH1ttO3z+mcnnJZSW/urHh8dRjFEJ5BI87+JXlvklASBUAJoWKEuem+dolWOYVUUrdR9NnBIfipPwBYTcPHq/rCe+JcEKlc3C/ZPj6iiEZQ6VpY5FRp+V8LAYn7F15I7kYFkTh889+bng+V8bCq4S1pQQuQifdhY5AECoADQiUlTufjsq6ObJsgakWVVedO6FcP8ZiRJNjH7tFIkFCZikTf+KckPNfXmqfHfdAOC0AN0iWEXfH3pBscOaKrEIVFv6wbISNbo+iZq04NqS1pQkzskyR3YQAEIFoI5AWXbZPA9GJd";
            pageTop += "w8WaRZVTTxJVlPzC0Uls6XgAnjV0qJph8/1+oGh6+q4Bpq85hmffKDgS1YNiylb9aUMIi2AWtKiCxzyg7aINgWAKECUFakSBkoWPZ4k8f1rSqqeDohYuLnNRH6BeBsjx+t8H1hYS6Mqnv/qDJsVauKbeYXBrZef+ve8ZJiWOz8q2YOXVdRqJjbJ4xDsUwrX5DoHG92FWpfDqwvDVpTQrRT8xb7BwEgVACKCBSLRXm4KStKyJ5V5cMTk31aRVqLnzjsBdVahkrVOBW5NKq6jn764+edAJgUOnmumetqWEQU9Fpwt+P9n3XX6cenSJDo/MPg2rQCby1YU5KsK08QuwKAUAHIEim9qETKcV2rSmg9";
            pageTop += "kXhQrEoYw2ICxhclOoYmbr23qgunTddP0yi+Re6qKpjVybeQmOjzBYlZTUxM+u1kdVNasKaEnCMzCAChApAkUrSSrZzRUxazqmhi9K0qVrPDLwAnASNrQLhRoQXgVk1T1r47VcSKWTZ8V08amtz3WTYqZA4Ng1srfM4sJ2HVWcv28Uvm6zmJR7Wr7yKyAOcWrSkhcjduOfcjAEKFJoADLlCWXMDsuWl+b5pVRZOxJtWwAJyJEt8tVDdORd/z6teXd/+Y68c2GMwqid/U5oNFRFESN42DaL868ZzaPRQvSUG0fszKFKwpPnI7PqxAW36lgFABOLgiRcGL21HDAbN1rSpWyl0xLIbFpPgbElqgrU";
            pageTop += "RNFcuIBEedFGfbYNCP8ciybFRlGJ/ybL34FD+zx8SeXzLfqs1K0PjXMwNrSsjZuJ9ukRUECBWAgydSVuM/T0QtBcwWEgqeVSUUJVYnxSZ4K7U/mmj3LCjjTQorWEaUfVNFqIxdPxlZQ+Euyde95s37LBuFrSLDWifl41OGdVJuG9VJMbeRbUDoiz9hVqkXvXL5M7SmhByToCYrCBAqAAdHpPTjP+e7cC5jq8qdH5oQJS8lFICzYmWHJrJ/HnWTebU4lSrBuFlBrXas62sWlAutIlWq2VqRPN+acpO334/Fochio+8Y7uvjCZKlez42a2uKjwT1E+wXBAgVgMUWKBaPcror52RWlVCUPO8VejNX";
            pageTop += "j5XU9wu9WSaL7xIqbFFxVoYq1hhflCRZTkKqxpjomiR6qlhUTLz5VpxDCdk+ZqF6wXMFSbyoTTtgTQk5T9wKIFQAFlOkyGwukXK8a+cmq4omRIkSm/wV5Gm7/PrBtmZp8a0qL49rqlTbUbmsNUbF4oaipERdFAu8LVsLxcruV4lRsc+ai8c2IPRL5pt7R/gZQOaK64g1JURxK5vErQBCBWDxRMqx8Qp/6a7SFoi2kFXFJkTfqmIrfH9X5XFKsldq/+VnHnUTc5U4la+W3oOnSiaPxaxYxlBRrHZMVmZRovjyMnvMxWOWE98VZPVU1K52XeYK6qA1xUcRwQPECiBUAOZfpJxyImUcNKtV9Ovf2Y";
            pageTop += "9u/cVPduY8NSGGVhW5ZvScJlwrAGeZPv5GhT/63lfH4qXSIBAfq+qeQX4KdRZVY1ZMfBXJLEoSOCbsbAPCoQB0bjU9t7d7cretKbo/b3zPF8L7JOFN6X1AqADMsUhZjRJK4Wvl/NzXPjuMQbCAyS5gE+PP3LsnoGxS9TcxDDcq1PXIciDBUbb4m7libirpNvILuVX5viJIsJmrpiwmSiw+xTYg9EvmW4E3ndNevE73rCkmrK+Jz/Un+9v9qLOsIFYAoQIwhyIlNbPnhW/+k+FqW7Eefrn6Wj+mmq4ks6poojRLxUvDKqmjAFp7zuIr/PTlvQm5nFAxV0xZt5G5SYruaGzBtGXK4JvoKhufMnLr";
            pageTop += "TYoSq/TrB9FanI9fT2Xpr368EWtKU25FHed1931m+O/vP/aRiYq5HkcQK4BQAZgvkbIeipQkF8XuE58arqY1OVV1fUxM+vEkUvc441gV537QMS2mwp6zkvrDCdiLsagiOMzCoc9VmVzDgNprb7gl8TzyNixMomoxOrM0WRtZGf1hO9nGj67Am5+SrOdso8I61hT1gRRBUZoj93x8eE47f/KpPPeXiRXK7gNCBaDjIqUfBeXwFaAq03loOdFk8oM/+o3hv/V6E6tgTSZV995Js6o89+Tn9llQTJiYBUXfG6YuF7OoPOcJg+JBtWnpwkmxKH67lnH92PmULRJnbWI1ZkzM6ThmCbo5MTalfhXaJj";
            pageTop += "d6VL/VecrVFwontaliVoI+bWX3VxkJAKEC0F2Rsq9GitJ/NUH+zC9+cp9Y0QT/bCxWtOKXWGmCUVn76paV0KriW1AsK8hSbv2NCs3yUmbvH0s1LmvBsAJsRT7ji5eilgaJtCpWGLW7WXnMRfaaoOqsxaEIq1djFpa61hTdi7KBv2nHkXBSv9154rf2va5AcLVrWAHYcR6xAggVgDkRKb7lRBaHJDePJjSJAw38t977m41YVfxCbXWsKiZCXggKwE2W1D85YXkoE6fipxrfWMMS1DR+BlM5K4zb22e8N9I7x4LHxN04TTn+v11/E9aUQ3f+6lgc1UH9U0JE/fV7j+7XG+qjCn7W/VesVQqIFUCo";
            pageTop += "AHRIpGz4ImVUzfSufeLBBn1ZTsLX5V7RxCUhoAmnLlqp+/v3VLWqHHFZSZoALdPGCsD9MKhK61tZyrggLKumzOfM9ZOyot+z2MTnbAG3ZTKFfEtNmQDcm8e1Uh6dEHEvDoOSX5go8GZ1apqwpowsQLfU3i1a9/G1931mKK7UX0MLlESKzl/X8+zjn8g73HliVgChAjB7kSL1cdZ/7rX3/XaiGJFY+f4f/qOxmyd8XS6gYXDtPR+vFWfiWyqqHsesKppAzV1lVhUrAGcl9c1KoEnNrCplvtfPqin6OfuMuXXSUoh/4q4htN5k4Qe/Zh0763OqLTO5AeFkvIraVinJEgZ1M33sGBZLVAf1S7XXD4";
            pageTop += "Z98RuJIkV9NEmk6PUEi2CfbCBAqADMVqTsS0HeeeJTqZYTTU4/8GJSfAuC7yIaxQDUzeD57NAiUjXAci9WZeSWeGlcvGyvAFxYUt9cHmXiVNIsGWWoUuI+3Trx9n0CswjjdOZ4IpcousmraitR5xd4s7aVdcpSmataUySM/RTnqkhkSGjJWhKei+63iZQ0d1DKPSd1GRAqAF0SKTaxZbl5NAmYWHnd0Mx+aOKzFlyr4Ns6mUDDDQe/+flxHYy6VhU/LsXEi/3fsn3MoqIJr6hA8mNAim5u6Fs5/PaVKyhsbxM/RV0/VXeCtu+xNtjbgHBzbHXSfdW5y02m6zTrVFVrirkJ6xaH84VIaC3Ra+qL";
            pageTop += "ar+K7iDECiBUAKYsUjTgbmRPvpNiJXRpaGLRwK4JPUxN9oNrtVqug4IdVVG0agXc0Kpi7h8rAGcl9W1i9v9f1I0T7r9TJE3Zt3L42TnDTJSUbJ2irh//+4umJg9L5Ds3j+6rbUDoi7mx1emZrwwn+7rWFAkyuQllOatnQfqFoRBJCp41kaLXkgq+lYhZkVhhI0NAqABMSaQMIq8sfopvfkKsKEAxTE3WwJ4mVhRvMNqw7+21M4F2n/it4SRZJV7Ft6ooNVnXNE5VdsG6Jl5so8IffqvcJoVhTZSy2T9W5C2NMq4vP0unDLbvkSZ0tZG5eKw6rQXM2r2ta02xoFf1Hyu/XwW1zWvfMRLDip/yhY";
            pageTop += "jawhcwaTEraSJlFKMzcS+t3D5iBRAqAC2JFA2wm1Gwd4/QgP36E+f3uS1MrGiwT6qjogFecR1J1pPvP/aPxhsF1imzLwuNxIXiXqrEq4zjKeKJVdfnV6HV8Swl1jYq3AuoLebGCeNLysap5G04aAIhrUhc1ncX+cxIpO3VkknagNCvVivLTl1riu6lrqtOAK2Vx9d5KK7KFzyWojzso0FgrT6XJ1JGmxh+cXiMoA9oI8M+owkgVADaYeBWhfvEhmJONMmFAbKFxIrL9gmtJ1rdjszto8/VK+L22XFMTFWrij6vCdb+P7SqvPXXJwrCafLys4FuvK38jspFq9uWrRhbJOA2tOZYYbl8S8zeJoS2";
            pageTop += "AaGJRL/Am4SL+keduimKS9H36bN10pGtaFtYD0VtP7LwHR72a782i0SHXssSKerfVrwwKaYl5qRL6QdAqAA0aE3pu9Vg6mSuQV0Dv1aSaXVUksSKBvKRaf3rwwnAFyv6nPZZEUkiqChaLZubqUq8SmhVsc31rABcaGX50Xcfn7A0ZOFXp/UtVIUtKrfelfHa3SUsM3ft2zuomEjZu0YFydoGhGY98d1AmvStum8Va4pEj+JS1I9U8bgqOgeJHfW5XZelFooUWVn88zORYuImSaTouOrfSgv/i997d1bG1FkKwgFCBaA5kbIWpVSdDcXKd770/rGoSCqXnyVW/tJZT0KxouNqJZ6UJVRKbDxpKb";
            pageTop += "Hl41VCq4ptrjc63q+Ni70J1QqxgmdFAmOTrAJl3D8qdFbktbx6KEnfWSRTyKwwEibX3HDL3gaE8WQ+UeDNWVNuDlKUi1/noXE8iSxkVTceVL+TRUft8ZdegKzOzURKaGUpIlLUZ3VctUOKJSXkPJlAgFABqC9SevGfB4u+fyxGfvzcUIzY6rmIWNGEba9pMvM/q/1WNAFootCOtlXQ8W1yrFKnxbeqXI2vb5zNEv/fT13WxK2CZ6MJ7nClIN4i6c1prp9XeVYRv3JtXj2UJCvOTwoIFYtHUayRFXQbW1e8/ytNuY41RTFMFteSUbo+12pkBeYUPGsi0Y9XCYVInkixDQrNHfS9i2fKiCiCawGh";
            pageTop += "AlBDpCxHo+DZUmhC/O6X3j80q2uFGWbthGLFFyR+ppA+6wsZP7g2FEBFkbtA31ulTotvVZFYCgvAWbaP3Cc3xJOa1UcpksWTtJ9OUYHjW0L0b999kxdouydu3lz4vRPf7W1eKOEUbkDo/1/WlqrWFN1vu846FWx9t44Fz/pCJKyjkidSzFWk12UxKlBWP2RYY4XRBhAqANVIzPApgsWd2P49WnGGhd1MrIRiRq8p3kX4VhcLrk0SMWXOy/aYqWKdsUlS16TYEhMYOp/Jkvonx+4ff4O/1PP68fMJQqV4IG7anj/2fK7b5/VvLyygkqwplsK9twHho+MCbyOB+PlxbFBZa4rtZGxiqGpxtyS3Ti";
            pageTop += "hS/DoqZUSK+mvSLssFOUZwLSBUAMpbUzRwHqtzjJGwODtRK8V3t/hiJSkuJUms+CImaSfmolYVi70om/ocZvxY6XYrAGdWlpuGacqPjy0uVc5TFoQsi48vItKsIfZ8XsZPWtBvngvDrEUjYXJyLIok2qzAm8TFyAX2rtIWEVl6LE149Nlqxd2sPH4YPCt3ki9S7Hp9kaLzT6pWq6BxIfFctzJuNAquZQNDQKgAFBQpGjDPNnU8DfJWabasWLE6HH56slW11eT32grBtZqM/MmybOqzb1VRLIpZK1QAzlweQ5Fw693j1/KyeNIsF1npzWV2Nc4chLyqsmXws4Qk3uwYEmt+gTe1iRXHK2NN8eNG";
            pageTop += "TPBUKe6mdOabh5tGPu/2knphLF4kBsPndV1veu+XxwJGLkcfWYb8QnB++nJN+s7dCoBQAcgQKRoo+00fVxk3tr+PVqK+FSNLrMicbpO/L3IkfjSJaDK0mhVl8C0jduyigie0qpgrySY9Ex2Hhtk/X5mwPKSLjudLWTqaJE0M5WX83ORVo/XR/brZS0lWQG6V+BK55XxLURVriqUzD/uMV7jNCraFVWf99OQkK4s+N9oq4euJ1WprMiyzzygECBWAnFVdVDEupcgEnxdEmyRWrHqt7bhsYsXer8msSpl9f9IcWWd+u/RnbSM7Q9fkb1RoWSVFi7gliYE0AZUXP+KTVRwuTQzlFVOzz8nFZW4eCz";
            pageTop += "Y2YaJ6M2ZNKRNfYlYQX/yUtaaovffSmT83tnz4Ox0r86eoSLFCby2JFEPxKusMQ4BQAUi2pqheyvE2v0OTTRhEaxNxplhx1Wt9sWIBuyYYygbXatL0J3BNrkWLwflWlaHlxNVRGbqDgmMm/buM9SLN4pEWP5IWWJtnGSmDnyUka5C5eeSqe40nMKzicBmLiG8FGQvDJ8vXXBlldR0e3pu9Gjp7AkgWPhM/WSJFr71hWMRwFFRbsEZKHc5RXwUQKgD7RYoGxgen8V0SJKOqnaNKtL7bJUms6DW/eq2fWhxmCGmSK2dV+Wywkv9QYcFjVhVNxH6peU38Jlx8cZIVp5JVr6Ts3j+HvNol+SIl3bWU";
            pageTop += "tc+P/zn/uhS0e7OXkuxn6xSxiPibBPrWlLKl8v0g2Wdd/9B9NQGkPmPWnSyRov6k1yTELPOnZZFisNMyIFQAAvrT/DJfeGhCeYNXdj8UKyZk/Oq1/o7LFlw7nKDe8dulXCyaPEO3SNFsIt+qIjExtrDctVeZ1qdIEbdEUZCR3lzU/WMZSPutNelCJWufnyQrjNoxPFezuhSxpoTBs1WtKbKKhUGyEik/47KH1FeKiBTbs8fqrlSokVIH7am1zrAECBWAkTVFA+KxaX+vJoTvfOkDw4nDgmLNIuKLFV+U+NVr/ectuLZKIbdwEi2TTeRbVcz9oc/L/ZJUu6RKldqs6rZpQbhNiKAsQZFk5ZEVyX";
            pageTop += "f72LGLWlPC4Nkq1hSJC7MoWfBsKFJMcGSJFFny7DMSO1Ur4dbkrKsMDYBQgQMtUpbjP+dmeQ6aOGwvH3+PoDSx4levteeFb23xa29UsaoUzSYKs4cMP+PHJ82Nk2cZufG25rN/JH5C60UR0mJmtK+Qfzz7dxFrShg8OxKyz0/UPMlDwsPEhQXP+iXz/XoovrXEFynD/YTue2icEaS9qxqokVKHPi4gQKjAQaffhZNQGrIfa2JBrVlixd5vWT9aeSuLYzQJv6vULslJk2nRbKKklFsJnauvJFebTbLU5MU9pFk+imwamCaE8sRP2rGTsoRkqbCCbxMisIA1JSl4Vijtu2g8iGXl2HfKXRRaTKwe";
            pageTop += "illYkkSK3q97pGtvMbOnDHIBrTFMAUIFDqo1pfUsnzJo5fp9ZxWRRUIrW9+CkhSbYmLFAm81Ke64VXiZwNgkq4odN+8YaVaVtGqxN95Wxd2S7P4p6hZJmvDz3D5pAb5J8Slydfl7DGUJwNAKEgbPmjVFFYTLiBTbsFCCJM2t47uB/OfDIm+jvaq+0ZWfBllAgFCBAylSZE5e79p5yVxvokQTYujuSRIrVr3WxIriCSy4tkyZ/R88/huJzxfJJkqyqqS5eaoWcUuyWFRFE3YVt09arExacG2WNcVPIa5jTbHYFvUNCV1tfmgiJS2g1hcpuiZ7v855CunHVWAvIECowIFDA9+RLp6YiRLLCBqtdO";
            pageTop += "9KFSt+9VrbWVmxDX7dlSKBsbJO+CXwfbTqz8rYSbOqFJ3URZFKsP51FM0g0nEltPz3VxVLZWJl8qwptw63Lri7ljVF99piW3b+5FNDK5BlDvlVZ9NEip5/rXv/sEbKxTNdFCnieLy4WGXYgllwzdWrV2kFmLY1pRf/udh5Fe9tDKdJR1kcFiDpr5htMpKryETAqJjXV4fl+pOyOtLQZP6zv/T7KQIq+xj+ZJiHVv7hHjGvP3E+t2aKzuFV3i7FVVCbFfm8rjW0iLz5l/9loc8OLRPxpJ+G4ofS6r3IOlUkJVmWEImMoQXmm58fCiPrL/75p4kUv0Jt0e+cMbvxY/n2D35th1EMsKjAorPelrAo";
            pageTop += "u0Fg9oQ6qrXib0CoSce3rPiVaq16rRi5a94+Dq7V5HUkIWCzjFUlL5uojFWlahaPzqGOSBndp2KfD0WKrDJFP5tlTfFTiKtaU3S/7V4Mg2cDkWJVZ5NEiolCv0Jt0yKlSr2cAsgCSmAtIFRg4a0pGqlbCaCVsNBqv2x12LxjKqXUxIMmHa2Ek8SKvttcRvZePefvy6M02NxJNmPSksUmKxOo6KZ7FsiqCVfnVMSaMm3ilfvQSqXz08RbtOZKVmyK+kaW1alIbMoohfgzE8GzVonWhIdEo9xCoUix+BUrCCfLVpPpx2onXWPZSrolOMcOyzBtcP3AtIXKdjRKeWwNDdaH4xXz89/8fKMDtiZMS2";
            pageTop += "PVZKgJ6lWu3onvBtJfc/kI1cJQKXdzCyW5XUJ8t0ASfgn2kDf90pcSM2BCNMkWed+8keQyMlFm9yrNmqKtFfKEii/qdG9lnfHL9kvY+vfPREpSX2kys0f9U/09r281wIVYRK4ymgEWFVhEkbLetkgRGqwV4Cqx4u+QXBdl81g6siYqTTq+ONkLnD08fm44scXPaaVulpZREOddOd/1+czXszKBilpVFlGkpFlTsjJ8ylhTJEBMpKgvFBUpN44zyEbxSt9pMP1Y/UCWJwmUKYgUcZqKtYBQgUUUKUpHnqp/W2JFVoM3vucLjbmDZMXQJGOZP7Kc2CreFyvhc8raeX44ET5fqMy+JrGkuio+afsK";
            pageTop += "lYlVWTTSYlP8+JE0a0pebIriTXxRIsFi/7eqs0kiRWX9wwJvTVj61H/0fTq+4qNadPcksc6oBggVWDQkUsbpyMq6aCngb9+krZXvrS62pIlgW4tPkRjYC6a9O1GsKGV1NKkcHtZU0apd6P2Ka6gy6e5NVOl7AuVZZBYR3Y+kCri671kipYg1xS+Pr++wnbft/3IDJokUBVCbu1DiRntLNZF+LDeP0uaHuzNPaUfloJ8dx6oCCBVYWGuKBIpWgUrD1eDetmCRsFClT7k6NLgXCWgtekx/I0KtsJPEirmLhtVdb3/nWETo/VmBsWnVan2uG8c9HBq3rY65VCDDaNGw+yt3n7WHX+ckixe3N1NfU5";
            pageTop += "vavdS9leCw9vUFiS9StO+TPhO6heoiy6AshLe87cPDjLJpbFZofUptG4gVrCowFQimhWkIFQ1o58LBzyaRodk9XtHK9N72ytDqZ2gykbWjyK66eatMWUYsbkETkoSIH7QpoaJJVAG1SWQFxmpiKrI5oXYOfuXZb6R+x0FDfUqbMhYSKRkiwq+lYyLE/m3xSX6civWrUUXiu3Pvb5l+ZmLIhFDbrh5956E7f23cp6xWTPAbPXH7B782oMcBQgXm3ZqyHaVUoR3uLhsPwJauqYGw7VWiX9tCk5SqyNYVSL7Z3+pqKIbEFyt+TENIVnBlF1OHF4k//xd/K3XS94v4hUJIFg1ZBkORYvdd79H/64oU";
            pageTop += "WQBlQbHqtU301zy0iDh014fGZf21vUNKG12KhUqPXgQIFVgoa0oSqvJ55J6PDS0PijVQ5kqb29uH1WWbEEga3G31qUlLwbNaWRcRKzoHiZWkycCvgArNkmVN8e9niFLM9ZpvafFdQk2kHw93dfYsM9rschoiXtdlv8NnXeG6HLCqAEIFFsOaInfP1R8/l7kaDFdyEhB13TNpJJn167qDfGuNrah9saLJRkIlKbgzq0R+0doo0Iw1JWs7gvAe6r6pr1q121ERuI9UFim+W9T6kW3f0Ba+KKog3LGqAEIFFsOaIhfG9bfeHb0UrzyzirH5/niRY3quLVb87xKK91Bqc9XvG9a18Nw+4d42WWJF3/";
            pageTop += "39x86WmjihGmn7AWUVhku6d7pn5h4quqdTEaFux/vBcGuGb7TSBr7rVaTEoUz8XhS3ovMKhBNWFUCowFwKFVlTxgXe5MLQqtMPPFW5+DRBEA6iee+vg1911sRFnQDfvCqomvDSMnPSNqjDqtIsSUGumojf8J4vJraz+t8NsdBOS3WWYJHlo0p/8V2fvpBS2nMb8Sih1SZPnNv7tY2BWTsDkUe1WkCowNyJFA1a59MsDgoONAGS5+LR+1UHxQZxTeRtZAhpsrg1qF5aJ15Gg/vr7vtMbg2PIpNo1m6/UJ0w7kOpv2n3K2vLgarpx6EYr3u8PMJMnjx3ZyhoLPYq5fdwRyxWtulVgFCBeREqW/Gf";
            pageTop += "Y1nvSRIsMj2n+eL97AezeDS966wmDgWuhhNS1XiZMA6mKBaM+ZN4cqzyeShOUun7MqRZwPL6RehyzLL0NIHvVsrLSCq7mHA8FAsVdlcGhArMhUjpxX8uFn1/OChmWTE0wOu9fuBi0xlCWeKiavxKlUlQk8lPf/wc7p4pEMYRFaWsqDCLhh+H4p+DUp6bDh73M3lMWKVZJEP3bEmBvhs/lmOxskOPAoQKdF2o9OM/p+3/iq344bcezd3NOEmwyMKi4NtwUJVJ+mfu/eT4vU0VcCuy4hV5QYdJ4L5ZLGFTVlSEgsGnbqZQ2u/Jd5lmxXiF51ZEoOg38uo33BtaQM/EQqVPDwGECnRZpCgl+VlfUP";
            pageTop += "hxGhoANWBmrUKT0jPTAlvDwVjHV9xBUwN+lrio4n4ie2cxREqZGilhPZSQuplCeYI/TXSYdec1d5wsJVB0fL/QXZDifTkWKiv0EkCoQJeFinzUD+YNbhrs89KUkwSLyqInrQolAPx6JU1mCOWJi7Lup/BcYX4oIyrSAmV9mqw0G/5e0vplkvtpuHiIH2kCRZ+58bZ3RYfjzxRYdNwTi5UtegsgVKCrQmU78lKSkwbTm5dPTQySeUG04QBsA6s+469qw4wG0VSGUFgbJW0SK+p+yktfhvkVKUn9NQn1X8U71SWM20qzQKb9jvJKBOi4lpac9tsLIFUZECrQWZEik+8TZSwV/gpNK8CXnn5kuJNt";
            pageTop += "0sCZNNAmmar1PrlsrAhXUxlCaRlBIUUDEHU8WWrI6Ok+RdKFQ8GQRVMbFSZZRkLhYWKjqEBJsp7Yb7Og6N+NhcoSvQYQKtBFodKPvCBaDZBF/PihW0gou0YDY5KVJSw+ZeIgNEOHsQFNZAiVSTcukiGUVWAMukFateAswZBGU5k9RYJfw1iVPIGSZD3Js3ZmQFAtIFSgk0JFaYlHbCCVtUDiQBk/GiDzREtScF+WlSVpgkgSI+GgXmKztdTzVABv0q66aavxrMkBF1C3SXP5lBEoJlKa2KjQDx5XX96NxbAvJNTfJfqLCpTw/VmxYGnWl5tuf2coyh+Jhcopeg8gVKBLIkWD0sPhABaaj4uKlq";
            pageTop += "TBNs3KkiZYfFN10nvqbnpYtjZKOFlgTZkvsfKdL31gon+WCYjW5//ysY/U2j/Kt44kuTNDQW7vSRL5skoevvNDQ0um/5tJKweQJk5892pCAblbqakCCBXoklDpR57bJxwUVUjK38ytqGhJ8q+nDahJYiQMLKwSUJiFjuUH75YRLFXL68Ns0H2TuE2rhVLWIlOEtP5qmUJF+nzeAiDNvWOxXhL8P/ru44niRJYX/Y5TXEO4fwChAp0SKmO3TxYSHhoob4yFi2++ThMtYWBs0uQRplWmmeR9QZLkv69SxM0Gf2qjQFr/rJJ+nBSY69cISsqey8r2Ca0nee6dLIuRLJsvP/NokVgvaqoAQgU6I1Im";
            pageTop += "3D5FMdESmp+HK7R4EFXVy6L+/yQrS1LMSyhu0szlZVOakzY0BERK2Y0F09yYFlOVZGFJ67NJ1hOL4fphLDaS+rcff/WnT307+tfx4/1/+xfG3/OdL72/rOUR9w8gVKATQqUfpbh9ykz0srL4GQfGF/+PP44+0//y8N8fWX33eODMmiBCK0tS+XKLUZFpvkhQbhHhRWAsiCobFSaJZvVP7e6clBmXJFDM0hKKc1lB9N6keKykmJPzX/jD6JOf/b3hv297463RJ/7h34nefd9bq2Qt4f4BhAp0QqgUcvvk4acTP/fCy1H/C48NRcq3vvPsxPt+/ud+djhw/uLKz+VaWcLVY5Jgsa3r9b5wf5+yGU";
            pageTop += "KIFShbIyXM5BHmhtRvIXRRJvVrCX2JE99FmlX9OUmcDH/Lf/Fs9NFP/W70+OX/Z9953nvs347OxgsF/e5KCDGyfwChAjMXKb2oxE7JSfhxKBoo+/FqTgLl+Rd/mPk5f+DMwvzxfjXNJMFibqcnX/rrw++XGLrl0E0T1pcigiXc3wgQKWkCJRQh6mc/ePw34tfentg/fStfmvUkzb2TJk6MLz/25FCk5P3ufvk9fy36yOrfil77kz8ZivgCLlLcP4BQgZkKlY34z9kqn/UDBv9o68+G4uB//dL/Xfo4f/NvvHUoKm5/06257/WtJxpg/aBBiaTf/G/+efQH/+eTY8vN7/0P9098vmiGUJnCcDD/";
            pageTop += "lHGJJAWIWz2Ua+J+mCdQkqwnIsm9kydOhKyXH/3UPx33+6J85PS7o1/9938ueuWJ/zyv3MD7YqGySS8BhArMSqhsRxl7+6Rx6M5fHYqUr/zR/zv0hyeZmstiK70igsWsLOJru38lPofHEgdqHfPTH//gvueLZHMgVg6OSClSyE39IXQtWhzKtdffsi9w3Bco+qxiVCTqfRGTVCslS5xIzIxSrD88/C4tECRSQvdqUQ7ffONwkfCuN/7BMJYmBfb+AYQKzEykLMd/ni7zGa0Gj8SryUf+8M+HAbJVB8gsZAn55b/9C9G773tbpmjRIP1QfA55IilNrBTJEKKo2+KT5+5JS5eXde/Hz359XwC5L1";
            pageTop += "CSgmjts7IM2vfmiROlE8uKqGOZSJH1sP/FxxppAwXc/pen3xz9Oy//46TfwpVYqCzTUwChArMQKlolnS/yXg2419zxn0b/y6UXhxaUPD94U2gAVbaCMoV+/i1vHj4nX3xZK06aWDHBklCVczyBvOm9Xya4doEJq9b6lKlga7tv62+S9UT4GW1Z4kTH0HtViE2WljDtWLEof/pnf954W7z9rW+KPnzvH0fLr/5X4Ut3xGJlm94CCBWYtlDpRzlpyfLHv/DG/zj6778yEgjTEihJyEyt4NiqVpwssWIr4XDflbJl9mExrCpmOSxrSZNbxg+uNSFs7p2rP34uVZyk7Yvl1/jx047b5D/4a9dGH7r7";
            pageTop += "K9GbDr9sT90fC5UNegogVGDaQiU1LVkruK9f8yvR//6nb4we/vLlhblmBRCqlkveZDOqcPt89Mb3fPFA9g0FaFrG1EFA9/ovfu/dienEdbBYqCxxklbV2Q9Wz0o7botDr7ku+sC/9/9d/cDPf+2aQ69+hTRlQKjA1EWKSmM/kfTaV198b/TPLt8ePf6vrizktf/Wx34lt+icTV4HzeWjCfEffKI//Cvrk9xuBwWJhqqxSIrXUtab2mv1A+/IjK2yQPCwqKGP6vm89r7PDM+naNpxW9xw3dUf/cO//uQLv/7A//Y6Rk5AqMA0hcpqFMSn/MG374v+x3/55ujb33th4a+/qFg5SMitoAnXnxBX33";
            pageTop += "/fMCsEkknLulGNoPcPA8LfOmGZKlLG3jbKrJp23CJ3PHXx09vcdUCowLSESj/+c/qFH10fffHrfyX6wuV/K3r+pVcOVBsgVlxfyHErKAvrH//maqG08YNCURGhuCqJlTMfeMc4GFwok0fxKH48lKwo2hxT7qe6acctcSYWKn3uPiBUYDorwf/u3md+54/e+uon/vxNr3vh5Z8c2HY46GKlqFvB6m0g7JItT0WwtHu1oVlZLID2p68810raccNciIXKKqMnIFSgdd5y4qNL8Z9naYkR//OD/1luGf+DahEIUeaUvzXBQUJpwf9VLCKaKm4oweL3uzbTjhvicixUVhgxAKEC0xAqvajm/j6LhKwF";
            pageTop += "v/Obpw+MWKkbnKnaNmov35Wx6MiC8pkLX278uGpLCZbDh26sZKWZNrFQuYYRAxAqMA2hsh7/OUdLTPIbH37vMJZgUZEVRW6FKvsxHcT2EooVUZt12MoxbU7EYmVAM0AZrqUJoAKYbxNQIa2//4kLwwl9ESfc9/69jcZEirXXf7T2OwvZXibq/u79/y0ihbEDECowA5ZpgmQUs6EJXfECizbhtpFBoniN4//hfz0UQosm6joa0MrYAQgVOBAcownS0YQuS4EKeM0zElt/N76OtidcxVVICEkQzbuoU+xOW6JuQcCiAqUhRgVKQSBtOeY1y6Wt4M885rXmyqyrv84TBNQCQgXaFirar+NhWmIxJ9";
            pageTop += "8upLjOU82VDlZ/nQeoUAsIFWhVqKxHZPxUmny7vvdN1UJkbQo8pXwrjfnfjR9dS2fGilIZMn+gFNfRBFASfMwV0GT2D/6LC53c+0bBnw/FAmWaO+sWQVad0LKjPXBMtMhCNYvaNVhRGhlDECqAUIHWWKIJqqPA1H/91LeHBc9mHbeiPXo+0//9RlOO20ZiKhRUsrz8vBMvfnn5Nuia1YkxBA4CuH6gFG858dGd+M8RWqIes6xmK4tA/wuPDSfdRZtw26p6m7fxIpTi0lMXP92jGaAopCdDWRApDWApuRIL00Qp06pbooyeRbQKKC34vf/JRqPtqmO99+89iEgBmBFYVKAwbznxUfmWn6AlmuVv";
            pageTop += "/o23Rp/++K+06rJQHIriKg5SfQ/Fs9RxsWFFaY3dpy5+GvcPFAaLCpSBwaUF2qxmq8lWxecOYhGyOlVvsaK0ClZZKAUWFSjMW058dFVjOC3RDk3WD2l6A8F5p2i2FVaUqUEtFSgMFhUowzJN0B6KGfnYb/3T4URZB2WlyJKASNlD2VaykGRZrbCiMJYAQgUACiCBoQlTq/syLHqgbF1UkyVpDyZzj2k3Z9oNAKECrIKg4KQqsaLqp3koBkMTrawxbIaXjVmt/v4nLgzdY1hRZgaFI6EwFHwDhEqHJ1VVs/3I6XdHH1l9977X57FgW1dQALOsT1hQZgaB+YBQAVgU5Mr5v7b+bJxqawXbZrG78a";
            pageTop += "IJQQBAqABAA1iq7ZkPvGMhK8oCACBUoAmWaYLZWgCwosCCQIwKFIZgWijDUZoAABqAGBVAqAAAAABCBQAAAAChAgAAAAgVAAAAAIQKAAAAAEIFAAAAECoAAAAACBUAAABAqAAAAAAgVAAAAAAQKgAAAIBQAQAAAECowAy4RBMAQAMMaAJAqAAAAABCBQAAAAChAgAAAAgVgAwGNAEAMJYAQgUAAAAAoQIl2aEJAAAAoQJdZYsmAIC6PHXx0wNaARAqAAAAgFCBAwUWFQCoy2WaABAq0ApPXfw0MSoAUBfGEUCoAKshAOgsWGYBoQKshgCAMQQQKnAwGdAEAMAYAtPiOpoAOroauhI/tr3v3Eo4";
            pageTop += "jywT8lL8WMl57ji3E2BhxxBAqMABpQn/8mVPaOw4QaLHzlMXP92k/3qzyJvecuKjy/GfZfffXvAXMQPQIA3/xgGhAlBZqFzyxIgJka0uZg7F52TnJwYJQsYsMSZo7N/H6A4ApRcpAKW45urVq7QClCKeuCU2jniCxCZ6TfLbbuI/KG1h4qWHgAHI5Uw8PvRpBkCoQNuT8zDOAxNuZhv1oj3rix64kOAgI0vKKmMGIFQAui1efOHSQ7zAAeGBWKCs0wyAUAGYT/Gy4okXLC+wSGBFAYQKwIKKl160Z3XR36O0CswRu/FjPRYoGzQFIFQADoZwWQ6EC1YX6CoPOZFCrRRAqAAccPHS88SLHkdoFZ";
            pageTop += "ghF5xA2aYpAKECAEnCZdkTLXrgLgIECiBUAKCzwmUpEC7UdgEECiBUAKDT4sUXLsS5QFkUJNuPHxsIFECoAMA0hIsf46IHcS6QhDYIXY8fmwTJAkIFAGYpXJYj4lxgD7l3+rE4GdAUgFABgC4KFz/OhbTog4GKtKn+CdYTQKgAwFyKF1+46C/uosUQJ30nTrZpDkCoAMAiCZflaNLqQnYR4gQAoQIAnRUuS9FkFV39xeoye5SxM5AwiXDrAEIFAGBCvCxH7Bo9Cy45cTIgIBYQKgAA5cQLu0a3J0y2nDjBagIIFQCABsXLcjRpedH/SZFO5oonSrawmABCBQBgdgKm58SLL2QOUtyLLCXbJkqc";
            pageTop += "MMFaAggVAIA5EDDLnoBZmmMRc8UTIxIhA/2fjBwAhAoALK6IiTwBY4ImmoGYueT+7jghEnmCBDECgFABAEgVNCZkfHolDmGCwwfxAYBQAQAAgIPAtTQBAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAABCBQAAAAChAgAAAFCM62gCaIKUwllGr8Sh/AJaFM4CADjgUPANqgqTtfjPg1P6usvR3oZsm4gXAICDA64fqMrSFL/rWPw47YTR07FI2oofq9wCAIDFB4sKVCIWCsvR5K61+vepKH+zt9340Y8fm+7/tknbivd3peCxtOPs6lMXPz3gjgAAIFQA8sSLBItEw7GMt70vFhabBY+3Gv9Zyz";
            pageTop += "meeCA+5jp3AABg8cD1A40RiwUFweYJhp0Sx+vHD1lX7s9567lY1CBUAAAWECwq0DixaMjqVCequGriY/biPxdz3nYCN1Ct+yaLmKxdg65ZqOJzkytQfWCDYGqAgwXpydAGikM50uQBJUDiyUqWlaxMo340ipUpMumttHj9shoNs5SclWleUJsc1yNuI513vwvn7+5X3/Wp9S42nBN5PfewOKu038Al93fg+onS8LcYNgCSwaICbQzaAzfhJVHL6pFzbHFP3qBf4BhNcsVZKfpNTEYNnvu+++DcZ+c63LWuxOe87J2vRMHFBo57KT5ur+L90OdWo1FWWl0uxOexOoX7X/k32FCb3x9//wYjJRQF";
            pageTop += "iwrMG/2cgfpUtJdJlHWMQTSZsXS0wHc/4kTHdsJry96K2j+W/n1Wj3iQ10p6vaZ7ShYO1ZU5VvHzu9FkUT2fXsfv/XaKdSLLepHH5QL9JWnC1v3WZHsyoY9suf4VsuLa+GRG381jy/XZYzWut46VbKlm/6v7/XAAwaICjdOyRUUD5bNNr47dSnEjYwDOXe2WONZD8bHWGmjnZbeaz7OCPOIEUp6lKWkwuBLtFdszsbCdcog0y4KOsZ7xueVoz2XXyxAemdldzkW0njOJSqip7TerurVcFeZBcI4STKtF4mdcH153AjbknqKWtxL3/4FoZNHbbvh3PtPvB4QKQCeFijv+VsZkVMeMr4nu4abOOz";
            pageTop += "7eRspkVEr4FPie9YzJYsJdkiOuLgYr37Uy15whIksLsxTRkZuG7s5hO8PCckbZZDXaOkmkVLqXrs03g2OVPr/4OP0o3fXUeup+Tv+jdADUhvRkmEfaMh1vVXwtETc5P5Ty8mknZJqgX/E1n1OBSOmVFWYZFoqdCm236awrV7ynBwXPYTPj9ToiZSnJklJVcLr2Dd09yxUOtVnxtabI+o5BBIBQAWiGLNN0VTeBEyuXU14+61bVrZ13CYFl57HrRMpOB+5Hkbo8SaS1x+Wap7QR7bfUbNS8xkEgZqtko21lHH9rCvdpK+f6ABAqcOBYmrMVXNZku9bQd1xJeT5XcLhYA3OxrHUspbqKRWC7alvk";
            pageTop += "tNHphIl4s6H+sVugb1cRqgAIFYAZcKzhia3tFeemNxGFnGzoa+pMVuZ+uFLHNdJS2+1kiLA22iKvjXwuNXiNm/ysARAqsAC4IMtUq0KHC2cNMq6pN2MxIPfFHVF305PXo1HmyCwtBytTuMYTUXMWNoCFgToqMG+sVnxt1khAnezqyXXZfdARK89ywnPHG27/bX7eAPvBogJzg7OmpE32Zwjcgxn0yVVaAQChAmDukX6GSOnP8eVRqbP7bKc8v+7SlgGgJXD9QNcFiiYB+e2TCkop3XR1TjZ0W055fpcN6eaCQZRcVE1bJFjNFwBAqMACkLv6dKmgPfdILck+Z1aUtImMbI/5QPfpfMprx1015l";
            pageTop += "Nztls2AEIFIIG+K/ntD+jL0Z7FIS9A0QqSbc/LBTu3Vdqmh+t0ie4jARLfxwtReql69dstxVFhIQNAqMB8o8qeZ2t+XhOCVrhaxW52eRXrXFdp1UsfoFjXXCEX5KkofR8hidEnnKBZw7oC0AwE08K8ih2tbGWKf1YWGucu6iJpuyhfYLO2+cIJj9UCb1Xf3NZmfQTaAtQHiwpMm0tutZloHneCQw8V2Oq5x5ECE4M2+evMTq3uOvpRsiur9G7C0Bmxshnf2zNReryKL6YVAL7mNp/cwMICUA0sKjBt1nM2MdtWPRRVS40f8vdrRfq+qFi58nMKapzlKlYCxW17v5UgUnQNJxApcy9WJEAlVnYL";
            pageTop += "vN0Ey9Ysaq5Mo+rxrCsrw+KDRQXmYhUb/9n0aqkczXi7xIHESqM7ALtVcVqGjqw/y9HI+pPk5nnEragH3M3FEStxn9hy/fFYgY+oz553Inatoc0Mi3Ax/k5uGCBUAKY0OUiASBRINJzOeOsxN4GcavDrz0blgoBlPVlHnCx0f9xy4jmtzk+aYHk4/pzE6yruIACECize5DAMaHSrxCyxclKm9gZrrah2y5Z7CLmXVtwjKYZGlp3VKGMzQliY/rjuUu7Xc/rkRP+MRgG3q1OwrqgwYluCaCkqZlECQKjAgWPNiYSsQVITR1NCZcPtMjyBl36cNEEpwHeb7J4DIVi2nYA2wVJkw0IJXFlX2twC4t";
            pageTop += "a2rTYucPxpegG0BcG0MM8r2byg1KNuI8Mm2Eo7j/ixGo3iUJI41+HUaWi+XyoQvBeNAsCvFPzY+bYCbafhWqIWECBUADImhSg/G+jUlE5HE01aFkifu3Xg+qYKEUqg3h8Vyw46T/YMAEIFFpM8EbA8pYlJK9e0CrTHG7TswHwJlg3XBx8q0pcpEAeAUIHFY5Dz+vFpnYiLRUkz929wqw6sWNlxtXNORNnWFWUEUWMHAKECCzYJbHfslNbTJiFXQwMObl+VqF6ORlk4aazSUgAIFVg8LnXlRFz2Rtr5rGHaP/BiRS5CuQF3MwTtCi0FgFCBg8PuDL5zPeV5paPiAkKsbEfZLp4erQSAUIGDw9YM";
            pageTop += "JqJBlJ6ufJoVMzjLW1o8E1Y3AIQKLBjHuyRUHFkrZqwqc4BtMNniZoKbtDIAQgUOwGSS85b+jFbM21F6SirpyvNBLxrt4bPRUmzRNk0MgFCBbg7+TZK12r2sjeNmeK3rUXqMzAaBtZ3HhIRii9pIG95CwAAgVKB7NDY5u4k+S6jMtCZFThE4amZ0H7/8fBsZW8spzw9oegCECrRLVrBor8HvWXcTfhIXXFDrTMkpAre2SPsAZVzLXFqOAmucrCr9KfxOLrF3DgBCBdrnSMZrx5qYnF2A49mUly+7jQK7wnpGO6039B3HO3CdyxWEaxs0KYb9mjgnmwqszbAGrkcAgFCBVlfVRSalfo3jL8UPff";
            pageTop += "58ylseiTpWhyKnCNzpljejm6ZI6LrlpEpbhHEkTe10vJ4g6CtZAbOE/zRS4bNcYuwcDggV6CJFVoTKehmUGcQsVTQaBRqeTniLglbvjwf6U1W3ts8RDEsttkutwNqcDKJpira075q2tSetPY5UmLiThMP5OtshpFgDVVZ/reF2n5ZQ7XWk/8GCcs3Vq1dpBWhiRdVzA23ZSekRNxkkZUAsu4FWxz6WcYwLEgJ1fPtONG1mfI8mklM1v0PHP9nk8QuctzjjrDpt9oEVdx/T3H4XpuGOi89DffDBjLeonXtlxGx8zJ2U67rs+t1miXu1kdAH9BtYrSKwC7T7rrverRbvu64/LVbsivv+7QgAoQIz";
            pageTop += "ECgadFej7JiUttAAqMm3X2Fy73krvRUniI4V/Lgmp21PWG0XFQFuonq6gHCzY2+GE4yznuicl9zf4yXaa+DOfce1W1XL07KzWiyVPA//HBKvr8Z5rLjH0YIfv+RZSwZZLhfXz8/mXNemO96OfyzX1+w8TyaICAmdjZLXveaO2SvRb/3r3a4qXF2br7o2r/r9gy4EusP8cB1NADVYmaJIueQmuIEb6Oqs0NYyLBt5HHOPk955FRr0dc7xQP9ANCoilsbJ4NzCiXy9xOTgown8dHDcqpPFas41FD0Hm/SqspxjPcniuCeufOGadN/WnDVs1QmOIwnXddbETPzeoiJ7o6JYfLDm9UZR9TixlYr33v";
            pageTop += "/+lYgUbECowJToR81kKdiKOEoYwLaqrvwz2AgEwFY0WTOj6LlGUcniXEpXjiey7Sg9Q8ZnkHLu4We3c84jaRKuI/TSJpmsdjQrUJHjFEXX8ECJ8zNxs1y2LZwFYOBZSXpRcWvcricMNxtwwzxQou2XC/a1omwlfH9W/0v6fkQKlALXDwBAAzi3yHLLIhsAoQIAAADQFUhPBgAAAIQKAAAAAEIFAAAAECoAAAAAbUN68gHGVZVMK92+TTVJAACYNWT9HBxB0ov26j6ULXOvomZKs1QNBYmXrbZKcgMAACBUDoY4UQVNe7RVPdbKYg+LTVEWGwAAmgbXz2KJk+VoVOZ7LcrepGwrmqxkaQKj5/4W";
            pageTop += "3b8lLMt9DXcBAAAQKhAKFAkLlVY/nfIW21skr3z3IOHYvSh9jxMAAIBWwfUz/yJlPUq3oMg1s96ES8aJIX1X6i6y8fdgUQEAAIQKjN082tE1aUM0WVBW24gZcYG5gxRhdA9BtgAA0CTUUZlPkSI3zFaKSLkQP1baCmx1QkQi6XLCy0vcHQAAQKgcbJGyGv95OEq2aJyJhcRq2zu2uuPrPHa5IwAA0CYE086fSDmf8rJESn9a5yLLiouPedB7uhclBOQ23AZZReoiUqQBABYLYlQWQ6TcH0/QGzM6LwkDS1F+ID6P9QaPveTEj1xdEijHCn501wmm4aNO3Iw7B33/ds3LWYrPY7OhfrAcPG3F+E";
            pageTop += "KWE94b1blHKd/vp7qPr9fdM5++X+3Yte1aia/fnFUMlJf6X4RBKJhTrjVTdJdg4P3dqmNRdde5knA/y7A8rUWT64+yIveYJRYXLCrzIVJ6GSLlkVmJFJt8PKGy0uD1agA6nfE2i5FJEi9yi510Dx1PwcUbbqIsOwCvZLR96YVBA8c430D71pnwV6PylY19QbMdtO25Ep8/F5/7HdPe2kHtZX2pgnioeq1lsPtxzp3vZff9GxXaarWJ84zPYbtt66YTVfpdH5FgmaZFGRAqsH8ltplhOVid5flpcIjPccOJg6Wa1yqBsp4wEVodmEHa4Oc+a1sFhJPK0Wjkolp357pRQrBooH/A/bvnrAlHC372";
            pageTop += "svt8k1aAE561YjlHzBlWQXjbe1RlPWiLrPo6V1zf3U4QKWHb2jFXoux6PaveOUxrxZ4nUsyCt5UiUvxrXfYeR0sed8tbDJjlI0moH3OPs/H5K7h+vYRgsXO3PpZ3P/x7ve19fhpi0i/LsO7GCFhAcP10X6hkrebOdGEVEZ9j302YV+LzWa4oxvoJ11mpDoxnpj+XMahWTt92omgzyq7+e2oa8TI56eKiVbega+uthO9/xLXxToW+sJUxie/Gx1yaYt/ezhEUD8Xns1bx2BJ5D2e85X157sIS1ajXqowVBYpJtt7HMs5tJ7jmE8SoLSZk/XRbpPQyRMqVDpk6+261WGUgtPiPk8HAqkG6V2Xg0e";
            pageTop += "rRxWHc4cROlGBhueisK1GF4w9yVvUb0xownQtnM2NS32j5+7fdRLbP8lIlVsJ9JqsfHXFWjmlZU44W6PtV20737ULGW3ZK9PVlJw4T2yx+nK/Sbu5+ZAmxyzMSKasJwmwtAoQKTJ31iq9NFU3KGizLBmmmpFrLXbLcROCpG8R7GZPBWWcNqsKg4mttsFXy+aYZpAioWfwu2vieR3KEYqNtV7Gva1PQUznCZ8NZYKqIlUspL29GsyFJdJ2scn2AUIF61pTjGSvl/pxfn1ZhYWCoREqv6Towqi0T/7k/5eXTVcRKzgS1fZCESoL16HLNQ9pkcylFJBx1v4+2V+xHp7BS326hr6eJiiNdWuDUuDfL";
            pageTop += "GWPjKrMHQgWmR9bguDnPF+YmgXDPoCttiBRvAN/IWG2ermFZSfqu7Y409c6cfq+/Kk5zK7Q94dqEd6FD97MoWW1zquIxByWfn9XYiPsHoQJTXDGcXEShkpFqPY2KuqsZq/3TLl4GqnGppXs2cCI25HhbZv7AmtmftxuR0WbiiAvAnmdWM16bWgwTIFQOOnkT5mAeLyoj1frCFKP1swaxvjtHqEfde2n3YCvHQrDeskXi0hxnkQwKtO88jiH6/ealS2NVQajAFOhlvHalbctDi2ykDDLr0zoBF1vyUNpqLEp3NcD0sNog1s8lbpP2lTrVtLAMrCnrc9yG2wvaN3wRkmY1OtZ2DBMgVCDb7TOXA5";
            pageTop += "AbOJJqMcwiBiBLjJwmc6BbZKQsH2lh9Wzi5BI1OTo3hvgF7i7nLOhWaTGECrT7Y8xiXgfPtNXp1ONtnDC6UOFcIZ02Moy2C4jL1QZ/e5r45jY2JWA5o//P6xjii9I19zt+KGPBgRsXoQItsbJoF5SRTrjbRL2UimRNRATVlmenob7SSxIqblJKS1VuSqzYca4swL4xadaGC3M6hix5v8srntjaKChsAKEC01gJtbhyneZKqBPXUiAzArHSPVqzqjgxba7J9TlfGEikJFXU3Z3ja/P3lFovIGAb6ReAUIEZrlxnMMgkMZjxeQ0qnDPMqK/npCr3an7Xurda78+xSFnKEHRrc1gTJlzsSGxtFhSw";
            pageTop += "R0lVRqhAO/QW6WLcSjVtv5RZW4cGB+U+zBErnjDZyhAUja2eA2vKvIsUnX+4o7LtndWf0+vyg2g3w6zHHOsoQgWhAlBLeM3aOpQlVI5y62ZCXgBkWqpynWytdW9Cn7v0dAkUZzmQsAszBuUWWZlhLFgT+K7jspWKj5PFh1AByKOzg0SeGZxaDJ28Z1m7K5cOngysKRsdq1G0ktYv3WM9fkiAqB+fD8S1gmZPaKPCOXb3hEG0lzP22EoTsFkiBhAqUJE8d8i8TZ69Gtc6DS7R5TopbLPuS2pQbYWUVD/2oWvWlAfj67kaPuLnL7rHuWhkQUkqorgoqbl+EO1GRQF7ilRlhAo0y85BudA5rrAL7Q";
            pageTop += "uVrH6znSJkjkQlgqDd5LVqK/IF648SMBfja9zRhptzbB3MCqItKmCPRMSqIFRgqixMnZWODJ7bdKm5pIldldeihJTXDqHqq2fix4mEx/vixwPRKAblSsYxdH2nnWgZzFO8hhsfjhUVkjmpytRUmWOuowk6xyAamXQrrzgBoTLHhBsSpk1Km/FEpgk6DHpWSuqpvOBRZ02xyetCR+M41nKqyG5617PirAarUfqmfSq6uBW/d21OMoBWCwjTJAGbtAVJoX4B3QSLyvxNnMdoIlhgwg0J8yalqqvnrltTSqEg0/ix5hYyD2W8Vdd8vuv1RZyQtCDnrCDasB0k7EhVRqhAywPOdpRtyl2kbJQedxxq";
            pageTop += "IKtAUqZHZkpqYE15ZJ6zYhLGjx0nWE5E6VkwkRMrXXYjrxYQpGUF7ElSlecTXD/dZBAl7zRsnIrmZ3NCrYSOp7zWhUFjOWd1BrMhVzxoUnbpuUm/lfWMFfRqVCCTZM4Fy8AtaJ7IeNtm1F1X8sQGhE5U7RT8Pfdyjku8ypyBRaWb5PlR56m8e9aE04UVXdpAfWUB+tE0J6Ha9zJtQ8Ic1lOez9o91yaqS4ssRp275IGMt3SyxHzCXkVyB56NRrF7eY/TUXaxxlVSlREq0MwAk1W8yAaYecn+yZoIjnXAFLtU4bwRKtntOJjib0WC5lKBVblNgqveRLZ+AIaTjZyxpIuLntVgwXCpwiPtmo9E7O";
            pageTop += "M1d+D66fYAk5X9sxbNQXCYVnXx5LAbpWciaPXUn+EpHlsAobLdgXNYnuE16bdyPGXCC8WI/f/yQXDtOfdY31kk0n5/nSEIotUGkcsVj6OFXJrbaz2a4z2dDiJYVOZ3JXR6jgLDNru4ossISi5SXKpLk1HapH58iqfhm9urVhxeKXBNSde/GSW76iZcG4E1ZeMAjSVZfflIx851NUFUVlogRaNtBKKUftGLAKEC9VdCBQbT9a6cr9sYTXuPJJWrzrqOWUbip4mkeaxSuptyX5ancO/9QX+3RtvViR0okqq87q3U+wdoLBnM0emWqURbtU+EgggQKlDzh5YV1Hm6QysDTfpyVT0cBYGVbnVzqYOD";
            pageTop += "xqmuC8ASpFkxptE/egXOo7boyqGf8rljbgM/9bGDFJsydwRBtP26i4Wccec0qcoIFWhmJbRTYBLvdySKfT1nBZc1OaxN+xoSMguMC3NaV2OrpBhrEr+P1lkFL1cVO+63spmxSrdz3D1I1pQ5YzVYpDW12OvaAgkQKgsnVjTpZ1WaPBrN2N+uktzepH854zrS9uE4MoNrWE9ZyXe2xoKzDJTNUjrZpghMEHxNCJUm7+mwDaK9eJ0NRpUJLnWkb+veWxDtpaYWCxnxS1FEPRWECjQqVtYyJnkhM+ZMBmA3wKwXmDBtBbObcQ29aU34UUqWSFdjU5zr4mL8eLakUGl7QPbv/eVZWqNyUpVNiB44oZ";
            pageTop += "Lzu+pK0Lhv3dhosY9OLJC6vpUAIFTmjdU0a4XjrIJZpzwALrmB7kiRgc+JgCxXROtuLHf8fsJLF5rasKyla1jPWgG7tk3Lclhrwx+fIPjqTjCFNiTMIescNuYsSHqpwbEjTbj1OyZUrrSwcWBWXSqsKggVaHC1qAG2lyNWzrmaCdNCk4Jfh2Q3L8PAvX4m5WW5EAYti5V+tD82RSKlyZVVo8X4AtfaoMrKsekJyROpxuUGYj/KbEiY1r+yTP3zZk1potqvBGradhzrJYTbUltiSpmCXv9u3MLjrjGtbx6bo+KZCBWYD7ESP1YyVs5CLpStNiPaXSpy0h4r/YLX0c8QK8faEitOxJ1sWaQ0KlQS";
            pageTop += "XGtZFqvtKD2e6XhTItbdm0E0aUlrsg17DQjoKOE+77TQp5Zb/MmfauA+pfUXbca40UCfbqKvr5UdQyowKPj9gFCBhgTLqpvodzMm+6ddXZNGJ3y3+hgkTPilBhknVt6Xcg0mVlYaOuc0YVVZpOT4/deaOu9o0rV2pcB29xI1lzNE7GadPuHuyVY0aUk7U+C8ihzXqDv59xP61XpD5xZS91yz+tGxqnFbnphMqrx8qYKwbMWiEroP6/ajjPEmy/1zmgJwCBVo54fXd6uZrOBB1TXZdoKl1oDqJnutwJ5IG/zKDjJu8OilTKz6jifqii1nVt4KhNWum1zrWAGyBjZVvuzXPO/lhIlms0Cb5rkIT3";
            pageTop += "p9YqnM+TiLjO7/0UCkNLEKPhW031qN30aYqlw35fxUBUtDUfL64GaZSdQKL0ajLQiSfqcPxG3RK2Ndcn3xWIW2KcJGgsBqi6zxqc9mhd3lmqtXr9IKc44byNaj/JLpj7jJb1BEVLgfbs8NRqdz3n6iTgVMNzGtR8klvS3or1/ivE+5SeB4QhusVZ243LF13AcLvH3XtfeWe+RNDktu4luJki1W9xQVg541Jq9PXPLOcSflfE4lTFS7rh37NfvusmvPpH2tHnD3fLvica2tVsoeo0Tft5T2zZKT/yn3uaLbHFxyvwFdx5bbw2fJE0q9jH4ztqJUaIdT7nd5LGdcWS/ZN+36w+Nedt83aMpVV+I3";
            pageTop += "e9n16UEECBVoVbCsZQxWSYNXFO333/bcJHWswDGuuEF6rYHzX3Lnvxal70FyJZj4twILRy9l8L/kBtNByXNa8VZ9y1H2FvJtUmmDthwBWJULri2rCAhrz6L9y59Edpxw6Zf4rqjEBLru+k/V+5x5ju74q1PsQ1ecWN0oeq9cuu5qhfsTjilrfrs7a+xKjfu+VuI++r/xqvtdnaEwYHdg9+QFwk3CA2/FcipHtByv+GO+5AbAQZM+ZbeC0mC+7lZy9vAn2aPuUUSMXXYibKOG6X8pmu7mfmlsVGzTDeeySVvBtjbppbBTsT2PpYjqrGuv0jfr3Osi59iWSLli1hb3GFS8T3X7+/GK9/2yE4hHUt";
            pageTop += "p0qcF7eKmAYFqOAKECrQoWS8freytLWykWXdXsetaKQbRnct6a0jVsuokx6fyj4Bps1WXnuuXOdbuB85DwOxGIwdZx1+wPzltN9AfPpbHi9YXIsyL4933L3ffGBKnuideeW0XM+86NYxPHdovN3vdExnZFd9FKxjn2XVsOKvaJXsLCpI022KrSBn6fTegvfttuFXXreNdcpv/d485jq4r7yF3HdgSdAdcPAAAAdBayfgAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAECoAAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAIBQAQAAAECoAAAAACBUAAAAAKECAAAAgFABAACAA8J1NAFU4ZnffVsv";
            pageTop += "/nORlgCAkly6/YNf69EMUBQsKgAAAIBQAQAAACgLrh+oynb8eIBmAIAKYwdAYa65evUqrQAAAACdBNcPAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAABCBQAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAECoAAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAIBQAQAAAECoAAAAACBUAAAAAKECAAAAgFABAAAAhAoAAAAAQgUAAAAAoQIAAAAIFQAAAACECgAAACBUAAAAABAqAAAAAAgVAAAAQKgAAAAAIFQAAAAAoQIAAACAUAEAAACECgAAAABCBQAAAAChAgAAAAgVAAAAAIQKAAAAIFQAAA";
            pageTop += "AAECoAAAAACBUAAABAqAAAAAAgVAAAAAChAgAAAIBQAQAAAECoAAAAAEIFAAAAAKECAAAACBUAAAAAhAoAAAAAQgUAAAAQKgAAAAAIFQAAAECoAAAAACBUAAAAABAqAAAAgFABAAAAQKgAAAAAQgUAAAAAoQIAAAAIFQAAAACECgAAAABCBQAAABAqAAAAAAgVAAAAQKgAAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAABCBQAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAECoAAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAIBQAQAAAECoAAAAACBUAAAAAKECAAAAgFABAAAAhAoAAAAAQgUA";
            pageTop += "AAAAoQIAAAAIFQAAAACECgAAACBUAAAAABAqAAAAgFABAAAAQKgAAAAAIFQAAAAAoQIAAACAUAEAAACECgAAAABCBQAAACCb/1+AAQAeXUHaMpCpHwAAAABJRU5ErkJggg==' alt='BC Gov Logo'></td><td width='80%'>";
            pageTop += $"<h1 class='title'>Liquor and Cannabis Regulation Branch</h1>{heading}</td></tr></table>";

            var pageBottom = $"<div class='footer'><div class='footer-box'><p></p></div><div class='issued-box'><p style='text-align:right;'><small>Printed: {DateTime.Today.ToString("MMMM dd, yyyy")}</small></p></div></div></div></div>";

            var locationDetails = "";

            foreach (var location in specialEvent.AdoxioSpecialeventSpecialeventlocations)
            {
                locationDetails += pageTop;
                // draw the location
                locationDetails += $"<h2 class='info'>Event Location: {HttpUtility.HtmlEncode(location.AdoxioLocationname)}</h2>";
                locationDetails += "<table class='info'>";


                locationDetails += $"<tr><th class='heading'>Location Name:</th><td class='field'>{HttpUtility.HtmlEncode(location.AdoxioLocationname)}</td></tr>";
                locationDetails += $"<tr><th class='heading'>Location Description:</th><td class='field'>{HttpUtility.HtmlEncode(location.AdoxioLocationdescription)}</td></tr>";
                locationDetails += $"<tr><th class='heading'>Event Address:</th><td class='field'>{HttpUtility.HtmlEncode(location.AdoxioEventlocationstreet2)} {HttpUtility.HtmlEncode(location.AdoxioEventlocationstreet1)}, {HttpUtility.HtmlEncode(specialEvent.AdoxioSpecialEventCityDistrictId.AdoxioName)} BC, {location.AdoxioEventlocationpostalcode}</td></tr>";
                locationDetails += $"<tr><th class='heading'>Total Attendees:</th><td class='field'>{location.AdoxioMaximumnumberofguestslocation}</td></tr>";

                // show all service areas
                locationDetails += "</table>";
                locationDetails += "<h3 class='info'>Service Area(s):</h2>";
                foreach (var sched in location.AdoxioSpecialeventlocationLicencedareas)
                {

                    var minors = sched.AdoxioMinorpresent.HasValue && sched.AdoxioMinorpresent == true ? "Yes" : "No";

                    locationDetails += "<table class='info'>";
                    locationDetails += $"<tr><th class='heading'>Description:</th><td class='field'>{HttpUtility.HtmlEncode(sched.AdoxioEventname)}</td></tr>";
                    locationDetails += $"<tr><th class='heading'># Guests in Service Area:</th><td class='field'>{sched.AdoxioLicencedareamaxnumberofguests}</td></tr>";
                    locationDetails += $"<tr><th class='heading'>Minors Present?:</th><td class='field'>{minors}</td></tr>";
                    if (minors == "Yes")
                    {
                        locationDetails += $"<tr><th class='heading'># Minors in Service Area:</th><td class='field'>{sched.AdoxioLicencedareanumberofminors}</td></tr>";
                    }
                    locationDetails += $"<tr><th class='heading'>Setting:</th><td class='field'>{(ViewModels.ServiceAreaSetting?)sched.AdoxioSetting}</td></tr>";
                    locationDetails += "</table>";
                }


                // show all event dates
                locationDetails += "<h3 class='info'>Event Date(s):</h3>";
                foreach (var sched in location.AdoxioSpecialeventlocationSchedule)
                {

                    var startDateParam = "";
                    if (sched.AdoxioEventstart.HasValue)
                    {
                        
                        DateTime startDate = DateUtility.FormatDatePacific(sched.AdoxioEventstart).Value; 

                        startDateParam = startDate.ToString("MMMM dd, yyyy");
                    }

                    var eventTimeParam = "";
                    if (sched.AdoxioEventstart.HasValue && sched.AdoxioEventend.HasValue)
                    {
                        DateTime startTime = DateUtility.FormatDatePacific(sched.AdoxioEventstart).Value;
                        DateTime endTime = DateUtility.FormatDatePacific(sched.AdoxioEventend).Value;
                        eventTimeParam = startTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US")) + " - " + endTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US"));
                    }

                    var serviceTimeParam = "";

                    if (sched.AdoxioServicestart.HasValue && sched.AdoxioServiceend.HasValue)
                    {
                        DateTime startTime = DateUtility.FormatDatePacific(sched.AdoxioServicestart).Value;
                        DateTime endTime = DateUtility.FormatDatePacific(sched.AdoxioServiceend).Value;
                        serviceTimeParam = startTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US")) + " - " + endTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US"));
                    }

                    locationDetails += "<table class='info'>";
                    locationDetails += $"<tr><th class='heading'>Date:</th><td class='field'>{startDateParam}</td></tr>\n";
                    locationDetails += $"<tr><th class='heading'>Event Times:</th><td class='field'>{eventTimeParam}</td></tr>\n";
                    locationDetails += $"<tr><th class='heading'>Service Times:</th><td class='field'>{serviceTimeParam}</td></tr>\n";
                    locationDetails += "</table>\n";
                }
                locationDetails += pageBottom;

            }

            var feesInfo = "";

            feesInfo += "<h2 class='info'>Liquor & Fees</h2>";
            feesInfo += "<table class='info'>";
            feesInfo += "<tr><th class='heading' rowspan='4'>Liquor Quantities</th>";
            feesInfo += "<th class='heading center'>Type</th><th class='heading center'>Servings</th><th class='heading center'>Price</th><th class='heading center'>Revenue</th><th class='heading center'>Cost</th></tr>\n";

            //MicrosoftDynamicsCRMinvoice invoice = getSpecialEventInvoice(invoiceId);

            decimal totalRevenue = 0;
            decimal totalPurchaseCost = 0;
            decimal totalProceeds = 0;

            foreach (var forecast in specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent)
            {
                var itemName = "";
                if (forecast.AdoxioName.IndexOf("Beer") >= 0)
                {
                    itemName = "Beer/Cider/Cooler";
                }
                else
                {
                    if (forecast.AdoxioName.IndexOf("Wine") >= 0)
                    {
                        itemName = "Wine";
                    }
                    else
                    {
                        itemName = "Spirits";
                    }

                }
                feesInfo += $"<tr><td class='field center'>{itemName}</td>";
                feesInfo += $"<td class='field center'>{forecast.AdoxioEstimatedservings}</td>";
                feesInfo += $"<td class='field center'>{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", forecast.AdoxioPriceperserving))}</td>";
                feesInfo += $"<td class='field center'>{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", forecast.AdoxioEstimatedrevenue))}</td>";
                feesInfo += $"<td class='field center'>{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", forecast.AdoxioEstimatedcost))}</td></tr>";

                totalRevenue += (decimal)forecast.AdoxioEstimatedrevenue;
                totalPurchaseCost += (decimal)forecast.AdoxioEstimatedcost;
                totalProceeds += (decimal)forecast.AdoxioEstimatedrevenue - (decimal)forecast.AdoxioEstimatedcost;

            }

            feesInfo += "<tr style='background-color:#e0e0e0;'>";
            feesInfo += "<th class='heading fat' style='border-top: solid black 1px;' colspan=5>Estimated revenue</th>";
            feesInfo += $"<td class='field center fat' style='border-top: solid black 1px;'>{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", totalRevenue))}</td</tr>";

            feesInfo += "<tr style='background-color:#e0e0e0'><th class='heading fat' colspan=5>Estimated liquor purchase cost</th>";
            feesInfo += $"<td class='field center fat' >{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", totalPurchaseCost))}</th></tr>";

            feesInfo += "<tr style='background-color:#e0e0e0;'><th class='heading fat' colspan=5>Estimated net proceeds/profit from liquor sales</th>";
            feesInfo += $"<td class='field center fat' >{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", Math.Max(totalProceeds,0)))}</th></tr>";

            feesInfo += "<tr style='background-color:#e0e0e0;'><th class='heading' colspan=5>Total PST Amount Due</th>";
            feesInfo += $"<td class='field center' >{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", specialEvent.AdoxioNetestimatedpst))}</th></tr>";

            feesInfo += $"<tr style='background-color:#e0e0e0;'><th class='heading' colspan=5>Application Fees (Based on {specialEvent.AdoxioSpecialeventSpecialeventlocations.Count} event location(s) and capacity)</th>";
            feesInfo += $"<td class='field center'>{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", specialEvent.AdoxioInvoice.Totalamount - specialEvent.AdoxioNetestimatedpst))}</th></tr>";

            feesInfo += "<tr><th class='heading' colspan=5><h1>Total Fees Due Upon Approval</h1></th>";
            feesInfo += $"<td class='field center'> <h1>{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", specialEvent.AdoxioInvoice.Totalamount))}</h1></th></tr>";

            feesInfo += "</table>";

            parameters.Add("title", title);
            parameters.Add("heading", heading);
            parameters.Add("appInfo", appInfo);

            parameters.Add("printDate", DateTime.Today.ToString("MMMM dd, yyyy"));
            parameters.Add("eligibilityInfo", eligibilityInfo);
            parameters.Add("locationDetails", locationDetails);
            parameters.Add("feesInfo", feesInfo);

            var templateName = "sep";

            byte[] data = await _pdfClient.GetPdf(parameters, templateName);

            await StoreCopyOfPdf(parameters, templateName, specialEvent.AdoxioSpecialeventid, data, "Summary");

            return File(data, "application/pdf", $"SEP Summary.pdf");


            //return new UnauthorizedResult();
        }

        private string getPermitCategoryLabel(ViewModels.SEPPublicOrPrivate? value) {
            string test = value?.ToString();
            var res = "";
            switch (test) {
                case "Members":
                    res = "Private – An organization's members or staff, invited guests and ticket holders";
                    break;
                case "Family":
                    res = "Private – Family and invited friends only";
                    break;

                case "Hobbyist":
                    res = "Private – Hobbyist competition";
                    break;
                case "Anyone":
                    res = "Public – Open to the general public or anyone who wishes to participate or buy a ticket";
                    break;
            }
            return res;
        }

        /// <summary>
        ///     endpoint for a permit pdf
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("applicant/{eventId}/permit/{filename}")]
        public async Task<IActionResult> GetPermitPDF(string eventId, string filename)
        {
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = GetSpecialEventData(eventId);

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            var title = "LCRB SPECIAL EVENTS PERMIT";

            var issued = specialEvent.Statuscode == 845280003;

            // if special event is not issued...
            if (!issued)
            {
                return NotFound();
            }

            var issuedDateParam = "";
            // get the date issued
            try
            {
                
                DateTime issuedDate = DateUtility.FormatDatePacific(specialEvent.AdoxioDateissued).Value;
                issuedDateParam = issuedDate.ToString("MMMM dd, yyyy");
            }
            catch (HttpOperationException)
            {
                issuedDateParam = "ERROR";
            }

            var heading = $"<h1>SPECIAL EVENT PERMIT: {specialEvent.AdoxioSpecialeventpermitnumber}<br>Issued: {issuedDateParam}</h1>";



            var appInfo = "<h2 class='info'>General Application Info</h2>";

            appInfo += "<table class='info'>";
            appInfo += $"<tr><th class='heading'>Event Name:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioEventname)}</td></tr>";
            appInfo += $"<tr><th class='heading'>Event Municipality:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioSpecialEventCityDistrictId.AdoxioName)}</td></tr>";
            appInfo += $"<tr><th class='heading'>Applicant Name:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Fullname)}</td></tr>";
            appInfo +=
                $"<tr><th class='heading'>Applicant Info:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Address1Line1)}<br>";
            appInfo += $"{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Address1City)}, {HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Address1Stateorprovince)}<br>{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Address1Postalcode)}<br>{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Telephone1)}<br>{HttpUtility.HtmlEncode(specialEvent.AdoxioContactId.Emailaddress1)}</td></tr>";
            appInfo += "</table>";

            var eligibilityInfo = "<h2 class='info'>Eligibility</h2>";

            var eventStartDateParam = "";
            DateTime eventStartDate = DateUtility.FormatDatePacific(specialEvent.AdoxioEventstartdate).Value;

            eventStartDateParam = eventStartDate.ToString("MMMM dd, yyyy");

            eligibilityInfo += "<table class='info'>";
            eligibilityInfo += $"<tr><th class='heading'>Event Start:</th><td class='field'>{eventStartDateParam}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Organization Type:</th><td class='field'>{(ViewModels.HostOrgCatergory?)specialEvent.AdoxioHostorganisationcategory}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Responsible Beverage Service #:</th><td class='field'>{specialEvent.AdoxioResponsiblebevservicenumber}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Organization Name:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioHostorganisationname)}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Address:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioHostorganisationaddress)}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Occasion of Event:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioSpecialeventdescripton)}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Licence Already Exists At Location?:</th><td class='field'>{(ViewModels.LicensedSEPLocationValue?)specialEvent.AdoxioIslocationlicensedos}</td></tr>";
            eligibilityInfo += $"<tr><th class='heading'>Permit Category:</th><td class='field'>{getPermitCategoryLabel((ViewModels.SEPPublicOrPrivate?)specialEvent.AdoxioPrivateorpublic)}</td></tr>"; // to do
            eligibilityInfo += $"<tr><th class='heading'>Public Property:</th><td class='field'>{specialEvent.AdoxioIsonpublicproperty}</td></tr>";
            eligibilityInfo += "</table>";

            var pageTop = "<div class='page'><div class='page-container'><table style='width:100%; padding:20px 5px;'><tr><td width='20%'>";
            pageTop += "<img width='200' height='189' src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAioAAAILCAYAAADGwLcgAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAVtBJREFUeNrsvXuQZNd933cBAiBA7GIH4lMEUjuIGAAK6ezAFqEqG6ztJU3TlYjepcgSU7Gona04iS3a3MEfIVklpnaQiBWK/2BAhbTyqGxv6KRKNhkMLFcSiiK2t4SUI1ARZl2ERbKgYDYBKZEUiRk8ScLk5n67z6/n9Jn7fnTf7vl8qrpmtx+37z339Dnf83uda65evRoBAAAAdJ";
            pageTop += "FraQIAAABAqAAAAAAgVAAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAAKECAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAAAIFQAAAECoAAAAACBUAAAAAKECAAAAgFABAAAAQKgAAAAAQgUAAAAAoQIAAAAIFQAAAACECgAAAABCBQAAABAqAAAAAAgVAAAAQKgAAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAABCBQAAAAChAgAAAAgVAAAAAIQKAAAAAEIFAAAAECoAAAAACBUAAABAqAAAAAAgVAAAAAAQKgAAAIBQAQAAAECoAAAAAEIFAAAAAKECAAAAgFABAAAAhAoAAAAAQgUAAAAQKgAA";
            pageTop += "AAAIFQAAAACECgAAACBUAAAAABAqAAAAgFABAAAAQKgAAAAAIFQAAAAAoQIAAACAUAEAAACECgAAAABCBQAAABAqAAAAAAgVAAAAAIQKAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAAAgVAAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAAKECAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAAAIFQAAAECoAAAAACBUAAAAAKECAAAAgFABAAAAQKgAAAAAQgUAAAAAoQIAAAAIFQAAAACECgAAACBUaAIAAABAqAAAAAAgVAAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAAKECAAAACBUAAAAAhA";
            pageTop += "oAAAAgVAAAAAAQKgAAAAAIFQAAAJhPrqMJACCJZ373bUvxnxX3X//fxop7vgjb7uEzsH/c/sGvDWhxAEjimqtXr9IKAAdbiKx4QsT+HpnRaV3xRM34gZABQKgAwGKLkp4TIMvu7/E5vIzd+LHlP2IBs8XdBUCoAMB8iRKzkpg4OdbEcV/9+l+IXv2Ge6PnnvzcxPOvWT4Z3XzHqWjniU9Fr+x8Y/z80j0fi65fujv63sUzhY5Tg0tOuAyceNmmFwAsDsSoACyGMOl5j1bcNhIXt7zt16OXv/WVCUFy3c23xa+9Pbr2+sMT75dI0fMht7ztw8PnGxQqx93jrGuPK060DB8IFwCECgBMV5gojuSU";
            pageTop += "EyWnoinFk/ybF781/BsKkiq8svP1Nk/1aPw47R4mXDadaNmkBwEgVACgeXGy7ETJqWhG8SU/MaFywy21jvOqm98cH+vb0zx1CRdZW87G7ag4l4ETLpuxcNmhdwEgVACgnjhZjRqKM2kCuXRe/taj1Qedm2+btlDxkfXppHucj9v4EUQLAEIFAIqLE3PrrHVJnIifvvJ8Y8cyN1IHCEVLH/cQAEIFAPYLFHPrnE57zwxcJhNYAO31t96V+HroEkoKpFXGj5jldeSJFuce6jvRQvozwIyhhD7A7MTJUvxYjx/b8X8fzhIpSgH+2V/6/aFYmfmgcX1yjIpcQvPC60+cj25566+nvSz3kGJanojvzV";
            pageTop += "b8WKW3AswOLCoA0xcoSideyxImIZYlc9Nt74xe+OY/mdm5y/1z7Q3Vs35edfNtw7+zdP1I7MnaUzDzSO43uYU24r969El3Bpjy4ogmAJiaQDkVPwZaqaeJFE2ib/qlLw2LpU0KlW8MJ3cVVpslrzz79VKWk1CQXOeEyk9mKFRe/fqRS+rlZ76yr+3f+J4vjN1TAbKynIsfT8f3sO/EJgAgVAAWQqCseu6dzNRii9248bZ37nvtR9/96lAkXHv9oU5d309feS73errETbe/c2gZ+tH3/nji+cN3fmjYvgWChiUy5RYauK0JAAChAjDXAuV8NKrlMTlhxmJEK/hQeLz09CNDy8NNgVj5oUsJvvG2";
            pageTop += "d81clPixMrKyFMUCcf/NS7MRMGrrm+L2+9F3H9/32mvuODW0APlVd8WhO391GCOUgETnRQQLAEIFYKEEinHN9YeHK/hDd/7axPMvbm+6iXNycrTJVRaBWfHKs6NJ/LrXVAvqtUDcWVlaTOS9/Myj+0SjKu5KJPpIkC3d8/G8NkewACBUAOZCoJxKEyiKe7h+aTKt96XtR4Yr+FCQaBJXoKdW/r615aevvDDcZyd8vouE1zoecG44PNPzspTpH35rMj7F7oGJROPm5VFMUChgTMQgWAAQKgDzIFB6Lkj24SjBgiJR8dp3/PZwZR5ibp7QtfD8Nz7vJtDJ4FnFqYwm3HtnPOHfmyoCRtd8OEXA3N";
            pageTop += "1o4biy3HT7u4YiUKLPFxwSf3ret/Tovh2660NDMRlW4rV08ZTAWxMsfVddGAAQKgAzESjL8UNL8ItRRpCsJsUXYuGhiTyc2F745v80/Hs4nhB9bMV/6M7J518ex6nMxv2TFNtRljIxLY2KFOfeSXL7+OLQuHFouToc36PP7zuWdpGWgAkDcgMUdPu0q5WzxC8GAKECME2Rsh7/UdXSk0XeL0EiS8Ktv/jJfSLmxac3h5YGX8SYm0fWFt+NMnYL3f6umV5/lcJzae6gUgNWDZeXibuXA7ePicHQHSQxonv20tOT7iBZU3Rfnvva54p+tdKaKRwHgFABmIpA6bk4FE0+R4p+zqwqSW4eW7EfCqwq";
            pageTop += "FhcRWlXk/tFK/6YaVpW6FW6tHko5kXHYtcVzlb/Xd9mURRatMKtH4lDXIvHiH1ttO3z+mcnnJZSW/urHh8dRjFEJ5BI87+JXlvklASBUAJoWKEuem+dolWOYVUUrdR9NnBIfipPwBYTcPHq/rCe+JcEKlc3C/ZPj6iiEZQ6VpY5FRp+V8LAYn7F15I7kYFkTh889+bng+V8bCq4S1pQQuQifdhY5AECoADQiUlTufjsq6ObJsgakWVVedO6FcP8ZiRJNjH7tFIkFCZikTf+KckPNfXmqfHfdAOC0AN0iWEXfH3pBscOaKrEIVFv6wbISNbo+iZq04NqS1pQkzskyR3YQAEIFoI5AWXbZPA9GJd";
            pageTop += "w8WaRZVTTxJVlPzC0Uls6XgAnjV0qJph8/1+oGh6+q4Bpq85hmffKDgS1YNiylb9aUMIi2AWtKiCxzyg7aINgWAKECUFakSBkoWPZ4k8f1rSqqeDohYuLnNRH6BeBsjx+t8H1hYS6Mqnv/qDJsVauKbeYXBrZef+ve8ZJiWOz8q2YOXVdRqJjbJ4xDsUwrX5DoHG92FWpfDqwvDVpTQrRT8xb7BwEgVACKCBSLRXm4KStKyJ5V5cMTk31aRVqLnzjsBdVahkrVOBW5NKq6jn764+edAJgUOnmumetqWEQU9Fpwt+P9n3XX6cenSJDo/MPg2rQCby1YU5KsK08QuwKAUAHIEim9qETKcV2rSmg9";
            pageTop += "kXhQrEoYw2ICxhclOoYmbr23qgunTddP0yi+Re6qKpjVybeQmOjzBYlZTUxM+u1kdVNasKaEnCMzCAChApAkUrSSrZzRUxazqmhi9K0qVrPDLwAnASNrQLhRoQXgVk1T1r47VcSKWTZ8V08amtz3WTYqZA4Ng1srfM4sJ2HVWcv28Uvm6zmJR7Wr7yKyAOcWrSkhcjduOfcjAEKFJoADLlCWXMDsuWl+b5pVRZOxJtWwAJyJEt8tVDdORd/z6teXd/+Y68c2GMwqid/U5oNFRFESN42DaL868ZzaPRQvSUG0fszKFKwpPnI7PqxAW36lgFABOLgiRcGL21HDAbN1rSpWyl0xLIbFpPgbElqgrU";
            pageTop += "RNFcuIBEedFGfbYNCP8ciybFRlGJ/ybL34FD+zx8SeXzLfqs1K0PjXMwNrSsjZuJ9ukRUECBWAgydSVuM/T0QtBcwWEgqeVSUUJVYnxSZ4K7U/mmj3LCjjTQorWEaUfVNFqIxdPxlZQ+Euyde95s37LBuFrSLDWifl41OGdVJuG9VJMbeRbUDoiz9hVqkXvXL5M7SmhByToCYrCBAqAAdHpPTjP+e7cC5jq8qdH5oQJS8lFICzYmWHJrJ/HnWTebU4lSrBuFlBrXas62sWlAutIlWq2VqRPN+acpO334/Fochio+8Y7uvjCZKlez42a2uKjwT1E+wXBAgVgMUWKBaPcror52RWlVCUPO8VejNX";
            pageTop += "j5XU9wu9WSaL7xIqbFFxVoYq1hhflCRZTkKqxpjomiR6qlhUTLz5VpxDCdk+ZqF6wXMFSbyoTTtgTQk5T9wKIFQAFlOkyGwukXK8a+cmq4omRIkSm/wV5Gm7/PrBtmZp8a0qL49rqlTbUbmsNUbF4oaipERdFAu8LVsLxcruV4lRsc+ai8c2IPRL5pt7R/gZQOaK64g1JURxK5vErQBCBWDxRMqx8Qp/6a7SFoi2kFXFJkTfqmIrfH9X5XFKsldq/+VnHnUTc5U4la+W3oOnSiaPxaxYxlBRrHZMVmZRovjyMnvMxWOWE98VZPVU1K52XeYK6qA1xUcRwQPECiBUAOZfpJxyImUcNKtV9Ovf2Y";
            pageTop += "9u/cVPduY8NSGGVhW5ZvScJlwrAGeZPv5GhT/63lfH4qXSIBAfq+qeQX4KdRZVY1ZMfBXJLEoSOCbsbAPCoQB0bjU9t7d7cretKbo/b3zPF8L7JOFN6X1AqADMsUhZjRJK4Wvl/NzXPjuMQbCAyS5gE+PP3LsnoGxS9TcxDDcq1PXIciDBUbb4m7libirpNvILuVX5viJIsJmrpiwmSiw+xTYg9EvmW4E3ndNevE73rCkmrK+Jz/Un+9v9qLOsIFYAoQIwhyIlNbPnhW/+k+FqW7Eefrn6Wj+mmq4ks6poojRLxUvDKqmjAFp7zuIr/PTlvQm5nFAxV0xZt5G5SYruaGzBtGXK4JvoKhufMnLr";
            pageTop += "TYoSq/TrB9FanI9fT2Xpr368EWtKU25FHed1931m+O/vP/aRiYq5HkcQK4BQAZgvkbIeipQkF8XuE58arqY1OVV1fUxM+vEkUvc441gV537QMS2mwp6zkvrDCdiLsagiOMzCoc9VmVzDgNprb7gl8TzyNixMomoxOrM0WRtZGf1hO9nGj67Am5+SrOdso8I61hT1gRRBUZoj93x8eE47f/KpPPeXiRXK7gNCBaDjIqUfBeXwFaAq03loOdFk8oM/+o3hv/V6E6tgTSZV995Js6o89+Tn9llQTJiYBUXfG6YuF7OoPOcJg+JBtWnpwkmxKH67lnH92PmULRJnbWI1ZkzM6ThmCbo5MTalfhXaJj";
            pageTop += "d6VL/VecrVFwontaliVoI+bWX3VxkJAKEC0F2Rsq9GitJ/NUH+zC9+cp9Y0QT/bCxWtOKXWGmCUVn76paV0KriW1AsK8hSbv2NCs3yUmbvH0s1LmvBsAJsRT7ji5eilgaJtCpWGLW7WXnMRfaaoOqsxaEIq1djFpa61hTdi7KBv2nHkXBSv9154rf2va5AcLVrWAHYcR6xAggVgDkRKb7lRBaHJDePJjSJAw38t977m41YVfxCbXWsKiZCXggKwE2W1D85YXkoE6fipxrfWMMS1DR+BlM5K4zb22e8N9I7x4LHxN04TTn+v11/E9aUQ3f+6lgc1UH9U0JE/fV7j+7XG+qjCn7W/VesVQqIFUCo";
            pageTop += "AHRIpGz4ImVUzfSufeLBBn1ZTsLX5V7RxCUhoAmnLlqp+/v3VLWqHHFZSZoALdPGCsD9MKhK61tZyrggLKumzOfM9ZOyot+z2MTnbAG3ZTKFfEtNmQDcm8e1Uh6dEHEvDoOSX5go8GZ1apqwpowsQLfU3i1a9/G1931mKK7UX0MLlESKzl/X8+zjn8g73HliVgChAjB7kSL1cdZ/7rX3/XaiGJFY+f4f/qOxmyd8XS6gYXDtPR+vFWfiWyqqHsesKppAzV1lVhUrAGcl9c1KoEnNrCplvtfPqin6OfuMuXXSUoh/4q4htN5k4Qe/Zh0763OqLTO5AeFkvIraVinJEgZ1M33sGBZLVAf1S7XXD4";
            pageTop += "Z98RuJIkV9NEmk6PUEi2CfbCBAqADMVqTsS0HeeeJTqZYTTU4/8GJSfAuC7yIaxQDUzeD57NAiUjXAci9WZeSWeGlcvGyvAFxYUt9cHmXiVNIsGWWoUuI+3Trx9n0CswjjdOZ4IpcousmraitR5xd4s7aVdcpSmataUySM/RTnqkhkSGjJWhKei+63iZQ0d1DKPSd1GRAqAF0SKTaxZbl5NAmYWHnd0Mx+aOKzFlyr4Ns6mUDDDQe/+flxHYy6VhU/LsXEi/3fsn3MoqIJr6hA8mNAim5u6Fs5/PaVKyhsbxM/RV0/VXeCtu+xNtjbgHBzbHXSfdW5y02m6zTrVFVrirkJ6xaH84VIaC3Ra+qL";
            pageTop += "ar+K7iDECiBUAKYsUjTgbmRPvpNiJXRpaGLRwK4JPUxN9oNrtVqug4IdVVG0agXc0Kpi7h8rAGcl9W1i9v9f1I0T7r9TJE3Zt3L42TnDTJSUbJ2irh//+4umJg9L5Ds3j+6rbUDoi7mx1emZrwwn+7rWFAkyuQllOatnQfqFoRBJCp41kaLXkgq+lYhZkVhhI0NAqABMSaQMIq8sfopvfkKsKEAxTE3WwJ4mVhRvMNqw7+21M4F2n/it4SRZJV7Ft6ooNVnXNE5VdsG6Jl5so8IffqvcJoVhTZSy2T9W5C2NMq4vP0unDLbvkSZ0tZG5eKw6rQXM2r2ta02xoFf1Hyu/XwW1zWvfMRLDip/yhY";
            pageTop += "jawhcwaTEraSJlFKMzcS+t3D5iBRAqAC2JFA2wm1Gwd4/QgP36E+f3uS1MrGiwT6qjogFecR1J1pPvP/aPxhsF1imzLwuNxIXiXqrEq4zjKeKJVdfnV6HV8Swl1jYq3AuoLebGCeNLysap5G04aAIhrUhc1ncX+cxIpO3VkknagNCvVivLTl1riu6lrqtOAK2Vx9d5KK7KFzyWojzso0FgrT6XJ1JGmxh+cXiMoA9oI8M+owkgVADaYeBWhfvEhmJONMmFAbKFxIrL9gmtJ1rdjszto8/VK+L22XFMTFWrij6vCdb+P7SqvPXXJwrCafLys4FuvK38jspFq9uWrRhbJOA2tOZYYbl8S8zeJoS2";
            pageTop += "AaGJRL/Am4SL+keduimKS9H36bN10pGtaFtYD0VtP7LwHR72a782i0SHXssSKerfVrwwKaYl5qRL6QdAqAA0aE3pu9Vg6mSuQV0Dv1aSaXVUksSKBvKRaf3rwwnAFyv6nPZZEUkiqChaLZubqUq8SmhVsc31rABcaGX50Xcfn7A0ZOFXp/UtVIUtKrfelfHa3SUsM3ft2zuomEjZu0YFydoGhGY98d1AmvStum8Va4pEj+JS1I9U8bgqOgeJHfW5XZelFooUWVn88zORYuImSaTouOrfSgv/i997d1bG1FkKwgFCBaA5kbIWpVSdDcXKd770/rGoSCqXnyVW/tJZT0KxouNqJZ6UJVRKbDxpKb";
            pageTop += "Hl41VCq4ptrjc63q+Ni70J1QqxgmdFAmOTrAJl3D8qdFbktbx6KEnfWSRTyKwwEibX3HDL3gaE8WQ+UeDNWVNuDlKUi1/noXE8iSxkVTceVL+TRUft8ZdegKzOzURKaGUpIlLUZ3VctUOKJSXkPJlAgFABqC9SevGfB4u+fyxGfvzcUIzY6rmIWNGEba9pMvM/q/1WNAFootCOtlXQ8W1yrFKnxbeqXI2vb5zNEv/fT13WxK2CZ6MJ7nClIN4i6c1prp9XeVYRv3JtXj2UJCvOTwoIFYtHUayRFXQbW1e8/ytNuY41RTFMFteSUbo+12pkBeYUPGsi0Y9XCYVInkixDQrNHfS9i2fKiCiCawGh";
            pageTop += "AlBDpCxHo+DZUmhC/O6X3j80q2uFGWbthGLFFyR+ppA+6wsZP7g2FEBFkbtA31ulTotvVZFYCgvAWbaP3Cc3xJOa1UcpksWTtJ9OUYHjW0L0b999kxdouydu3lz4vRPf7W1eKOEUbkDo/1/WlqrWFN1vu846FWx9t44Fz/pCJKyjkidSzFWk12UxKlBWP2RYY4XRBhAqANVIzPApgsWd2P49WnGGhd1MrIRiRq8p3kX4VhcLrk0SMWXOy/aYqWKdsUlS16TYEhMYOp/Jkvonx+4ff4O/1PP68fMJQqV4IG7anj/2fK7b5/VvLyygkqwplsK9twHho+MCbyOB+PlxbFBZa4rtZGxiqGpxtyS3Ti";
            pageTop += "hS/DoqZUSK+mvSLssFOUZwLSBUAMpbUzRwHqtzjJGwODtRK8V3t/hiJSkuJUms+CImaSfmolYVi70om/ocZvxY6XYrAGdWlpuGacqPjy0uVc5TFoQsi48vItKsIfZ8XsZPWtBvngvDrEUjYXJyLIok2qzAm8TFyAX2rtIWEVl6LE149Nlqxd2sPH4YPCt3ki9S7Hp9kaLzT6pWq6BxIfFctzJuNAquZQNDQKgAFBQpGjDPNnU8DfJWabasWLE6HH56slW11eT32grBtZqM/MmybOqzb1VRLIpZK1QAzlweQ5Fw693j1/KyeNIsF1npzWV2Nc4chLyqsmXws4Qk3uwYEmt+gTe1iRXHK2NN8eNG";
            pageTop += "TPBUKe6mdOabh5tGPu/2knphLF4kBsPndV1veu+XxwJGLkcfWYb8QnB++nJN+s7dCoBQAcgQKRoo+00fVxk3tr+PVqK+FSNLrMicbpO/L3IkfjSJaDK0mhVl8C0jduyigie0qpgrySY9Ex2Hhtk/X5mwPKSLjudLWTqaJE0M5WX83ORVo/XR/brZS0lWQG6V+BK55XxLURVriqUzD/uMV7jNCraFVWf99OQkK4s+N9oq4euJ1WprMiyzzygECBWAnFVdVDEupcgEnxdEmyRWrHqt7bhsYsXer8msSpl9f9IcWWd+u/RnbSM7Q9fkb1RoWSVFi7gliYE0AZUXP+KTVRwuTQzlFVOzz8nFZW4eCz";
            pageTop += "Y2YaJ6M2ZNKRNfYlYQX/yUtaaovffSmT83tnz4Ox0r86eoSLFCby2JFEPxKusMQ4BQAUi2pqheyvE2v0OTTRhEaxNxplhx1Wt9sWIBuyYYygbXatL0J3BNrkWLwflWlaHlxNVRGbqDgmMm/buM9SLN4pEWP5IWWJtnGSmDnyUka5C5eeSqe40nMKzicBmLiG8FGQvDJ8vXXBlldR0e3pu9Gjp7AkgWPhM/WSJFr71hWMRwFFRbsEZKHc5RXwUQKgD7RYoGxgen8V0SJKOqnaNKtL7bJUms6DW/eq2fWhxmCGmSK2dV+Wywkv9QYcFjVhVNxH6peU38Jlx8cZIVp5JVr6Ts3j+HvNol+SIl3bWU";
            pageTop += "tc+P/zn/uhS0e7OXkuxn6xSxiPibBPrWlLKl8v0g2Wdd/9B9NQGkPmPWnSyRov6k1yTELPOnZZFisNMyIFQAAvrT/DJfeGhCeYNXdj8UKyZk/Oq1/o7LFlw7nKDe8dulXCyaPEO3SNFsIt+qIjExtrDctVeZ1qdIEbdEUZCR3lzU/WMZSPutNelCJWufnyQrjNoxPFezuhSxpoTBs1WtKbKKhUGyEik/47KH1FeKiBTbs8fqrlSokVIH7am1zrAECBWAkTVFA+KxaX+vJoTvfOkDw4nDgmLNIuKLFV+U+NVr/ectuLZKIbdwEi2TTeRbVcz9oc/L/ZJUu6RKldqs6rZpQbhNiKAsQZFk5ZEVyX";
            pageTop += "f72LGLWlPC4Nkq1hSJC7MoWfBsKFJMcGSJFFny7DMSO1Ur4dbkrKsMDYBQgQMtUpbjP+dmeQ6aOGwvH3+PoDSx4levteeFb23xa29UsaoUzSYKs4cMP+PHJ82Nk2cZufG25rN/JH5C60UR0mJmtK+Qfzz7dxFrShg8OxKyz0/UPMlDwsPEhQXP+iXz/XoovrXEFynD/YTue2icEaS9qxqokVKHPi4gQKjAQaffhZNQGrIfa2JBrVlixd5vWT9aeSuLYzQJv6vULslJk2nRbKKklFsJnauvJFebTbLU5MU9pFk+imwamCaE8sRP2rGTsoRkqbCCbxMisIA1JSl4Vijtu2g8iGXl2HfKXRRaTKwe";
            pageTop += "illYkkSK3q97pGtvMbOnDHIBrTFMAUIFDqo1pfUsnzJo5fp9ZxWRRUIrW9+CkhSbYmLFAm81Ke64VXiZwNgkq4odN+8YaVaVtGqxN95Wxd2S7P4p6hZJmvDz3D5pAb5J8Slydfl7DGUJwNAKEgbPmjVFFYTLiBTbsFCCJM2t47uB/OfDIm+jvaq+0ZWfBllAgFCBAylSZE5e79p5yVxvokQTYujuSRIrVr3WxIriCSy4tkyZ/R88/huJzxfJJkqyqqS5eaoWcUuyWFRFE3YVt09arExacG2WNcVPIa5jTbHYFvUNCV1tfmgiJS2g1hcpuiZ7v855CunHVWAvIECowIFDA9+RLp6YiRLLCBqtdO";
            pageTop += "9KFSt+9VrbWVmxDX7dlSKBsbJO+CXwfbTqz8rYSbOqFJ3URZFKsP51FM0g0nEltPz3VxVLZWJl8qwptw63Lri7ljVF99piW3b+5FNDK5BlDvlVZ9NEip5/rXv/sEbKxTNdFCnieLy4WGXYgllwzdWrV2kFmLY1pRf/udh5Fe9tDKdJR1kcFiDpr5htMpKryETAqJjXV4fl+pOyOtLQZP6zv/T7KQIq+xj+ZJiHVv7hHjGvP3E+t2aKzuFV3i7FVVCbFfm8rjW0iLz5l/9loc8OLRPxpJ+G4ofS6r3IOlUkJVmWEImMoQXmm58fCiPrL/75p4kUv0Jt0e+cMbvxY/n2D35th1EMsKjAorPelrAo";
            pageTop += "u0Fg9oQ6qrXib0CoSce3rPiVaq16rRi5a94+Dq7V5HUkIWCzjFUlL5uojFWlahaPzqGOSBndp2KfD0WKrDJFP5tlTfFTiKtaU3S/7V4Mg2cDkWJVZ5NEiolCv0Jt0yKlSr2cAsgCSmAtIFRg4a0pGqlbCaCVsNBqv2x12LxjKqXUxIMmHa2Ek8SKvttcRvZePefvy6M02NxJNmPSksUmKxOo6KZ7FsiqCVfnVMSaMm3ilfvQSqXz08RbtOZKVmyK+kaW1alIbMoohfgzE8GzVonWhIdEo9xCoUix+BUrCCfLVpPpx2onXWPZSrolOMcOyzBtcP3AtIXKdjRKeWwNDdaH4xXz89/8fKMDtiZMS2";
            pageTop += "PVZKgJ6lWu3onvBtJfc/kI1cJQKXdzCyW5XUJ8t0ASfgn2kDf90pcSM2BCNMkWed+8keQyMlFm9yrNmqKtFfKEii/qdG9lnfHL9kvY+vfPREpSX2kys0f9U/09r281wIVYRK4ymgEWFVhEkbLetkgRGqwV4Cqx4u+QXBdl81g6siYqTTq+ONkLnD08fm44scXPaaVulpZREOddOd/1+czXszKBilpVFlGkpFlTsjJ8ylhTJEBMpKgvFBUpN44zyEbxSt9pMP1Y/UCWJwmUKYgUcZqKtYBQgUUUKUpHnqp/W2JFVoM3vucLjbmDZMXQJGOZP7Kc2CreFyvhc8raeX44ET5fqMy+JrGkuio+afsK";
            pageTop += "lYlVWTTSYlP8+JE0a0pebIriTXxRIsFi/7eqs0kiRWX9wwJvTVj61H/0fTq+4qNadPcksc6oBggVWDQkUsbpyMq6aCngb9+krZXvrS62pIlgW4tPkRjYC6a9O1GsKGV1NKkcHtZU0apd6P2Ka6gy6e5NVOl7AuVZZBYR3Y+kCri671kipYg1xS+Pr++wnbft/3IDJokUBVCbu1DiRntLNZF+LDeP0uaHuzNPaUfloJ8dx6oCCBVYWGuKBIpWgUrD1eDetmCRsFClT7k6NLgXCWgtekx/I0KtsJPEirmLhtVdb3/nWETo/VmBsWnVan2uG8c9HBq3rY65VCDDaNGw+yt3n7WHX+ckixe3N1NfU5";
            pageTop += "vavdS9leCw9vUFiS9StO+TPhO6heoiy6AshLe87cPDjLJpbFZofUptG4gVrCowFQimhWkIFQ1o58LBzyaRodk9XtHK9N72ytDqZ2gykbWjyK66eatMWUYsbkETkoSIH7QpoaJJVAG1SWQFxmpiKrI5oXYOfuXZb6R+x0FDfUqbMhYSKRkiwq+lYyLE/m3xSX6civWrUUXiu3Pvb5l+ZmLIhFDbrh5956E7f23cp6xWTPAbPXH7B782oMcBQgXm3ZqyHaVUoR3uLhsPwJauqYGw7VWiX9tCk5SqyNYVSL7Z3+pqKIbEFyt+TENIVnBlF1OHF4k//xd/K3XS94v4hUJIFg1ZBkORYvdd79H/64oU";
            pageTop += "WQBlQbHqtU301zy0iDh014fGZf21vUNKG12KhUqPXgQIFVgoa0oSqvJ55J6PDS0PijVQ5kqb29uH1WWbEEga3G31qUlLwbNaWRcRKzoHiZWkycCvgArNkmVN8e9niFLM9ZpvafFdQk2kHw93dfYsM9rschoiXtdlv8NnXeG6HLCqAEIFFsOaInfP1R8/l7kaDFdyEhB13TNpJJn167qDfGuNrah9saLJRkIlKbgzq0R+0doo0Iw1JWs7gvAe6r6pr1q121ERuI9UFim+W9T6kW3f0Ba+KKog3LGqAEIFFsOaIhfG9bfeHb0UrzyzirH5/niRY3quLVb87xKK91Bqc9XvG9a18Nw+4d42WWJF3/";
            pageTop += "39x86WmjihGmn7AWUVhku6d7pn5h4quqdTEaFux/vBcGuGb7TSBr7rVaTEoUz8XhS3ovMKhBNWFUCowFwKFVlTxgXe5MLQqtMPPFW5+DRBEA6iee+vg1911sRFnQDfvCqomvDSMnPSNqjDqtIsSUGumojf8J4vJraz+t8NsdBOS3WWYJHlo0p/8V2fvpBS2nMb8Sih1SZPnNv7tY2BWTsDkUe1WkCowNyJFA1a59MsDgoONAGS5+LR+1UHxQZxTeRtZAhpsrg1qF5aJ15Gg/vr7vtMbg2PIpNo1m6/UJ0w7kOpv2n3K2vLgarpx6EYr3u8PMJMnjx3ZyhoLPYq5fdwRyxWtulVgFCBeREqW/Gf";
            pageTop += "Y1nvSRIsMj2n+eL97AezeDS966wmDgWuhhNS1XiZMA6mKBaM+ZN4cqzyeShOUun7MqRZwPL6RehyzLL0NIHvVsrLSCq7mHA8FAsVdlcGhArMhUjpxX8uFn1/OChmWTE0wOu9fuBi0xlCWeKiavxKlUlQk8lPf/wc7p4pEMYRFaWsqDCLhh+H4p+DUp6bDh73M3lMWKVZJEP3bEmBvhs/lmOxskOPAoQKdF2o9OM/p+3/iq344bcezd3NOEmwyMKi4NtwUJVJ+mfu/eT4vU0VcCuy4hV5QYdJ4L5ZLGFTVlSEgsGnbqZQ2u/Jd5lmxXiF51ZEoOg38uo33BtaQM/EQqVPDwGECnRZpCgl+VlfUP";
            pageTop += "hxGhoANWBmrUKT0jPTAlvDwVjHV9xBUwN+lrio4n4ie2cxREqZGilhPZSQuplCeYI/TXSYdec1d5wsJVB0fL/QXZDifTkWKiv0EkCoQJeFinzUD+YNbhrs89KUkwSLyqInrQolAPx6JU1mCOWJi7Lup/BcYX4oIyrSAmV9mqw0G/5e0vplkvtpuHiIH2kCRZ+58bZ3RYfjzxRYdNwTi5UtegsgVKCrQmU78lKSkwbTm5dPTQySeUG04QBsA6s+469qw4wG0VSGUFgbJW0SK+p+yktfhvkVKUn9NQn1X8U71SWM20qzQKb9jvJKBOi4lpac9tsLIFUZECrQWZEik+8TZSwV/gpNK8CXnn5kuJNt";
            pageTop += "0sCZNNAmmar1PrlsrAhXUxlCaRlBIUUDEHU8WWrI6Ok+RdKFQ8GQRVMbFSZZRkLhYWKjqEBJsp7Yb7Og6N+NhcoSvQYQKtBFodKPvCBaDZBF/PihW0gou0YDY5KVJSw+ZeIgNEOHsQFNZAiVSTcukiGUVWAMukFateAswZBGU5k9RYJfw1iVPIGSZD3Js3ZmQFAtIFSgk0JFaYlHbCCVtUDiQBk/GiDzREtScF+WlSVpgkgSI+GgXmKztdTzVABv0q66aavxrMkBF1C3SXP5lBEoJlKa2KjQDx5XX96NxbAvJNTfJfqLCpTw/VmxYGnWl5tuf2coyh+Jhcopeg8gVKBLIkWD0sPhABaaj4uKlq";
            pageTop += "TBNs3KkiZYfFN10nvqbnpYtjZKOFlgTZkvsfKdL31gon+WCYjW5//ysY/U2j/Kt44kuTNDQW7vSRL5skoevvNDQ0um/5tJKweQJk5892pCAblbqakCCBXoklDpR57bJxwUVUjK38ytqGhJ8q+nDahJYiQMLKwSUJiFjuUH75YRLFXL68Ns0H2TuE2rhVLWIlOEtP5qmUJF+nzeAiDNvWOxXhL8P/ru44niRJYX/Y5TXEO4fwChAp0SKmO3TxYSHhoob4yFi2++ThMtYWBs0uQRplWmmeR9QZLkv69SxM0Gf2qjQFr/rJJ+nBSY69cISsqey8r2Ca0nee6dLIuRLJsvP/NokVgvaqoAQgU6I1Im";
            pageTop += "3D5FMdESmp+HK7R4EFXVy6L+/yQrS1LMSyhu0szlZVOakzY0BERK2Y0F09yYFlOVZGFJ67NJ1hOL4fphLDaS+rcff/WnT307+tfx4/1/+xfG3/OdL72/rOUR9w8gVKATQqUfpbh9ykz0srL4GQfGF/+PP44+0//y8N8fWX33eODMmiBCK0tS+XKLUZFpvkhQbhHhRWAsiCobFSaJZvVP7e6clBmXJFDM0hKKc1lB9N6keKykmJPzX/jD6JOf/b3hv297463RJ/7h34nefd9bq2Qt4f4BhAp0QqgUcvvk4acTP/fCy1H/C48NRcq3vvPsxPt+/ud+djhw/uLKz+VaWcLVY5Jgsa3r9b5wf5+yGU";
            pageTop += "KIFShbIyXM5BHmhtRvIXRRJvVrCX2JE99FmlX9OUmcDH/Lf/Fs9NFP/W70+OX/Z9953nvs347OxgsF/e5KCDGyfwChAjMXKb2oxE7JSfhxKBoo+/FqTgLl+Rd/mPk5f+DMwvzxfjXNJMFibqcnX/rrw++XGLrl0E0T1pcigiXc3wgQKWkCJRQh6mc/ePw34tfentg/fStfmvUkzb2TJk6MLz/25FCk5P3ufvk9fy36yOrfil77kz8ZivgCLlLcP4BQgZkKlY34z9kqn/UDBv9o68+G4uB//dL/Xfo4f/NvvHUoKm5/06257/WtJxpg/aBBiaTf/G/+efQH/+eTY8vN7/0P9098vmiGUJnCcDD/";
            pageTop += "lHGJJAWIWz2Ua+J+mCdQkqwnIsm9kydOhKyXH/3UPx33+6J85PS7o1/9938ueuWJ/zyv3MD7YqGySS8BhArMSqhsRxl7+6Rx6M5fHYqUr/zR/zv0hyeZmstiK70igsWsLOJru38lPofHEgdqHfPTH//gvueLZHMgVg6OSClSyE39IXQtWhzKtdffsi9w3Bco+qxiVCTqfRGTVCslS5xIzIxSrD88/C4tECRSQvdqUQ7ffONwkfCuN/7BMJYmBfb+AYQKzEykLMd/ni7zGa0Gj8SryUf+8M+HAbJVB8gsZAn55b/9C9G773tbpmjRIP1QfA55IilNrBTJEKKo2+KT5+5JS5eXde/Hz359XwC5L1";
            pageTop += "CSgmjts7IM2vfmiROlE8uKqGOZSJH1sP/FxxppAwXc/pen3xz9Oy//46TfwpVYqCzTUwChArMQKlolnS/yXg2419zxn0b/y6UXhxaUPD94U2gAVbaCMoV+/i1vHj4nX3xZK06aWDHBklCVczyBvOm9Xya4doEJq9b6lKlga7tv62+S9UT4GW1Z4kTH0HtViE2WljDtWLEof/pnf954W7z9rW+KPnzvH0fLr/5X4Ut3xGJlm94CCBWYtlDpRzlpyfLHv/DG/zj6778yEgjTEihJyEyt4NiqVpwssWIr4XDflbJl9mExrCpmOSxrSZNbxg+uNSFs7p2rP34uVZyk7Yvl1/jx047b5D/4a9dGH7r7";
            pageTop += "K9GbDr9sT90fC5UNegogVGDaQiU1LVkruK9f8yvR//6nb4we/vLlhblmBRCqlkveZDOqcPt89Mb3fPFA9g0FaFrG1EFA9/ovfu/dienEdbBYqCxxklbV2Q9Wz0o7botDr7ku+sC/9/9d/cDPf+2aQ69+hTRlQKjA1EWKSmM/kfTaV198b/TPLt8ePf6vrizktf/Wx34lt+icTV4HzeWjCfEffKI//Cvrk9xuBwWJhqqxSIrXUtab2mv1A+/IjK2yQPCwqKGP6vm89r7PDM+naNpxW9xw3dUf/cO//uQLv/7A//Y6Rk5AqMA0hcpqFMSn/MG374v+x3/55ujb33th4a+/qFg5SMitoAnXnxBX33";
            pageTop += "/fMCsEkknLulGNoPcPA8LfOmGZKlLG3jbKrJp23CJ3PHXx09vcdUCowLSESj/+c/qFH10fffHrfyX6wuV/K3r+pVcOVBsgVlxfyHErKAvrH//maqG08YNCURGhuCqJlTMfeMc4GFwok0fxKH48lKwo2hxT7qe6acctcSYWKn3uPiBUYDorwf/u3md+54/e+uon/vxNr3vh5Z8c2HY46GKlqFvB6m0g7JItT0WwtHu1oVlZLID2p68810raccNciIXKKqMnIFSgdd5y4qNL8Z9naYkR//OD/1luGf+DahEIUeaUvzXBQUJpwf9VLCKaKm4oweL3uzbTjhvicixUVhgxAKEC0xAqvajm/j6LhKwF";
            pageTop += "v/Obpw+MWKkbnKnaNmov35Wx6MiC8pkLX278uGpLCZbDh26sZKWZNrFQuYYRAxAqMA2hsh7/OUdLTPIbH37vMJZgUZEVRW6FKvsxHcT2EooVUZt12MoxbU7EYmVAM0AZrqUJoAKYbxNQIa2//4kLwwl9ESfc9/69jcZEirXXf7T2OwvZXibq/u79/y0ihbEDECowA5ZpgmQUs6EJXfECizbhtpFBoniN4//hfz0UQosm6joa0MrYAQgVOBAcownS0YQuS4EKeM0zElt/N76OtidcxVVICEkQzbuoU+xOW6JuQcCiAqUhRgVKQSBtOeY1y6Wt4M885rXmyqyrv84TBNQCQgXaFirar+NhWmIxJ9";
            pageTop += "8upLjOU82VDlZ/nQeoUAsIFWhVqKxHZPxUmny7vvdN1UJkbQo8pXwrjfnfjR9dS2fGilIZMn+gFNfRBFASfMwV0GT2D/6LC53c+0bBnw/FAmWaO+sWQVad0LKjPXBMtMhCNYvaNVhRGhlDECqAUIHWWKIJqqPA1H/91LeHBc9mHbeiPXo+0//9RlOO20ZiKhRUsrz8vBMvfnn5Nuia1YkxBA4CuH6gFG858dGd+M8RWqIes6xmK4tA/wuPDSfdRZtw26p6m7fxIpTi0lMXP92jGaAopCdDWRApDWApuRIL00Qp06pbooyeRbQKKC34vf/JRqPtqmO99+89iEgBmBFYVKAwbznxUfmWn6AlmuVv";
            pageTop += "/o23Rp/++K+06rJQHIriKg5SfQ/Fs9RxsWFFaY3dpy5+GvcPFAaLCpSBwaUF2qxmq8lWxecOYhGyOlVvsaK0ClZZKAUWFSjMW058dFVjOC3RDk3WD2l6A8F5p2i2FVaUqUEtFSgMFhUowzJN0B6KGfnYb/3T4URZB2WlyJKASNlD2VaykGRZrbCiMJYAQgUACiCBoQlTq/syLHqgbF1UkyVpDyZzj2k3Z9oNAKECrIKg4KQqsaLqp3koBkMTrawxbIaXjVmt/v4nLgzdY1hRZgaFI6EwFHwDhEqHJ1VVs/3I6XdHH1l9977X57FgW1dQALOsT1hQZgaB+YBQAVgU5Mr5v7b+bJxqawXbZrG78a";
            pageTop += "IJQQBAqABAA1iq7ZkPvGMhK8oCACBUoAmWaYLZWgCwosCCQIwKFIZgWijDUZoAABqAGBVAqAAAAABCBQAAAAChAgAAAAgVAAAAAIQKAAAAAEIFAAAAECoAAAAACBUAAABAqAAAAAAgVAAAAAAQKgAAAIBQAQAAAECowAy4RBMAQAMMaAJAqAAAAABCBQAAAAChAgAAAAgVgAwGNAEAMJYAQgUAAAAAoQIl2aEJAAAAoQJdZYsmAIC6PHXx0wNaARAqAAAAgFCBAwUWFQCoy2WaABAq0ApPXfw0MSoAUBfGEUCoAKshAOgsWGYBoQKshgCAMQQQKnAwGdAEAMAYAtPiOpoAOroauhI/tr3v3Eo4";
            pageTop += "jywT8lL8WMl57ji3E2BhxxBAqMABpQn/8mVPaOw4QaLHzlMXP92k/3qzyJvecuKjy/GfZfffXvAXMQPQIA3/xgGhAlBZqFzyxIgJka0uZg7F52TnJwYJQsYsMSZo7N/H6A4ApRcpAKW45urVq7QClCKeuCU2jniCxCZ6TfLbbuI/KG1h4qWHgAHI5Uw8PvRpBkCoQNuT8zDOAxNuZhv1oj3rix64kOAgI0vKKmMGIFQAui1efOHSQ7zAAeGBWKCs0wyAUAGYT/Gy4okXLC+wSGBFAYQKwIKKl160Z3XR36O0CswRu/FjPRYoGzQFIFQADoZwWQ6EC1YX6CoPOZFCrRRAqAAccPHS88SLHkdoFZ";
            pageTop += "ghF5xA2aYpAKECAEnCZdkTLXrgLgIECiBUAKCzwmUpEC7UdgEECiBUAKDT4sUXLsS5QFkUJNuPHxsIFECoAMA0hIsf46IHcS6QhDYIXY8fmwTJAkIFAGYpXJYj4lxgD7l3+rE4GdAUgFABgC4KFz/OhbTog4GKtKn+CdYTQKgAwFyKF1+46C/uosUQJ30nTrZpDkCoAMAiCZflaNLqQnYR4gQAoQIAnRUuS9FkFV39xeoye5SxM5AwiXDrAEIFAGBCvCxH7Bo9Cy45cTIgIBYQKgAA5cQLu0a3J0y2nDjBagIIFQCABsXLcjRpedH/SZFO5oonSrawmABCBQBgdgKm58SLL2QOUtyLLCXbJkqc";
            pageTop += "MMFaAggVAIA5EDDLnoBZmmMRc8UTIxIhA/2fjBwAhAoALK6IiTwBY4ImmoGYueT+7jghEnmCBDECgFABAEgVNCZkfHolDmGCwwfxAYBQAQAAgIPAtTQBAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAABCBQAAAAChAgAAAFCM62gCaIKUwllGr8Sh/AJaFM4CADjgUPANqgqTtfjPg1P6usvR3oZsm4gXAICDA64fqMrSFL/rWPw47YTR07FI2oofq9wCAIDFB4sKVCIWCsvR5K61+vepKH+zt9340Y8fm+7/tknbivd3peCxtOPs6lMXPz3gjgAAIFQA8sSLBItEw7GMt70vFhabBY+3Gv9Zyz";
            pageTop += "meeCA+5jp3AABg8cD1A40RiwUFweYJhp0Sx+vHD1lX7s9567lY1CBUAAAWECwq0DixaMjqVCequGriY/biPxdz3nYCN1Ct+yaLmKxdg65ZqOJzkytQfWCDYGqAgwXpydAGikM50uQBJUDiyUqWlaxMo340ipUpMumttHj9shoNs5SclWleUJsc1yNuI513vwvn7+5X3/Wp9S42nBN5PfewOKu038Al93fg+onS8LcYNgCSwaICbQzaAzfhJVHL6pFzbHFP3qBf4BhNcsVZKfpNTEYNnvu+++DcZ+c63LWuxOe87J2vRMHFBo57KT5ur+L90OdWo1FWWl0uxOexOoX7X/k32FCb3x9//wYjJRQF";
            pageTop += "iwrMG/2cgfpUtJdJlHWMQTSZsXS0wHc/4kTHdsJry96K2j+W/n1Wj3iQ10p6vaZ7ShYO1ZU5VvHzu9FkUT2fXsfv/XaKdSLLepHH5QL9JWnC1v3WZHsyoY9suf4VsuLa+GRG381jy/XZYzWut46VbKlm/6v7/XAAwaICjdOyRUUD5bNNr47dSnEjYwDOXe2WONZD8bHWGmjnZbeaz7OCPOIEUp6lKWkwuBLtFdszsbCdcog0y4KOsZ7xueVoz2XXyxAemdldzkW0njOJSqip7TerurVcFeZBcI4STKtF4mdcH153AjbknqKWtxL3/4FoZNHbbvh3PtPvB4QKQCeFijv+VsZkVMeMr4nu4abOOz";
            pageTop += "7eRspkVEr4FPie9YzJYsJdkiOuLgYr37Uy15whIksLsxTRkZuG7s5hO8PCckbZZDXaOkmkVLqXrs03g2OVPr/4OP0o3fXUeup+Tv+jdADUhvRkmEfaMh1vVXwtETc5P5Ty8mknZJqgX/E1n1OBSOmVFWYZFoqdCm236awrV7ynBwXPYTPj9ToiZSnJklJVcLr2Dd09yxUOtVnxtabI+o5BBIBQAWiGLNN0VTeBEyuXU14+61bVrZ13CYFl57HrRMpOB+5Hkbo8SaS1x+Wap7QR7bfUbNS8xkEgZqtko21lHH9rCvdpK+f6ABAqcOBYmrMVXNZku9bQd1xJeT5XcLhYA3OxrHUspbqKRWC7alvk";
            pageTop += "tNHphIl4s6H+sVugb1cRqgAIFYAZcKzhia3tFeemNxGFnGzoa+pMVuZ+uFLHNdJS2+1kiLA22iKvjXwuNXiNm/ysARAqsAC4IMtUq0KHC2cNMq6pN2MxIPfFHVF305PXo1HmyCwtBytTuMYTUXMWNoCFgToqMG+sVnxt1khAnezqyXXZfdARK89ywnPHG27/bX7eAPvBogJzg7OmpE32Zwjcgxn0yVVaAQChAmDukX6GSOnP8eVRqbP7bKc8v+7SlgGgJXD9QNcFiiYB+e2TCkop3XR1TjZ0W055fpcN6eaCQZRcVE1bJFjNFwBAqMACkLv6dKmgPfdILck+Z1aUtImMbI/5QPfpfMprx1015l";
            pageTop += "Nztls2AEIFIIG+K/ntD+jL0Z7FIS9A0QqSbc/LBTu3Vdqmh+t0ie4jARLfxwtReql69dstxVFhIQNAqMB8o8qeZ2t+XhOCVrhaxW52eRXrXFdp1UsfoFjXXCEX5KkofR8hidEnnKBZw7oC0AwE08K8ih2tbGWKf1YWGucu6iJpuyhfYLO2+cIJj9UCb1Xf3NZmfQTaAtQHiwpMm0tutZloHneCQw8V2Oq5x5ECE4M2+evMTq3uOvpRsiur9G7C0Bmxshnf2zNReryKL6YVAL7mNp/cwMICUA0sKjBt1nM2MdtWPRRVS40f8vdrRfq+qFi58nMKapzlKlYCxW17v5UgUnQNJxApcy9WJEAlVnYL";
            pageTop += "vN0Ey9Ysaq5Mo+rxrCsrw+KDRQXmYhUb/9n0aqkczXi7xIHESqM7ALtVcVqGjqw/y9HI+pPk5nnEragH3M3FEStxn9hy/fFYgY+oz553Inatoc0Mi3Ax/k5uGCBUAKY0OUiASBRINJzOeOsxN4GcavDrz0blgoBlPVlHnCx0f9xy4jmtzk+aYHk4/pzE6yruIACECize5DAMaHSrxCyxclKm9gZrrah2y5Z7CLmXVtwjKYZGlp3VKGMzQliY/rjuUu7Xc/rkRP+MRgG3q1OwrqgwYluCaCkqZlECQKjAgWPNiYSsQVITR1NCZcPtMjyBl36cNEEpwHeb7J4DIVi2nYA2wVJkw0IJXFlX2twC4t";
            pageTop += "a2rTYucPxpegG0BcG0MM8r2byg1KNuI8Mm2Eo7j/ixGo3iUJI41+HUaWi+XyoQvBeNAsCvFPzY+bYCbafhWqIWECBUADImhSg/G+jUlE5HE01aFkifu3Xg+qYKEUqg3h8Vyw46T/YMAEIFFpM8EbA8pYlJK9e0CrTHG7TswHwJlg3XBx8q0pcpEAeAUIHFY5Dz+vFpnYiLRUkz929wqw6sWNlxtXNORNnWFWUEUWMHAKECCzYJbHfslNbTJiFXQwMObl+VqF6ORlk4aazSUgAIFVg8LnXlRFz2Rtr5rGHaP/BiRS5CuQF3MwTtCi0FgFCBg8PuDL5zPeV5paPiAkKsbEfZLp4erQSAUIGDw9YM";
            pageTop += "JqJBlJ6ufJoVMzjLW1o8E1Y3AIQKLBjHuyRUHFkrZqwqc4BtMNniZoKbtDIAQgUOwGSS85b+jFbM21F6SirpyvNBLxrt4bPRUmzRNk0MgFCBbg7+TZK12r2sjeNmeK3rUXqMzAaBtZ3HhIRii9pIG95CwAAgVKB7NDY5u4k+S6jMtCZFThE4amZ0H7/8fBsZW8spzw9oegCECrRLVrBor8HvWXcTfhIXXFDrTMkpAre2SPsAZVzLXFqOAmucrCr9KfxOLrF3DgBCBdrnSMZrx5qYnF2A49mUly+7jQK7wnpGO6039B3HO3CdyxWEaxs0KYb9mjgnmwqszbAGrkcAgFCBVlfVRSalfo3jL8UPff";
            pageTop += "58ylseiTpWhyKnCNzpljejm6ZI6LrlpEpbhHEkTe10vJ4g6CtZAbOE/zRS4bNcYuwcDggV6CJFVoTKehmUGcQsVTQaBRqeTniLglbvjwf6U1W3ts8RDEsttkutwNqcDKJpira075q2tSetPY5UmLiThMP5OtshpFgDVVZ/reF2n5ZQ7XWk/8GCcs3Vq1dpBWhiRdVzA23ZSekRNxkkZUAsu4FWxz6WcYwLEgJ1fPtONG1mfI8mklM1v0PHP9nk8QuctzjjrDpt9oEVdx/T3H4XpuGOi89DffDBjLeonXtlxGx8zJ2U67rs+t1miXu1kdAH9BtYrSKwC7T7rrverRbvu64/LVbsivv+7QgAoQIz";
            pageTop += "ECgadFej7JiUttAAqMm3X2Fy73krvRUniI4V/Lgmp21PWG0XFQFuonq6gHCzY2+GE4yznuicl9zf4yXaa+DOfce1W1XL07KzWiyVPA//HBKvr8Z5rLjH0YIfv+RZSwZZLhfXz8/mXNemO96OfyzX1+w8TyaICAmdjZLXveaO2SvRb/3r3a4qXF2br7o2r/r9gy4EusP8cB1NADVYmaJIueQmuIEb6Oqs0NYyLBt5HHOPk955FRr0dc7xQP9ANCoilsbJ4NzCiXy9xOTgown8dHDcqpPFas41FD0Hm/SqspxjPcniuCeufOGadN/WnDVs1QmOIwnXddbETPzeoiJ7o6JYfLDm9UZR9TixlYr33v";
            pageTop += "/+lYgUbECowJToR81kKdiKOEoYwLaqrvwz2AgEwFY0WTOj6LlGUcniXEpXjiey7Sg9Q8ZnkHLu4We3c84jaRKuI/TSJpmsdjQrUJHjFEXX8ECJ8zNxs1y2LZwFYOBZSXpRcWvcricMNxtwwzxQou2XC/a1omwlfH9W/0v6fkQKlALXDwBAAzi3yHLLIhsAoQIAAADQFUhPBgAAAIQKAAAAAEIFAAAAECoAAAAAbUN68gHGVZVMK92+TTVJAACYNWT9HBxB0ov26j6ULXOvomZKs1QNBYmXrbZKcgMAACBUDoY4UQVNe7RVPdbKYg+LTVEWGwAAmgbXz2KJk+VoVOZ7LcrepGwrmqxkaQKj5/4W";
            pageTop += "3b8lLMt9DXcBAAAQKhAKFAkLlVY/nfIW21skr3z3IOHYvSh9jxMAAIBWwfUz/yJlPUq3oMg1s96ES8aJIX1X6i6y8fdgUQEAAIQKjN082tE1aUM0WVBW24gZcYG5gxRhdA9BtgAA0CTUUZlPkSI3zFaKSLkQP1baCmx1QkQi6XLCy0vcHQAAQKgcbJGyGv95OEq2aJyJhcRq2zu2uuPrPHa5IwAA0CYE086fSDmf8rJESn9a5yLLiouPedB7uhclBOQ23AZZReoiUqQBABYLYlQWQ6TcH0/QGzM6LwkDS1F+ID6P9QaPveTEj1xdEijHCn501wmm4aNO3Iw7B33/ds3LWYrPY7OhfrAcPG3F+E";
            pageTop += "KWE94b1blHKd/vp7qPr9fdM5++X+3Yte1aia/fnFUMlJf6X4RBKJhTrjVTdJdg4P3dqmNRdde5knA/y7A8rUWT64+yIveYJRYXLCrzIVJ6GSLlkVmJFJt8PKGy0uD1agA6nfE2i5FJEi9yi510Dx1PwcUbbqIsOwCvZLR96YVBA8c430D71pnwV6PylY19QbMdtO25Ep8/F5/7HdPe2kHtZX2pgnioeq1lsPtxzp3vZff9GxXaarWJ84zPYbtt66YTVfpdH5FgmaZFGRAqsH8ltplhOVid5flpcIjPccOJg6Wa1yqBsp4wEVodmEHa4Oc+a1sFhJPK0Wjkolp357pRQrBooH/A/bvnrAlHC372";
            pageTop += "svt8k1aAE561YjlHzBlWQXjbe1RlPWiLrPo6V1zf3U4QKWHb2jFXoux6PaveOUxrxZ4nUsyCt5UiUvxrXfYeR0sed8tbDJjlI0moH3OPs/H5K7h+vYRgsXO3PpZ3P/x7ve19fhpi0i/LsO7GCFhAcP10X6hkrebOdGEVEZ9j302YV+LzWa4oxvoJ11mpDoxnpj+XMahWTt92omgzyq7+e2oa8TI56eKiVbega+uthO9/xLXxToW+sJUxie/Gx1yaYt/ezhEUD8Xns1bx2BJ5D2e85X157sIS1ajXqowVBYpJtt7HMs5tJ7jmE8SoLSZk/XRbpPQyRMqVDpk6+261WGUgtPiPk8HAqkG6V2Xg0e";
            pageTop += "rRxWHc4cROlGBhueisK1GF4w9yVvUb0xownQtnM2NS32j5+7fdRLbP8lIlVsJ9JqsfHXFWjmlZU44W6PtV20737ULGW3ZK9PVlJw4T2yx+nK/Sbu5+ZAmxyzMSKasJwmwtAoQKTJ31iq9NFU3KGizLBmmmpFrLXbLcROCpG8R7GZPBWWcNqsKg4mttsFXy+aYZpAioWfwu2vieR3KEYqNtV7Gva1PQUznCZ8NZYKqIlUspL29GsyFJdJ2scn2AUIF61pTjGSvl/pxfn1ZhYWCoREqv6Towqi0T/7k/5eXTVcRKzgS1fZCESoL16HLNQ9pkcylFJBx1v4+2V+xHp7BS326hr6eJiiNdWuDUuDfL";
            pageTop += "GWPjKrMHQgWmR9bguDnPF+YmgXDPoCttiBRvAN/IWG2ermFZSfqu7Y409c6cfq+/Kk5zK7Q94dqEd6FD97MoWW1zquIxByWfn9XYiPsHoQJTXDGcXEShkpFqPY2KuqsZq/3TLl4GqnGppXs2cCI25HhbZv7AmtmftxuR0WbiiAvAnmdWM16bWgwTIFQOOnkT5mAeLyoj1frCFKP1swaxvjtHqEfde2n3YCvHQrDeskXi0hxnkQwKtO88jiH6/ealS2NVQajAFOhlvHalbctDi2ykDDLr0zoBF1vyUNpqLEp3NcD0sNog1s8lbpP2lTrVtLAMrCnrc9yG2wvaN3wRkmY1OtZ2DBMgVCDb7TOXA5";
            pageTop += "AbOJJqMcwiBiBLjJwmc6BbZKQsH2lh9Wzi5BI1OTo3hvgF7i7nLOhWaTGECrT7Y8xiXgfPtNXp1ONtnDC6UOFcIZ02Moy2C4jL1QZ/e5r45jY2JWA5o//P6xjii9I19zt+KGPBgRsXoQItsbJoF5SRTrjbRL2UimRNRATVlmenob7SSxIqblJKS1VuSqzYca4swL4xadaGC3M6hix5v8srntjaKChsAKEC01gJtbhyneZKqBPXUiAzArHSPVqzqjgxba7J9TlfGEikJFXU3Z3ja/P3lFovIGAb6ReAUIEZrlxnMMgkMZjxeQ0qnDPMqK/npCr3an7Xurda78+xSFnKEHRrc1gTJlzsSGxtFhSw";
            pageTop += "R0lVRqhAO/QW6WLcSjVtv5RZW4cGB+U+zBErnjDZyhAUja2eA2vKvIsUnX+4o7LtndWf0+vyg2g3w6zHHOsoQgWhAlBLeM3aOpQlVI5y62ZCXgBkWqpynWytdW9Cn7v0dAkUZzmQsAszBuUWWZlhLFgT+K7jspWKj5PFh1AByKOzg0SeGZxaDJ28Z1m7K5cOngysKRsdq1G0ktYv3WM9fkiAqB+fD8S1gmZPaKPCOXb3hEG0lzP22EoTsFkiBhAqUJE8d8i8TZ69Gtc6DS7R5TopbLPuS2pQbYWUVD/2oWvWlAfj67kaPuLnL7rHuWhkQUkqorgoqbl+EO1GRQF7ilRlhAo0y85BudA5rrAL7Q";
            pageTop += "uVrH6znSJkjkQlgqDd5LVqK/IF648SMBfja9zRhptzbB3MCqItKmCPRMSqIFRgqixMnZWODJ7bdKm5pIldldeihJTXDqHqq2fix4mEx/vixwPRKAblSsYxdH2nnWgZzFO8hhsfjhUVkjmpytRUmWOuowk6xyAamXQrrzgBoTLHhBsSpk1Km/FEpgk6DHpWSuqpvOBRZ02xyetCR+M41nKqyG5617PirAarUfqmfSq6uBW/d21OMoBWCwjTJAGbtAVJoX4B3QSLyvxNnMdoIlhgwg0J8yalqqvnrltTSqEg0/ix5hYyD2W8Vdd8vuv1RZyQtCDnrCDasB0k7EhVRqhAywPOdpRtyl2kbJQedxxq";
            pageTop += "IKtAUqZHZkpqYE15ZJ6zYhLGjx0nWE5E6VkwkRMrXXYjrxYQpGUF7ElSlecTXD/dZBAl7zRsnIrmZ3NCrYSOp7zWhUFjOWd1BrMhVzxoUnbpuUm/lfWMFfRqVCCTZM4Fy8AtaJ7IeNtm1F1X8sQGhE5U7RT8Pfdyjku8ypyBRaWb5PlR56m8e9aE04UVXdpAfWUB+tE0J6Ha9zJtQ8Ic1lOez9o91yaqS4ssRp275IGMt3SyxHzCXkVyB56NRrF7eY/TUXaxxlVSlREq0MwAk1W8yAaYecn+yZoIjnXAFLtU4bwRKtntOJjib0WC5lKBVblNgqveRLZ+AIaTjZyxpIuLntVgwXCpwiPtmo9E7O";
            pageTop += "M1d+D66fYAk5X9sxbNQXCYVnXx5LAbpWciaPXUn+EpHlsAobLdgXNYnuE16bdyPGXCC8WI/f/yQXDtOfdY31kk0n5/nSEIotUGkcsVj6OFXJrbaz2a4z2dDiJYVOZ3JXR6jgLDNru4ossISi5SXKpLk1HapH58iqfhm9urVhxeKXBNSde/GSW76iZcG4E1ZeMAjSVZfflIx851NUFUVlogRaNtBKKUftGLAKEC9VdCBQbT9a6cr9sYTXuPJJWrzrqOWUbip4mkeaxSuptyX5ancO/9QX+3RtvViR0okqq87q3U+wdoLBnM0emWqURbtU+EgggQKlDzh5YV1Hm6QysDTfpyVT0cBYGVbnVzqYOD";
            pageTop += "xqmuC8ASpFkxptE/egXOo7boyqGf8rljbgM/9bGDFJsydwRBtP26i4Wccec0qcoIFWhmJbRTYBLvdySKfT1nBZc1OaxN+xoSMguMC3NaV2OrpBhrEr+P1lkFL1cVO+63spmxSrdz3D1I1pQ5YzVYpDW12OvaAgkQKgsnVjTpZ1WaPBrN2N+uktzepH854zrS9uE4MoNrWE9ZyXe2xoKzDJTNUjrZpghMEHxNCJUm7+mwDaK9eJ0NRpUJLnWkb+veWxDtpaYWCxnxS1FEPRWECjQqVtYyJnkhM+ZMBmA3wKwXmDBtBbObcQ29aU34UUqWSFdjU5zr4mL8eLakUGl7QPbv/eVZWqNyUpVNiB44oZ";
            pageTop += "Lzu+pK0Lhv3dhosY9OLJC6vpUAIFTmjdU0a4XjrIJZpzwALrmB7kiRgc+JgCxXROtuLHf8fsJLF5rasKyla1jPWgG7tk3Lclhrwx+fIPjqTjCFNiTMIescNuYsSHqpwbEjTbj1OyZUrrSwcWBWXSqsKggVaHC1qAG2lyNWzrmaCdNCk4Jfh2Q3L8PAvX4m5WW5EAYti5V+tD82RSKlyZVVo8X4AtfaoMrKsekJyROpxuUGYj/KbEiY1r+yTP3zZk1potqvBGradhzrJYTbUltiSpmCXv9u3MLjrjGtbx6bo+KZCBWYD7ESP1YyVs5CLpStNiPaXSpy0h4r/YLX0c8QK8faEitOxJ1sWaQ0KlQS";
            pageTop += "XGtZFqvtKD2e6XhTItbdm0E0aUlrsg17DQjoKOE+77TQp5Zb/MmfauA+pfUXbca40UCfbqKvr5UdQyowKPj9gFCBhgTLqpvodzMm+6ddXZNGJ3y3+hgkTPilBhknVt6Xcg0mVlYaOuc0YVVZpOT4/deaOu9o0rV2pcB29xI1lzNE7GadPuHuyVY0aUk7U+C8ihzXqDv59xP61XpD5xZS91yz+tGxqnFbnphMqrx8qYKwbMWiEroP6/ajjPEmy/1zmgJwCBVo54fXd6uZrOBB1TXZdoKl1oDqJnutwJ5IG/zKDjJu8OilTKz6jifqii1nVt4KhNWum1zrWAGyBjZVvuzXPO/lhIlms0Cb5rkIT3";
            pageTop += "p9YqnM+TiLjO7/0UCkNLEKPhW031qN30aYqlw35fxUBUtDUfL64GaZSdQKL0ajLQiSfqcPxG3RK2Ndcn3xWIW2KcJGgsBqi6zxqc9mhd3lmqtXr9IKc44byNaj/JLpj7jJb1BEVLgfbs8NRqdz3n6iTgVMNzGtR8klvS3or1/ivE+5SeB4QhusVZ243LF13AcLvH3XtfeWe+RNDktu4luJki1W9xQVg541Jq9PXPLOcSflfE4lTFS7rh37NfvusmvPpH2tHnD3fLvica2tVsoeo0Tft5T2zZKT/yn3uaLbHFxyvwFdx5bbw2fJE0q9jH4ztqJUaIdT7nd5LGdcWS/ZN+36w+Nedt83aMpVV+I3";
            pageTop += "e9n16UEECBVoVbCsZQxWSYNXFO333/bcJHWswDGuuEF6rYHzX3Lnvxal70FyJZj4twILRy9l8L/kBtNByXNa8VZ9y1H2FvJtUmmDthwBWJULri2rCAhrz6L9y59Edpxw6Zf4rqjEBLru+k/V+5x5ju74q1PsQ1ecWN0oeq9cuu5qhfsTjilrfrs7a+xKjfu+VuI++r/xqvtdnaEwYHdg9+QFwk3CA2/FcipHtByv+GO+5AbAQZM+ZbeC0mC+7lZy9vAn2aPuUUSMXXYibKOG6X8pmu7mfmlsVGzTDeeySVvBtjbppbBTsT2PpYjqrGuv0jfr3Osi59iWSLli1hb3GFS8T3X7+/GK9/2yE4hHUt";
            pageTop += "p0qcF7eKmAYFqOAKECrQoWS8freytLWykWXdXsetaKQbRnct6a0jVsuokx6fyj4Bps1WXnuuXOdbuB85DwOxGIwdZx1+wPzltN9AfPpbHi9YXIsyL4933L3ffGBKnuideeW0XM+86NYxPHdovN3vdExnZFd9FKxjn2XVsOKvaJXsLCpI022KrSBn6fTegvfttuFXXreNdcpv/d485jq4r7yF3HdgSdAdcPAAAAdBayfgAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAECoAAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAIBQAQAAAECoAAAAACBUAAAAAKECAAAAgFABAACAA8J1NAFU4ZnffVsv";
            pageTop += "/nORlgCAkly6/YNf69EMUBQsKgAAAIBQAQAAACgLrh+oynb8eIBmAIAKYwdAYa65evUqrQAAAACdBNcPAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAABCBQAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAECoAAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAIBQAQAAAECoAAAAACBUAAAAAKECAAAAgFABAAAAhAoAAAAAQgUAAAAAoQIAAAAIFQAAAACECgAAACBUAAAAABAqAAAAAAgVAAAAQKgAAAAAIFQAAAAAoQIAAACAUAEAAACECgAAAABCBQAAAAChAgAAAAgVAAAAAIQKAAAAIFQAAA";
            pageTop += "AAECoAAAAACBUAAABAqAAAAAAgVAAAAAChAgAAAIBQAQAAAECoAAAAAEIFAAAAAKECAAAACBUAAAAAhAoAAAAAQgUAAAAQKgAAAAAIFQAAAECoAAAAACBUAAAAABAqAAAAgFABAAAAQKgAAAAAQgUAAAAAoQIAAAAIFQAAAACECgAAAABCBQAAABAqAAAAAAgVAAAAQKgAAAAAIFQAAAAAECoAAACAUAEAAABAqAAAAABCBQAAAAChAgAAAIBQAQAAAIQKAAAAAEIFAAAAECoAAAAACBUAAAAAhAoAAAAgVAAAAAAQKgAAAIBQAQAAAECoAAAAACBUAAAAAKECAAAAgFABAAAAhAoAAAAAQgUA";
            pageTop += "AAAAoQIAAAAIFQAAAACECgAAACBUAAAAABAqAAAAgFABAAAAQKgAAAAAIFQAAAAAoQIAAACAUAEAAACECgAAAABCBQAAACCb/1+AAQAeXUHaMpCpHwAAAABJRU5ErkJggg==' alt='BC Gov Logo'>\n</td><td width='80%'>";
            pageTop += $"<h1 class='title'>Liquor and Cannabis Regulation Branch</h1>{heading}</td></tr></table>\n";

            var pageBottom = $"<div class='footer'><div class='footer-box'><p></p></div><div class='issued-box'><p style='text-align:right;'><small>Printed: {DateTime.Today.ToString("MMMM dd, yyyy")}</small></p></div></div></div></div>\n";

            var locationDetails = "";
            var locationNumber = 1;

            foreach (var location in specialEvent.AdoxioSpecialeventSpecialeventlocations)
            {
                locationDetails += pageTop;
                // draw the location
                locationDetails += $"<h2 class='info'>Event Location: {HttpUtility.HtmlEncode(location.AdoxioLocationname)}</h2>\n";
                locationDetails += "<table class='info'>";
                locationDetails += $"<tr><th class='heading'>Location Permit:</th><td class='field'>{HttpUtility.HtmlEncode(specialEvent.AdoxioSpecialeventpermitnumber)}-{locationNumber++}</td></tr>\n";
                locationDetails += $"<tr><th class='heading'>Location Name:</th><td class='field'>{HttpUtility.HtmlEncode(location.AdoxioLocationname)}</td></tr>\n";
                locationDetails += $"<tr><th class='heading'>Location Description:</th><td class='field'>{HttpUtility.HtmlEncode(location.AdoxioLocationdescription)}</td></tr>\n";
                locationDetails += $"<tr><th class='heading'>Event Address:</th><td class='field'>{HttpUtility.HtmlEncode(location.AdoxioEventlocationstreet2)} {HttpUtility.HtmlEncode(location.AdoxioEventlocationstreet1)}, {HttpUtility.HtmlEncode(specialEvent.AdoxioSpecialEventCityDistrictId.AdoxioName)} BC, {HttpUtility.HtmlEncode(location.AdoxioEventlocationpostalcode)}</td></tr>\n";
                locationDetails += $"<tr><th class='heading'>Total Attendees:</th><td class='field'>{location.AdoxioMaximumnumberofguestslocation}</td></tr>\n";

                // issued permits only display a minor detail of the service areas
                int serviceAttendees = 0;
                var serviceAreaDetails = "";
                var serviceAreaCount = 1;
                foreach (var sched in location.AdoxioSpecialeventlocationLicencedareas)
                {
                    if (sched.AdoxioLicencedareamaxnumberofguests != null)
                    {
                        serviceAttendees += (int)sched.AdoxioLicencedareamaxnumberofguests;
                    }                    
                    serviceAreaDetails += $"<tr><th class='heading'>Service Area #{serviceAreaCount++}:</th><td class='field'>{HttpUtility.HtmlEncode(sched.AdoxioEventname)} (capacity: {sched.AdoxioLicencedareamaxnumberofguests})</td></tr>";
                }

                locationDetails += serviceAreaDetails;
                locationDetails += $"<tr><th class='heading'>Total Attendees in Service Areas:</th><td class='field'>{serviceAttendees}</td></tr>\n";
                locationDetails += "</table>";

                // show all event dates
                locationDetails += "<h3 class='info'>Event Date(s):</h3>";

                // we will need to paginate every 4 days;
                //var eventNumber = 1;
                foreach (var sched in location.AdoxioSpecialeventlocationSchedule)
                {

                    var startDateParam = "";
                    if (sched.AdoxioEventstart.HasValue)
                    {
                        DateTime startDate = DateUtility.FormatDatePacific(sched.AdoxioEventstart).Value;
                        startDateParam = startDate.ToString("MMMM dd, yyyy");
                    }

                    var eventTimeParam = "";
                    if (sched.AdoxioEventstart.HasValue && sched.AdoxioEventend.HasValue)
                    {
                        DateTime startTime = DateUtility.FormatDatePacific(sched.AdoxioEventstart).Value;
                        DateTime endTime = DateUtility.FormatDatePacific(sched.AdoxioEventend).Value;
                        eventTimeParam = startTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US")) + " - " + endTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US"));
                    }

                    var serviceTimeParam = "";

                    if (sched.AdoxioServicestart.HasValue && sched.AdoxioServiceend.HasValue)
                    {
                        DateTime startTime = DateUtility.FormatDatePacific(sched.AdoxioServicestart).Value;
                        DateTime endTime = DateUtility.FormatDatePacific(sched.AdoxioServiceend).Value;
                        serviceTimeParam = startTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US")) + " - " + endTime.ToString("t", CultureInfo.CreateSpecificCulture("en-US"));
                    }

                    locationDetails += "<table class='info'>";
                    locationDetails += $"<tr><th class='heading'>Date:</th><td class='field'>{startDateParam}</td>\n";
                    locationDetails += $"<th class='heading'>Event Times:</th><td class='field'>{eventTimeParam}</td>\n";
                    locationDetails += $"<th class='heading'>Service Times:</th><td class='field'>{serviceTimeParam}</td></tr>\n";
                    locationDetails += "</table>\n";
                }

                // show the Ts and Cs if they're there.
                if (specialEvent.AdoxioSpecialeventSpecialeventtsacs?.Count > 0)
                {
                    locationDetails += "<h3 class='info'>Permit Terms and Conditions</h3><ul>";
                    foreach (var tc in specialEvent.AdoxioSpecialeventSpecialeventtsacs)
                    {
                        locationDetails += $"<li>{HttpUtility.HtmlEncode(tc.AdoxioTermsandcondition)}</li>";
                    }
                    locationDetails += "</ul>";
                }

                locationDetails += "<p>The terms and conditions to which this Special Event Permit is subject include the terms and conditions contained in the Special Event Permit Terms and Conditions Handbook, which is available on the Liquor and Cannabis Regulation Branch website.</p>";


                //locationDetails += "<p>&nbsp;</p><p>Signed: ________________________________</p>";
                locationDetails += "<p><em>The information on this form is collected by the Liquor and Cannabis Regulation Branch under Section 26(a) and (c) of the Freedom of Information and Protection of Privacy Act and will be used for the purpose of liquor licensing and compliance and";
                locationDetails += "enforcement matters in accordance with the Liquor Control and Licensing Act. Should you have any questions about the collection, uses, or disclosure of personal information, please contact the Freedom of Information Officer at PO Box 9292 STN";
                locationDetails += "PROV GVT, Victoria, BC, V8W 9J8 or by phone toll free at 1-866-209-2111.</em></p>";

                locationDetails += pageBottom;

            }

            var feesInfo = "";

            feesInfo += "<h2 class='info'>Quantities and Prices of Drinks</h2>\n";
            feesInfo += "<p>All liquor for your event must be bought from an approved outlet. For a list of approved ";
            feesInfo += "outlets please see the <a href='https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/guide-sep.pdf' target='_blank'>Special Event Permit Terms and Conditions</a>.</p>\n";
            feesInfo += "<table class='info'>\n";
            feesInfo += "<tr><th class='heading fat center'>Drink Type</th><th class='heading fat center'>Number of Servings</th><th class='heading fat center'>Price Per Serving</th></tr>\n";

            foreach (var forecast in specialEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent)
            {
                var itemName = "";
                if (forecast.AdoxioName.IndexOf("Beer") >= 0)
                {
                    itemName = "Beer/Cider/Cooler";
                }
                else
                {
                    if (forecast.AdoxioName.IndexOf("Wine") >= 0)
                    {
                        itemName = "Wine";
                    }
                    else
                    {
                        itemName = "Spirits";
                    }

                }
                feesInfo += $"<tr><td class='field center'>{HttpUtility.HtmlEncode(itemName)}</td>";
                feesInfo += $"<td class='field center'>{forecast.AdoxioEstimatedservings}</td>";
                feesInfo += $"<td class='field center'>{HttpUtility.HtmlEncode(String.Format("{0:$#,##0.00}", forecast.AdoxioPriceperserving))}</td></tr>\n";


            }

            feesInfo += "</table>\n";

            parameters.Add("title", title);
            parameters.Add("heading", heading);
            parameters.Add("appInfo", appInfo);
            parameters.Add("printDate", DateTime.Today.ToString("MMMM dd, yyyy"));
            parameters.Add("eligibilityInfo", eligibilityInfo);
            parameters.Add("locationDetails", locationDetails);
            parameters.Add("feesInfo", feesInfo);

            var templateName = "sep";

            byte[] data = await _pdfClient.GetPdf(parameters, templateName);

            try
            {
                await StoreCopyOfPdf(parameters, templateName, specialEvent.AdoxioSpecialeventid, data, "Permit");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error uploading copy of PDF");
            }
            

            return File(data, "application/pdf", $"Special Event Permit - {specialEvent.AdoxioSpecialeventpermitnumber}.pdf");


            //return new UnauthorizedResult();
        }

        private async Task StoreCopyOfPdf(Dictionary<string, string> parameters, string templateName, string id, byte[] data, string documentType)
        {
            try
            {
                var hash = await _pdfClient.GetPdfHash(parameters, templateName);
                var entityName = "adoxio_specialevent";
                var folderName = await _dynamicsClient.GetFolderName("specialevent", id).ConfigureAwait(true);
                _fileManagerClient.UploadPdfIfChanged(_logger, entityName, id, folderName, documentType, data, hash);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error uploading PDF");
            }
        }

        private MicrosoftDynamicsCRMadoxioSpecialevent GetSpecialEventData(string eventId)
        {
            string[] expand = new[] {
                "adoxio_Invoice",
                "adoxio_specialevent_licencedarea",
                "adoxio_specialevent_schedule",
                "adoxio_specialevent_specialeventlocations",
                "adoxio_SpecialEventCityDistrictId",
                "adoxio_ContactId",
                "adoxio_AccountId",
                "adoxio_specialevent_adoxio_sepdrinksalesforecast_SpecialEvent",
                "adoxio_specialevent_specialeventtsacs",
                "adoxio_PoliceAccountId"
            };
            MicrosoftDynamicsCRMadoxioSpecialevent specialEvent = null;
            if (!string.IsNullOrEmpty(eventId))
            {
                try
                {
                    specialEvent = _dynamicsClient.Specialevents.GetByKey(eventId, expand: expand);
                    var locations = specialEvent.AdoxioSpecialeventSpecialeventlocations;
                    var areas = specialEvent.AdoxioSpecialeventLicencedarea;
                    var schedules = specialEvent.AdoxioSpecialeventSchedule;

                    foreach (var schedule in schedules)
                    {
                        var parentLocation = locations.Where(loc => loc.AdoxioSpecialeventlocationid == schedule._adoxioSpecialeventlocationidValue).FirstOrDefault();
                        if (parentLocation != null)
                        {
                            if (parentLocation.AdoxioSpecialeventlocationSchedule == null)
                            {
                                parentLocation.AdoxioSpecialeventlocationSchedule = new List<MicrosoftDynamicsCRMadoxioSpecialeventschedule>();
                            }
                            parentLocation.AdoxioSpecialeventlocationSchedule.Add(schedule);
                        }
                        
                    }

                    foreach (var area in areas)
                    {
                        var parentLocation = locations.Where(loc => loc.AdoxioSpecialeventlocationid == area._adoxioSpecialeventlocationidValue).FirstOrDefault();
                        if (parentLocation != null)
                        {
                            if (parentLocation.AdoxioSpecialeventlocationLicencedareas == null)
                            {
                                parentLocation.AdoxioSpecialeventlocationLicencedareas = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                            }
                            parentLocation.AdoxioSpecialeventlocationLicencedareas.Add(area);
                        }                        
                    }
                }
                catch (HttpOperationException e)
                {
                    _logger.LogError(e, "Error getting special event");
                    specialEvent = null;
                }
            }
            return specialEvent;
        }

        [HttpPost]
        public IActionResult CreateSpecialEvent([FromBody] ViewModels.SpecialEvent specialEvent)
        {
            if (specialEvent == null)
            {
                return BadRequest();
            }


            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);


            var newSpecialEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            newSpecialEvent.CopyValues(specialEvent);
            // do not set the invoice trigger on create
            newSpecialEvent.AdoxioInvoicetrigger = null;
            newSpecialEvent.Statuscode = (int?)EventStatus.Draft;

            if (!string.IsNullOrEmpty(userSettings.AccountId) && userSettings.AccountId != "00000000-0000-0000-0000-000000000000")
            {
                newSpecialEvent.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);
            }
            if (!string.IsNullOrEmpty(userSettings.ContactId) && userSettings.ContactId != "00000000-0000-0000-0000-000000000000")
            {
                newSpecialEvent.ContactODataBind = _dynamicsClient.GetEntityURI("contacts", userSettings.ContactId);
            }

            if (!string.IsNullOrEmpty(specialEvent?.SepCity?.Id))
            {
                newSpecialEvent.SepCityODataBind = _dynamicsClient.GetEntityURI("adoxio_sepcities", specialEvent.SepCity.Id);
            }
            try
            {
                newSpecialEvent = _dynamicsClient.Specialevents.Create(newSpecialEvent);
                specialEvent.Id = newSpecialEvent.AdoxioSpecialeventid;
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating special event");
                throw httpOperationException;
            }

            SaveTotalServings(specialEvent, newSpecialEvent);

            if (specialEvent.EventLocations?.Count > 0)
            {
                // newSpecialEvent.AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>();
                // add locations to the new special event
                specialEvent.EventLocations.ForEach((Action<ViewModels.SepEventLocation>)(location =>
                {
                    var newLocation = new MicrosoftDynamicsCRMadoxioSpecialeventlocation();
                    newLocation.CopyValues(location);
                    newLocation.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                    // newSpecialEvent.AdoxioSpecialeventSpecialeventlocations.Add(newLocation);
                    try
                    {
                        newLocation = _dynamicsClient.Specialeventlocations.Create(newLocation);
                        location.Id = newLocation.AdoxioSpecialeventlocationid;
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error creating special event location");
                        throw httpOperationException;
                    }

                    // Add service areas to new location
                    if (location.ServiceAreas?.Count > 0)
                    {
                        newLocation.AdoxioSpecialeventlocationLicencedareas = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                        location.ServiceAreas.ForEach((Action<ViewModels.SepServiceArea>)(area =>
                        {
                            var newArea = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea();
                            newArea.CopyValues(area);
                            newArea.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                            newArea.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", newLocation.AdoxioSpecialeventlocationid);
                            try
                            {
                                newArea = _dynamicsClient.Specialeventlicencedareas.Create(newArea);
                                area.Id = newArea.AdoxioSpecialeventlicencedareaid;
                            }
                            catch (HttpOperationException httpOperationException)
                            {
                                _logger.LogError(httpOperationException, "Error creating special event location");
                                throw httpOperationException;
                            }
                            newLocation.AdoxioSpecialeventlocationLicencedareas.Add(newArea);
                        }));
                    }

                    // Add event dates to location
                    if (location.EventDates?.Count > 0)
                    {
                        location.EventDates.ForEach(dates =>
                        {
                            var newDates = new MicrosoftDynamicsCRMadoxioSpecialeventschedule();
                            newDates.CopyValues(dates);
                            newDates.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", newSpecialEvent.AdoxioSpecialeventid);
                            newDates.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", newLocation.AdoxioSpecialeventlocationid);
                            try
                            {
                                newDates = _dynamicsClient.Specialeventschedules.Create(newDates);
                                dates.Id = newDates.AdoxioSpecialeventscheduleid;
                            }
                            catch (HttpOperationException httpOperationException)
                            {
                                _logger.LogError(httpOperationException, "Error creating special event location");
                                throw httpOperationException;
                            }
                        });
                    }
                }));
            }

            // Set the invoice trigger after creating child entities
            try
            {
                var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
                {
                    AdoxioInvoicetrigger = true
                };
                _dynamicsClient.Specialevents.Update(newSpecialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error setting invoice trigger after creating special event with child entities");
                throw httpOperationException;
            }

            var result = this.GetSpecialEventData(newSpecialEvent.AdoxioSpecialeventid).ToViewModel(_dynamicsClient);
            result.LocalId = specialEvent.LocalId;
            return new JsonResult(specialEvent);
        }

        [HttpPut("terms-and-conditions/{eventId}")]
        public IActionResult UpdateSpecialEventTermsAndConditions(string eventId, [FromBody] List<ViewModels.SepTermAndCondition> termsAndCondtions)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(eventId))
            {
                return BadRequest();
            }

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            // get the special event.
            string[] expand = new[] { "adoxio_specialevent_specialeventtsacs" };
            var specialEvent = _dynamicsClient.Specialevents.GetByKey(eventId, expand: expand);
            if (userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }

            var toDelete = specialEvent.AdoxioSpecialeventSpecialeventtsacs.Select(tnc => tnc.AdoxioSpecialeventtandcid)
                        .Except(termsAndCondtions.Select(tnc => tnc.Id).Distinct())
                        .ToList();

            toDelete.ForEach(id =>
            {
                _dynamicsClient.Specialeventtandcs.Delete(id);
            });

            termsAndCondtions.ForEach(tnc =>
            {
                var patchTnC = new MicrosoftDynamicsCRMadoxioSpecialeventtandc
                {
                    AdoxioOriginator = tnc.Originator,
                    AdoxioTermsandcondition = tnc.Content
                };
                if (string.IsNullOrEmpty(tnc.Id)) //create tnc
                {
                    patchTnC.SpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", eventId);
                    var res = _dynamicsClient.Specialeventtandcs.Create(patchTnC);
                }
                else // update tnc
                {
                    _dynamicsClient.Specialeventtandcs.Update(tnc.Id, patchTnC);
                }
            });

            // get list of terms and conditions
            var newTermsList = _dynamicsClient.Specialevents.GetByKey(eventId, expand: expand)
            .AdoxioSpecialeventSpecialeventtsacs.Select(tnc => new SepTermAndCondition
            {
                Id = tnc.AdoxioSpecialeventtandcid,
                Content = tnc.AdoxioTermsandcondition,
                Originator = tnc.AdoxioOriginator
            }).ToList();

            return new JsonResult(newTermsList); ;
        }

        [HttpPost("generate-invoice/{eventId}")]
        public IActionResult GenerateInvoice(string eventId)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(eventId))
            {
                return BadRequest();
            }

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var existingEvent = GetSpecialEventData(eventId);
            if (existingEvent._adoxioAccountidValue != userSettings.AccountId &&
               existingEvent._adoxioContactidValue != userSettings.ContactId)
            {
                return Unauthorized();
            }
            
            // just enable the invoice trigger.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                AdoxioInvoicetrigger = true
            };
            
            try
            {
                _dynamicsClient.Specialevents.Update(eventId, patchEvent);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating/updating special event");
                throw httpOperationException;
            }
            
            return Ok();
        }

        /// <summary>
        /// Handle a submit event (last step on the summary page)
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpPost("submit/{eventId}")]
        public IActionResult Submit(string eventId)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(eventId))
            {
                return BadRequest();
            }

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var existingEvent = GetSpecialEventData(eventId);
            if (existingEvent._adoxioAccountidValue != userSettings.AccountId &&
                existingEvent._adoxioContactidValue != userSettings.ContactId)
            {
                return Unauthorized();
            }

            // just set the status to submitted.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                Statuscode = (int?)EventStatus.Submitted
            };
            
            try
            {
                _dynamicsClient.Specialevents.Update(eventId, patchEvent);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating/updating special event");
                throw httpOperationException;
            }
            
            return Ok();
        }

        [HttpPut("{eventId}")]
        public IActionResult UpdateSpecialEvent(string eventId, [FromBody] ViewModels.SpecialEvent specialEvent)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(eventId) || eventId != specialEvent?.Id)
            {
                return BadRequest();
            }

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            var existingEvent = GetSpecialEventData(eventId);
            if (existingEvent._adoxioAccountidValue != userSettings.AccountId &&
               existingEvent._adoxioContactidValue != userSettings.ContactId)
            {
                return Unauthorized();
            }

            var itemsToDelete = GetItemsToDelete(specialEvent, existingEvent);
            DeleteSpecialEventItems(itemsToDelete);

            // update drink forecast information
            SaveTotalServings(specialEvent, existingEvent);

            // ensure that the special event update does not write to the drink forecasts.
            if (specialEvent.DrinksSalesForecasts != null)
            {
                specialEvent.DrinksSalesForecasts.Clear();
                specialEvent.DrinksSalesForecasts = null;
            }

            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();
            patchEvent.CopyValues(specialEvent);

            // Only allow these status to be set by the portal. Any other status change is ignored
            if (specialEvent.EventStatus == EventStatus.Cancelled ||
                specialEvent.EventStatus == EventStatus.Draft ||
                specialEvent.EventStatus == EventStatus.Submitted
               )
            {
                patchEvent.Statuscode = (int?)specialEvent.EventStatus;
            }

            if (!string.IsNullOrEmpty(specialEvent?.SepCity?.Id))
            {
                patchEvent.SepCityODataBind = _dynamicsClient.GetEntityURI("adoxio_sepcities", specialEvent.SepCity.Id);
            }

            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.Id, patchEvent);
                specialEvent.Id = patchEvent.AdoxioSpecialeventid;
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error creating/updating special event");
                throw httpOperationException;
            }

 



            if (specialEvent.EventLocations?.Count > 0)
            {
                // patchEvent.AdoxioSpecialeventSpecialeventlocations = new List<MicrosoftDynamicsCRMadoxioSpecialeventlocation>();
                // add locations to the new special event
                specialEvent.EventLocations.ForEach((Action<ViewModels.SepEventLocation>)(location =>
                {
                    var newLocation = new MicrosoftDynamicsCRMadoxioSpecialeventlocation();
                    newLocation.CopyValues(location);
                    newLocation.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                    try
                    {
                        if (string.IsNullOrEmpty(location.Id))
                        { // create record
                            newLocation = _dynamicsClient.Specialeventlocations.Create(newLocation);
                            location.Id = newLocation.AdoxioSpecialeventlocationid;
                        }
                        else
                        { // update record
                            _dynamicsClient.Specialeventlocations.Update(location.Id, newLocation);
                        }

                        // Add service areas to new location
                        if (location.ServiceAreas?.Count > 0)
                        {
                            newLocation.AdoxioSpecialeventlocationLicencedareas = new List<MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea>();
                            location.ServiceAreas.ForEach((Action<ViewModels.SepServiceArea>)(area =>
                            {
                                var newArea = new MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea();
                                newArea.CopyValues(area);
                                newArea.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                                newArea.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", location.Id);
                                try
                                {
                                    if (string.IsNullOrEmpty((string)area.Id))
                                    { // create record
                                        newArea = _dynamicsClient.Specialeventlicencedareas.Create(newArea);
                                        area.Id = newArea.AdoxioSpecialeventlicencedareaid;
                                    }
                                    else
                                    { // update record
                                        _dynamicsClient.Specialeventlicencedareas.Update((string)area.Id, newArea);
                                        newArea.AdoxioSpecialeventlicencedareaid = area.Id;
                                    }
                                }
                                catch (HttpOperationException httpOperationException)
                                {
                                    _logger.LogError(httpOperationException, "Error creating/updating special event location");
                                    throw httpOperationException;
                                }
                            }));
                        }

                        // Add event dates to the location
                        if (location.EventDates?.Count > 0)
                        {
                            location.EventDates.ForEach(dates =>
                            {
                                var newDates = new MicrosoftDynamicsCRMadoxioSpecialeventschedule();
                                newDates.CopyValues(dates);
                                try
                                {
                                    if (string.IsNullOrEmpty(dates.Id))
                                    { // create record
                                        newDates.AdoxioSpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                                        newDates.AdoxioSpecialEventLocationODataBind = _dynamicsClient.GetEntityURI("adoxio_specialeventlocations", location.Id);
                                        newDates = _dynamicsClient.Specialeventschedules.Create(newDates);
                                        dates.Id = newDates.AdoxioSpecialeventscheduleid;
                                    }
                                    else
                                    { // update record
                                        _dynamicsClient.Specialeventschedules.Update(dates.Id, newDates);
                                    }
                                }
                                catch (HttpOperationException httpOperationException)
                                {
                                    _logger.LogError(httpOperationException, "Error creating/updating special event schedule");
                                    throw httpOperationException;
                                }
                            });
                        }


                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error creating/updating special event location");
                        throw httpOperationException;
                    }
                }));
            }
            var result = this.GetSpecialEventData(eventId).ToViewModel(_dynamicsClient);
            result.LocalId = specialEvent.LocalId;
            return new JsonResult(result);
        }


        private ItemsToDelete GetItemsToDelete(ViewModels.SpecialEvent updateEvent, MicrosoftDynamicsCRMadoxioSpecialevent existingEvent)
        {
            var toDelete = new ItemsToDelete();

            // create a list of id that exist in dynamics
            List<string> existingLocations = existingEvent.AdoxioSpecialeventSpecialeventlocations
                .Select(loc => loc.AdoxioSpecialeventlocationid)
                .ToList();
            List<string> existingEventDates = existingEvent.AdoxioSpecialeventSchedule
                .Select(eventDate => eventDate.AdoxioSpecialeventscheduleid)
                .ToList();
            List<string> existingServiceAreas = existingEvent.AdoxioSpecialeventLicencedarea
                .Select(area => area.AdoxioSpecialeventlicencedareaid)
                .ToList();

            // make the list of id present in the special event being sent to dynamis
            List<string> updateLocations = new List<string>();
            List<string> updateEventDates = new List<string>();
            List<string> updateServiceAreas = new List<string>();

            updateEvent?.EventLocations?.ForEach(loc =>
            {
                if (!string.IsNullOrEmpty(loc.Id))
                {
                    updateLocations.Add(loc.Id);
                }

                loc?.EventDates?.ForEach(eventDate =>
                {
                    if (!string.IsNullOrEmpty(eventDate.Id))
                    {
                        updateEventDates.Add(eventDate.Id);
                    }
                });

                loc?.ServiceAreas?.ForEach(area =>
                {
                    if (!string.IsNullOrEmpty(area.Id))
                    {
                        updateServiceAreas.Add(area.Id);
                    }
                });
            });


            // Subtract the sets of list to get ids due for deletion
            toDelete.Locations = existingLocations.Except(updateLocations).ToList();
            toDelete.ServiceAreas = existingServiceAreas.Except(updateServiceAreas).ToList();
            toDelete.EventDates = existingEventDates.Except(updateEventDates).ToList();

            return toDelete;
        }

        private void DeleteSpecialEventItems(ViewModels.ItemsToDelete itemsToDelete)
        {

            if (itemsToDelete.EventDates.Count > 0)
            {
                itemsToDelete.EventDates.ForEach(id =>
                {
                    _dynamicsClient.Specialeventschedules.Delete(id);
                });
            }
            if (itemsToDelete.ServiceAreas.Count > 0)
            {
                itemsToDelete.ServiceAreas.ForEach(id =>
                {
                    _dynamicsClient.Specialeventlicencedareas.Delete(id);
                });
            }
            if (itemsToDelete.Locations.Count > 0)
            {
                itemsToDelete.Locations.ForEach(id =>
                {
                    _dynamicsClient.Specialeventlocations.Delete(id);
                });
            }
        }
        private void SaveTotalServings(ViewModels.SpecialEvent specialEvent, MicrosoftDynamicsCRMadoxioSpecialevent existingEvent)
        {
            // get drink types
            var filter = "adoxio_name eq 'Beer/Cider/Cooler' or ";
            filter += "adoxio_name eq 'Wine' or ";
            filter += "adoxio_name eq 'Spirits'";
            var drinkTypes = _dynamicsClient.Sepdrinktypes.Get().Value
                            .ToList();

            specialEvent.Beer = specialEvent.Beer ?? 0;
            specialEvent.Wine = specialEvent.Wine ?? 0;
            specialEvent.Spirits = specialEvent.Spirits ?? 0;
            
            // calculate serving amounts from percentages
            int totalServings = specialEvent.TotalServings == null ? 0 : (int)specialEvent.TotalServings;
            var typeData = new List<(string, int, bool, decimal?)>{
                ("Beer/Cider/Cooler", (int)specialEvent.Beer,true, specialEvent.AverageBeerPrice),
                ("Wine", (int)specialEvent.Wine,true, specialEvent.AverageWinePrice),
                ("Spirits", (int)specialEvent.Spirits,true, specialEvent.AverageSpiritsPrice)
            };
            // if ChargingForLiquorReason is Combination, Adding 3 records with 0 price
            if (specialEvent.ChargingForLiquorReason == ChargingForLiquorReasons.Combination)
            {
                typeData.AddRange(new List<(string, int, bool, decimal?)>{
                ("Beer/Cider/Cooler", (int)specialEvent.Beer_free, false, 0),
                ("Wine", (int)specialEvent.Wine_free, false, 0),
                ("Spirits", (int)specialEvent.Spirits_free, false, 0)
                }
            );
            }

            foreach (var type in typeData)
            {
                string drinkTypeName = type.Item1;
                int estimatedServings = type.Item2;
                bool ischarging = type.Item3;
                var drinkType = drinkTypes.Where(drinkType => drinkType.AdoxioName == drinkTypeName).FirstOrDefault();
                MicrosoftDynamicsCRMadoxioSepdrinksalesforecast existingForecast = null;
                if (existingEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent != null)
                {
                    existingForecast = existingEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
                        .Where(drink => drink._adoxioTypeValue == drinkType.AdoxioSepdrinktypeid
                        && drink.AdoxioIscharging == ischarging).FirstOrDefault();
                }
                CreateOrUpdateForecast(specialEvent, existingForecast, drinkType, estimatedServings, type.Item4, ischarging);

            }
            // Create or Update Drink Sale Forecast with the serving amounts
            //typeData.ForEach(data =>
            //{
            //    string drinkTypeName = data.Item1;
            //    int estimatedServings = data.Item2;
            //    var drinkType = drinkTypes.Where(drinkType => drinkType.AdoxioName == drinkTypeName).FirstOrDefault();
            //    MicrosoftDynamicsCRMadoxioSepdrinksalesforecast existingForecast = null;
            //    if (existingEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent != null)
            //    {
            //        existingForecast = existingEvent.AdoxioSpecialeventAdoxioSepdrinksalesforecastSpecialEvent
            //            .Where(drink => drink._adoxioTypeValue == drinkType.AdoxioSepdrinktypeid)
            //            .FirstOrDefault();
            //    }
            //    CreateOrUpdateForecast(specialEvent, existingForecast, drinkType, estimatedServings, data.Item4);
            //});
        }

        private void CreateOrUpdateForecast(
            ViewModels.SpecialEvent specialEvent, 
            MicrosoftDynamicsCRMadoxioSepdrinksalesforecast existingDrinkForecast, 
            MicrosoftDynamicsCRMadoxioSepdrinktype beerType, 
            int estimatedServings,
            decimal? averagePrice,bool ischarging)
        {
            if (averagePrice == null)
            {
                // do not allow a null update
                _logger.LogError($"Invalid SEP drink forecast encountered - Average Price is null.  SEP ID - {specialEvent.Id}");
            }
            else
            {
                try

                {
                    var newForecast = new MicrosoftDynamicsCRMadoxioSepdrinksalesforecast()
                    {
                        AdoxioIscharging = ischarging,
                        AdoxioPriceperserving = averagePrice,
                        AdoxioEstimatedservings = estimatedServings,
                    };


                    if (existingDrinkForecast == null)
                    { // create record
                        newForecast.SpecialEventODataBind = _dynamicsClient.GetEntityURI("adoxio_specialevents", specialEvent.Id);
                        if (!string.IsNullOrEmpty(beerType?.AdoxioSepdrinktypeid))
                        {
                            newForecast.DrinkTypeODataBind = _dynamicsClient.GetEntityURI("adoxio_sepdrinktypes", beerType.AdoxioSepdrinktypeid);
                        }
                        _dynamicsClient.Sepdrinksalesforecasts.Create(newForecast);
                    }
                    else
                    { // update record
                        _dynamicsClient.Sepdrinksalesforecasts.Update((string)existingDrinkForecast.AdoxioSepdrinksalesforecastid, newForecast);
                    }
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError(httpOperationException, "Error creating/updating sep drinks sales forecast");
                    throw httpOperationException;
                }  
            }
            
        }

        [HttpGet("drink-types")]
        public IActionResult GetDrinkTypes()
        {
            List<ViewModels.SepDrinkType> result = new List<ViewModels.SepDrinkType>();
            var drinkTypes = _dynamicsClient.Sepdrinktypes.Get().Value;
            foreach (var item in drinkTypes)
            {
                result.Add(item.ToViewModel());
            }
            return new JsonResult(result);
        }

        private List<ViewModels.SpecialEventSummary> GetSepSummaries(string filter)
        {
            List<ViewModels.SpecialEventSummary> result = new List<ViewModels.SpecialEventSummary>();

            string[] expand = new[] { "adoxio_PoliceRepresentativeId", "adoxio_PoliceAccountId", "adoxio_specialevent_specialeventtsacs" };
            IList<MicrosoftDynamicsCRMadoxioSpecialevent> items = null;
            try
            {
                items = _dynamicsClient.Specialevents.Get(filter: filter, expand: expand).Value;

                foreach (var item in items)
                {
                    result.Add(item.ToSummaryViewModel());
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting special events");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unexpected Error getting special events");
            }

            return result;
        }
        
        private PagingResult<ViewModels.SpecialEventSummary> GetPagedSepSummaries(string filter, int pageIndex, int pageSize, string sort, string sortdir)
        {
            PagingResult<ViewModels.SpecialEventSummary> result = new PagingResult<ViewModels.SpecialEventSummary>()
            {
                Value = new List<SpecialEventSummary>()
            };

            var orderby = new List<string> {};
            if(sort != null && sort.Length > 0 && sortdir != null && sortdir.Length > 0)
            {
                string tmp = transformColumnNametoSchemaName(sort);
                tmp = tmp + " " + sortdir;
                orderby.Add(tmp);
                
            }
            string[] expand = new[] { "adoxio_PoliceRepresentativeId", "adoxio_PoliceAccountId", "adoxio_specialevent_specialeventtsacs" };
            try
            {
                var customHeaders = new Dictionary<string, List<string>>();
                var preferHeader = new List<string>();
                var odataVersionHeader = new List<string>();

                preferHeader.Add($"odata.maxpagesize={pageSize}");
                customHeaders.Add("Prefer", preferHeader);
                odataVersionHeader.Add("4.0");
                customHeaders.Add("OData-Version", odataVersionHeader);
                customHeaders.Add("OData-MaxVersion", odataVersionHeader);
                //HttpOperationResponse<MicrosoftDynamicsCRMadoxioSpecialeventCollection>();
                var sepSummaryQuery = new HttpOperationResponse<MicrosoftDynamicsCRMadoxioSpecialeventCollection>();
                if(orderby.Count > 0)
                {
                    sepSummaryQuery = _dynamicsClient.Specialevents.GetWithHttpMessagesAsync(filter: filter, expand: expand, orderby: orderby, customHeaders: customHeaders, count: true).GetAwaiter().GetResult();
                }
                else
                {
                    sepSummaryQuery = _dynamicsClient.Specialevents.GetWithHttpMessagesAsync(filter: filter, expand: expand, customHeaders: customHeaders, count: true).GetAwaiter().GetResult();
                }
               
                
                while(pageIndex > 0)
                {
                    string odataNextLink = sepSummaryQuery.Body.OdataNextLink;
                    sepSummaryQuery = _dynamicsClient.Specialevents.GetNextLink(odataNextLink, customHeaders);
                    pageIndex--;
                }

                var sepSummaries = sepSummaryQuery.Body.Value;
                result.Count = Int32.Parse(sepSummaryQuery.Body.Count);

                foreach(var sepSummary in sepSummaries)
                {
                    var viewModel = sepSummary.ToSummaryViewModel();//.GetAwaiter().GetResult();
                    result.Value.Add(viewModel);
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error getting special events");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unexpected Error getting special events");
            }

            return result;
        }

        private String transformColumnNametoSchemaName(String colName)
        {
            if(colName != null && colName.Length > 0)
            {
                if(colName == "eventStartDate")
                {
                    return "adoxio_eventstartdate";
                }else if(colName == "dateSubmitted")
                {
                    return "adoxio_datesubmitted";
                }
                else if (colName == "eventName")
                {
                    return "adoxio_eventname";
                }
                else if (colName == "eventStatus")
                {
                    return "statuscode";
                }
                else if (colName == "policeDecisionBy")
                {
                    return "_adoxio_policerepresentativeid_value";
                }
                else if (colName == "maximumNumberOfGuests")
                {
                    return "adoxio_maxnumofguests";
                }
                else if (colName == "typeOfEventLabel")
                {
                    return "adoxio_typeofevent";
                }
                else
                {
                    return "";
                }

            }

            return null;
        }

        // police get summary list of applications waiting approval
        [HttpGet("police/all")]
        public IActionResult GetPoliceCurrent()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            var result = new SpecialEventPoliceJobSummary()
            {
                // Application Status == Pending Review && Police Decision == Under Review
                InProgress = GetSepSummaries($"_adoxio_policejurisdictionid_value eq {userAccount._adoxioPolicejurisdictionidValue} and adoxio_policeapproval eq {(int?)ApproverStatus.PendingReview}"),

                // Police Decision == Reviewed 
                PoliceApproved = GetSepSummaries($"_adoxio_policejurisdictionid_value eq {userAccount._adoxioPolicejurisdictionidValue} and statuscode ne {(int?)EventStatus.Draft} and (adoxio_policeapproval eq { (int?)ApproverStatus.AutoReviewed } or adoxio_policeapproval eq { (int?)ApproverStatus.Approved } or adoxio_policeapproval eq {(int?)ApproverStatus.Reviewed})"),

                // Police Decision == Denied || Cancelled
                PoliceDenied = GetSepSummaries($"_adoxio_policejurisdictionid_value eq {userAccount._adoxioPolicejurisdictionidValue} and (adoxio_policeapproval eq {(int?)ApproverStatus.Denied} or adoxio_policeapproval eq {(int?)ApproverStatus.Cancelled})")
            };

            return new JsonResult(result);

        }

        [HttpGet("police/pending-review")]
        public IActionResult GetPolicePendingReview([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10, [FromQuery] string sort = "", [FromQuery] string sortdir = "" )
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            // Application Status == Pending Review && Police Decision == Under Review
            var result = GetPagedSepSummaries($"_adoxio_policejurisdictionid_value eq {userAccount._adoxioPolicejurisdictionidValue} and adoxio_policeapproval eq {(int?)ApproverStatus.PendingReview}", pageIndex, pageSize, sort, sortdir);

            return new JsonResult(result);

        }

        [HttpGet("police/approved")]
        public IActionResult GetPoliceApproved([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10, [FromQuery] string sort = "", [FromQuery] string sortdir = "")
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            // Police Decision == Reviewed
            var result = GetPagedSepSummaries($"_adoxio_policejurisdictionid_value eq {userAccount._adoxioPolicejurisdictionidValue} and statuscode ne {(int?)EventStatus.Draft} and (adoxio_policeapproval eq {(int?)ApproverStatus.AutoReviewed} or adoxio_policeapproval eq {(int?)ApproverStatus.Approved} or adoxio_policeapproval eq {(int?)ApproverStatus.Reviewed})", pageIndex, pageSize, sort, sortdir);

            return new JsonResult(result);

        }

        [HttpGet("police/denied")]
        public IActionResult GetPoliceDenied([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10, [FromQuery] string sort = "", [FromQuery] string sortdir = "")
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            // Police Decision == Denied || Cancelled 
            var result = GetPagedSepSummaries($"_adoxio_policejurisdictionid_value eq {userAccount._adoxioPolicejurisdictionidValue} and (adoxio_policeapproval eq {(int?)ApproverStatus.Denied} or adoxio_policeapproval eq {(int?)ApproverStatus.Cancelled})", pageIndex, pageSize, sort, sortdir);

            return new JsonResult(result);

        }

        // police get summary list of applications for the current user
        [HttpGet("police/my")]
        public IActionResult GetPoliceMy()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            var result = new SpecialEventPoliceJobSummary()
            {
                // Application Status == Pending Review && Police Decision == Under Review
                InProgress = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and adoxio_policeapproval eq {(int?)ApproverStatus.PendingReview}"),

                // Police Decision == Reviewed
                PoliceApproved = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and statuscode ne {(int?)EventStatus.Draft} and (adoxio_policeapproval eq { (int?)ApproverStatus.AutoReviewed } or adoxio_policeapproval eq { (int?)ApproverStatus.Approved } or adoxio_policeapproval eq {(int?)ApproverStatus.Reviewed})"),

                // Police Decision == Denied || Cancelled                
                PoliceDenied = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and (adoxio_policeapproval eq {(int?)ApproverStatus.Denied} or adoxio_policeapproval eq {(int?)ApproverStatus.Cancelled})")
            };

            return new JsonResult(result);
        }

        [HttpPost("police/{id}/assign")]
        public IActionResult PoliceAssign([FromBody] string assignee, string id)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (specialEvent._adoxioPolicejurisdictionidValue != null && userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }

            // get the assignee.


            var contact = _dynamicsClient.GetContactById(assignee).GetAwaiter().GetResult();
            if (contact == null || contact.ParentcustomeridAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)
            {
                return Unauthorized();
            }

            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                PoliceRepresentativeIdODataBind = _dynamicsClient.GetEntityURI("contacts", assignee)
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return new JsonResult(contact.ToViewModel());
        }

        [HttpPost("police/{id}/approve")]
        public IActionResult PoliceApprove(string id)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (specialEvent._adoxioPolicejurisdictionidValue != null  && userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }


            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                AdoxioPoliceapproval = 845280000 // Approved
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return Ok();
        }

        [HttpPost("police/{id}/deny")]
        public IActionResult PoliceDeny(string id, SepPoliceReviewReason reasonText)
        {

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (specialEvent._adoxioPolicejurisdictionidValue != null && userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }


            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                AdoxioPoliceapproval = 845280001, // Denied 
                AdoxioDenialreason = reasonText.Reason,
                AdoxioDatepoliceapproved = DateTime.Now
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return Ok();
        }

        [HttpPost("police/{id}/cancel")]
        public IActionResult PoliceCancel(string id, SepPoliceReviewReason reasonText)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (specialEvent._adoxioPolicejurisdictionidValue != null && userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }

            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                AdoxioPoliceapproval = 845280002, // Cancelled
                AdoxioCancellationreason = reasonText.Reason,
                AdoxioDatepoliceapproved = DateTime.Now
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return Ok();
        }

        [HttpPost("police/{id}/setMunicipality/{cityId}")]
        public IActionResult PoliceSetMunicipality(string id, string cityId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }
            // get the special event.

            var specialEvent = _dynamicsClient.Specialevents.GetByKey(id);
            if (specialEvent._adoxioPolicejurisdictionidValue != null && userAccount._adoxioPolicejurisdictionidValue != specialEvent._adoxioPolicejurisdictionidValue)  // ensure the current account has a matching police jurisdiction.
            {
                return Unauthorized();
            }

            // update the given special event.
            var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent()
            {
                SepCityODataBind = _dynamicsClient.GetEntityURI("adoxio_sepcities", cityId)
            };
            try
            {
                _dynamicsClient.Specialevents.Update(specialEvent.AdoxioSpecialeventid, patchEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Error updating special event");
                return StatusCode(500);
            }


            return Ok();
        }

        [HttpGet("police/home")]
        public IActionResult GetPoliceHome()
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            // get the account details.
            var userAccount = _dynamicsClient.GetAccountById(userSettings.AccountId);
            if (string.IsNullOrEmpty(userAccount._adoxioPolicejurisdictionidValue))  // ensure the current account has a police jurisdiction.
            {
                return Unauthorized();
            }

            var result = new SpecialEventPoliceHome()
            {
                AssignedJobsInProgress = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and adoxio_policeapproval eq 100000001").Count, // Under review
                // TODO - revise the filter for this query.
                AssignedJobsInProgressWithExceptions = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and adoxio_policeapproval eq 100000001").Count,  // Approved
                AssignedApplicationsIssued = GetSepSummaries($"_adoxio_policerepresentativeid_value eq {userSettings.ContactId} and statuscode eq 845280003").Count // status is issued
            };

            return new JsonResult(result);
        }

        /// <summary>
        /// Gets SepCity Autocomplete data for a given name using startswith
        /// </summary>
        /// <param name="defaults">If set to true, the name parameter is ignored and a list of `preview` cities is returned instead</param>
        /// <param name="name">The name to filter by using startswith</param>
        /// <returns>Dictionary of key value pairs with accountid and name as the pairs</returns>
        [HttpGet("sep-city/autocomplete")]
        // [Authorize(Policy = "Business-User")]
        public IActionResult GetAutocomplete(string name, bool defaults)
        {
            var results = new List<ViewModels.SepCity>();
            try
            {
                string filter = null;
                // escape any apostophes.
                if (name != null)
                {
                    name = name.Replace("'", "''");
                    // select active accounts that match the given name
                    if (defaults)
                    {
                        filter = $"statecode eq 0 and adoxio_ispreview eq true";
                    }
                    else
                    {
                        filter = $"statecode eq 0 and contains(adoxio_name,'{name}')";
                    }
                }
                var expand = new List<string> { "adoxio_PoliceJurisdictionId", "adoxio_LGINId" };
                var cities = _dynamicsClient.Sepcities.Get(filter: filter, expand: expand, top: 20).Value;
                foreach (var city in cities)
                {
                    var newCity = new ViewModels.SepCity
                    {
                        Id = city.AdoxioSepcityid,
                        Name = city.AdoxioName,
                        PoliceJurisdictionName = city?.AdoxioPoliceJurisdictionId?.AdoxioName,
                        LGINName = city?.AdoxioLGINId?.AdoxioName
                    };
                    results.Add(newCity);
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error while getting sep city autocomplete data.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting sep city autocomplete data.");
            }

            return new JsonResult(results);
        }

        [HttpGet("claim-info/{jobNumber}")]
        public IActionResult getClaimInfo(string jobNumber)
        {
            var filter = $"adoxio_specialeventpermitnumber eq '{jobNumber}' and statuscode eq 845280003";
            var claim = _dynamicsClient.Specialevents.Get(filter: filter).Value.ToList()
            .Select(sepEvent => new SepClaimInfo
            {
                JobNumber = sepEvent.AdoxioSpecialeventpermitnumber,
                AssociatedContactId = sepEvent._adoxioContactidValue

            }).FirstOrDefault();
            return new JsonResult(claim);
        }

        [HttpGet("link-claim-to-contact/{jobNumber}")]
        public IActionResult linkClaimToContact(string jobNumber)
        {

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            // there must be a contact
            if (!string.IsNullOrEmpty(userSettings.ContactId) && userSettings.ContactId != "00000000-0000-0000-0000-000000000000")
            {
                var filter = $"adoxio_specialeventpermitnumber eq '{jobNumber}' and statuscode eq 845280003";
                var claim = _dynamicsClient.Specialevents.Get(filter: filter).Value.FirstOrDefault();
                var patchEvent = new MicrosoftDynamicsCRMadoxioSpecialevent();

                patchEvent.ContactODataBind = _dynamicsClient.GetEntityURI("contacts", userSettings.ContactId);

                // it may have an account too
                if(!string.IsNullOrEmpty(userSettings.AccountId) && userSettings.AccountId != "00000000-0000-0000-0000-000000000000")
                {
                    patchEvent.AccountODataBind = _dynamicsClient.GetEntityURI("accounts", userSettings.AccountId);
                }

                try
                {
                    _dynamicsClient.Specialevents.Update(claim.AdoxioSpecialeventid, patchEvent);
                }
                catch (HttpOperationException httpOperationException)
                {
                    _logger.LogError(httpOperationException, "Error claiming SEP event");
                    throw httpOperationException;
                }
                return Ok();
            }else{
                return BadRequest();
            }
        }
    }

    public class SepClaimInfo
    {
        public string JobNumber { get; set; }
        public string AssociatedContactId { get; set; }
    }

    public class SepPoliceReviewReason
    {
        public string Reason {get; set; }
    }
}