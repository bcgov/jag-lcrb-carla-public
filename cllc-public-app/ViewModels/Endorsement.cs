using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models.Extensions;
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
            
            // get the hours of service and create a table
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

            // get the service areas and get their html
            MicrosoftDynamicsCRMadoxioServiceareaCollection allServiceAreas = _dynamicsClient.Serviceareas.Get(filter: $"_adoxio_endorsement_value eq {EndorsementId}");
            if (allServiceAreas.Value.Count > 0)
            {
                // sort the areas
                IEnumerable<MicrosoftDynamicsCRMadoxioServicearea> serviceAreas = allServiceAreas.Value
                    .Where(area => area.AdoxioAreacategory == (int)ServiceAreaCategoryEnum.Service)
                    .OrderBy(area => area.AdoxioAreanumber);
                IEnumerable<MicrosoftDynamicsCRMadoxioServicearea> outdoorAreas = allServiceAreas.Value
                    .Where(area => area.AdoxioAreacategory == (int)ServiceAreaCategoryEnum.OutdoorEvent)
                    .OrderBy(area => area.AdoxioAreanumber);
                IEnumerable<MicrosoftDynamicsCRMadoxioServicearea> capacityAreas = allServiceAreas.Value
                    .Where(area => area.AdoxioAreacategory == (int)ServiceAreaCategoryEnum.Capacity)
                    .OrderBy(area => area.AdoxioAreanumber);

                // print the service areas
                if (serviceAreas.Any())
                {
                    htmlVal += $@"<h3 style=""text-align: center;"">MAXIMUM CAPACITY {EndorsementName.ToUpper()}</h3>";
                    htmlVal += $@"<table>
                                    <tr>
                                        <th>Area No.</th>
                                        <th>Floor Level</th>
                                        <th>Indoor</th>
                                        <th>Patio</th>
                                        <th>Occupant Load</th>
                                    </tr>";
                    foreach (MicrosoftDynamicsCRMadoxioServicearea area in serviceAreas)
                    {
                        htmlVal += $@"<tr>
                                        <td>{area.AdoxioAreanumber}</td>
                                        <td>{area.AdoxioArealocation}</td>
                                        <td>{((bool)area.AdoxioIsindoor ? "✓" : "✗")}</td>
                                        <td>{((bool)area.AdoxioIspatio ? "✓" : "✗")}</td>
                                        <td style=""font-weight: bold;"">{area.AdoxioCapacity}</td>
                                    </tr>";   

                    }
                    htmlVal += "</table>";
                }

                // print the outdoor areas
                if (outdoorAreas.Any())
                {
                        htmlVal += $@"<h3 style=""text-align: center"">MAXIMUM CAPACITY {EndorsementName.ToUpper()}</h3>";
                        htmlVal += $@"<table>
                                            <tr>
                                                <th>Area No.</th>
                                                <th>Outdoor Area</th>
                                                <th>Capacity</th>
                                            </tr>";
                    foreach (MicrosoftDynamicsCRMadoxioServicearea area in outdoorAreas)
                    {
                        htmlVal += $@"<tr>
                                        <td>{area.AdoxioAreanumber}</td>
                                        <td>{area.AdoxioArealocation}</td>
                                        <td style=""font-weight: bold;"">{area.AdoxioCapacity}</td>
                                    </tr>";
                    }
                    htmlVal += "</table>";
                }

                // print the capacity area (should only be one)
                if (capacityAreas.Any())
                {
                    htmlVal += $@"<h3 style=""text-align: center"">MAXIMUM CAPACITY {EndorsementName.ToUpper()}</h3>";
                    htmlVal += $@"<table><td>Capacity</td><td style=""font-weight: bold;"">{capacityAreas.First().AdoxioCapacity}</td></table>";
                }

            }

            return htmlVal;
        }
    }
}
