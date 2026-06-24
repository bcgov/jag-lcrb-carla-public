extern alias DV;
using Gov.Lclb.Cllb.Public.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using adoxio_servicearea_adoxio_areacategory = DV::Gov.Lclb.Cllb.Interfaces.adoxio_servicearea_adoxio_areacategory;

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

        public async Task<string> ToHtmlAsync(IDataverseClient _dataverse)
        {
            string htmlVal = "";

            var hours = await _dataverse.GetHoursOfSaleByEndorsementIdAsync(EndorsementId);
            if (hours.Count > 0)
            {
                var hoursVal = hours.First();
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
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_MondayOpen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_TuesdayOpen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_WednesdayOpen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_ThursdayOpen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_FridayOpen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_SaturdayOpen)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_SundayOpen)}</td>
                                </tr>
                                <tr>
                                    <td class='hours'>End</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_MondayClose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_TuesdayClose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_WednesdayClose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_ThursdayClose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_FridayClose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_SaturdayClose)}</td>
                                    <td class='hours'>{StoreHoursUtility.ConvertOpenHoursToString((int?)hoursVal.adoxio_SundayClose)}</td>
                                </tr>
                            </table>";
            }

            var allServiceAreas = await _dataverse.GetServiceAreasByEndorsementIdAsync(EndorsementId);
            if (allServiceAreas.Count > 0)
            {
                var serviceAreas = allServiceAreas
                    .Where(area => area.adoxio_areacategory != adoxio_servicearea_adoxio_areacategory.No)
                    .OrderBy(area => area.adoxio_areanumber);

                if (serviceAreas.Any())
                {
                    htmlVal += $@"<h3 style=""text-align: center;"">MAXIMUM CAPACITY {EndorsementName.ToUpper()}</h3>";

                    htmlVal += @"<table style='border: black 0px; padding:2px; border-collapse: separate; border-spacing: 2px;'>
                                    <tr>";

                    var cells = 0;
                    var leftover = 0;

                    foreach (var area in serviceAreas)
                    {
                        cells++;
                        htmlVal += $@"<td class='area'><table style='padding:0px; margin: 0px; width:100%; border: 0px solid white;'><tr><td>{area.adoxio_arealocation}</td><td>{area.adoxio_capacity}</td></tr></table></td>";
                        leftover = cells % 4;
                        if (leftover == 0)
                        {
                            htmlVal += "</tr><tr>";
                        }
                    }

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