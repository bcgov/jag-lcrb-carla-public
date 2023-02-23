using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum EndorsementStatus {
        Active = 1,
        Cancelled = 845280000,
        Suspended = 845280001,
    }
    
    public class Endorsement
    {
        public string EndorsementId { get; set; }
        public string EndorsementName { get; set; }
        public string ApplicationTypeId { get; set; }
        public string ApplicationTypeName { get; set; }
        public List<HoursOfService> HoursOfServiceList { get; set; }
        public int AreaCapacity { get; set; }

        public string SimpleHeader()
        {
            return $@"<h3>{EndorsementName} Approved</h3>";
        }

        public string ToHtml(IDynamicsClient _dynamicsClient)
        {
            string htmlVal = "";

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
                                    <td class='hours'>Start</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioMondayopen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioTuesdayopen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioWednesdayopen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioThursdayopen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioFridayopen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSaturdayopen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSundayopen)}</td>
                                </tr>
                                <tr>
                                    <td class='hours'>End</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioMondayclose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioTuesdayclose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioWednesdayclose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioThursdayclose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioFridayclose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSaturdayclose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString(hoursVal.AdoxioSundayclose)}</td>
                                </tr>
                            </table>";
            }

            // get the service areas and get their html
            MicrosoftDynamicsCRMadoxioServiceareaCollection allServiceAreas = _dynamicsClient.Serviceareas.Get(filter: $"_adoxio_endorsement_value eq {EndorsementId}");
            if (allServiceAreas.Value.Count > 0)
            {
                // sort the areas
                IEnumerable<MicrosoftDynamicsCRMadoxioServicearea> serviceAreas = allServiceAreas.Value
                    .Where(area => area.AdoxioAreacategory != (int)ServiceAreaCategoryEnum.Capacity)
                    .OrderBy(area => area.AdoxioAreanumber);
                // IEnumerable<MicrosoftDynamicsCRMadoxioServicearea> outdoorAreas = allServiceAreas.Value
                //     .Where(area => area.AdoxioAreacategory == (int)ServiceAreaCategoryEnum.OutdoorEvent)
                //     .OrderBy(area => area.AdoxioAreanumber);
                IEnumerable<MicrosoftDynamicsCRMadoxioServicearea> capacityAreas = allServiceAreas.Value
                    .Where(area => area.AdoxioAreacategory == (int)ServiceAreaCategoryEnum.Capacity)
                    .OrderBy(area => area.AdoxioAreanumber);
                if (serviceAreas.Any())
                {
                    htmlVal += $@"<h3 style=""text-align: center;"">MAXIMUM CAPACITY {EndorsementName.ToUpper()}</h3>";
                }
                // print the service areas
                if (serviceAreas.Any())
                {
                    htmlVal += @"<table style='border: black 0px; padding:2px; border-collapse: separate; border-spacing: 2px;'>
                                    <tr>";

                    var cells = 0;
                    var leftover = 0;

                    foreach (MicrosoftDynamicsCRMadoxioServicearea area in serviceAreas)
                    {
                        cells++;

                        htmlVal += $@"<td class='area'><table style='padding:0px; margin: 0px; width:100%; border: 0px solid white;'><tr><td>{area.AdoxioArealocation}</td><td>{area.AdoxioCapacity}</td></tr></table></td>";

                        // every 4 cells
                        leftover = cells % 4;

                        if (leftover == 0)
                        {
                            htmlVal += "</tr><tr>"; // do a new row
                        }

                    }

                    // fill in the remaining spaces 
                    for (int i = 0; i < leftover; i++)
                    {
                        htmlVal += "<td class='space'>&nbsp;</td>";
                    }

                    htmlVal += "</tr></table>";
                }

            }

            return htmlVal;
        }
    }
}