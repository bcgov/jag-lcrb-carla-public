using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class Endorsement
    {
        public string EndorsementId { get; set; }
        public string EndorsementName { get; set; }
        public string ApplicationTypeId { get; set; }
        public string ApplicationTypeName { get; set; }

        public string ToHtml(IDynamicsClient _dynamicsClient)
        {
            string htmlVal = $"<h2>{EndorsementName} Approved</h2>";
            MicrosoftDynamicsCRMadoxioHoursofserviceCollection hours = _dynamicsClient.Hoursofservices.Get(filter: $"_adoxio_endorsement_value eq {EndorsementId}");

            if (hours.Value.Count > 0)
            {
                MicrosoftDynamicsCRMadoxioHoursofservice hoursVal = hours.Value.First();
                htmlVal += $@"<h3 style=""text-align: center;"">HOURS OF SALE FOR {EndorsementName.ToUpper()}</h3>
                            <table style=""width: 100%"">
                                <tr>
                                    <th></th>
                                    <th>Monday</th>
                                    <th>Tuesday</th>
                                    <th>Wednesday</th>
                                    <th>Thursday</th>
                                    <th>Friday</th>
                                    <th>Saturday</th>
                                    <th>Sunday</th>
                                </tr>
                                <tr>
                                    <td>Open</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioMondayopen)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioTuesdayopen)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioWednesdayopen)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioThursdayopen)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioFridayopen)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSaturdayopen)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSundayopen)}</td>
                                </tr>
                                <tr>
                                    <td>Close</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioMondayclose)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioTuesdayclose)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioWednesdayclose)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioThursdayclose)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioFridayclose)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSaturdayclose)}</td>
                                    <td>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSundayclose)}</td>
                                </tr>
                            </table>";
            }
            return htmlVal;
        }
    }
}
