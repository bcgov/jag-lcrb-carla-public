// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces
{
    using Microsoft.Rest;
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Licencesadoxioapplicationrelatedlicence.
    /// </summary>
    public static partial class LicencesadoxioapplicationrelatedlicenceExtensions
    {
            /// <summary>
            /// Get adoxio_licences_adoxio_application_RelatedLicence from
            /// adoxio_licenceses
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioLicencesid'>
            /// key: adoxio_licencesid of adoxio_licences
            /// </param>
            /// <param name='top'>
            /// </param>
            /// <param name='skip'>
            /// </param>
            /// <param name='search'>
            /// </param>
            /// <param name='filter'>
            /// </param>
            /// <param name='count'>
            /// </param>
            /// <param name='orderby'>
            /// Order items by property values
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            public static MicrosoftDynamicsCRMadoxioApplicationCollection Get(this ILicencesadoxioapplicationrelatedlicence operations, string adoxioLicencesid, int? top = default(int?), int? skip = default(int?), string search = default(string), string filter = default(string), bool? count = default(bool?), IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>))
            {
                return operations.GetAsync(adoxioLicencesid, top, skip, search, filter, count, orderby, select, expand).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get adoxio_licences_adoxio_application_RelatedLicence from
            /// adoxio_licenceses
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioLicencesid'>
            /// key: adoxio_licencesid of adoxio_licences
            /// </param>
            /// <param name='top'>
            /// </param>
            /// <param name='skip'>
            /// </param>
            /// <param name='search'>
            /// </param>
            /// <param name='filter'>
            /// </param>
            /// <param name='count'>
            /// </param>
            /// <param name='orderby'>
            /// Order items by property values
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<MicrosoftDynamicsCRMadoxioApplicationCollection> GetAsync(this ILicencesadoxioapplicationrelatedlicence operations, string adoxioLicencesid, int? top = default(int?), int? skip = default(int?), string search = default(string), string filter = default(string), bool? count = default(bool?), IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetWithHttpMessagesAsync(adoxioLicencesid, top, skip, search, filter, count, orderby, select, expand, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get adoxio_licences_adoxio_application_RelatedLicence from
            /// adoxio_licenceses
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioLicencesid'>
            /// key: adoxio_licencesid of adoxio_licences
            /// </param>
            /// <param name='top'>
            /// </param>
            /// <param name='skip'>
            /// </param>
            /// <param name='search'>
            /// </param>
            /// <param name='filter'>
            /// </param>
            /// <param name='count'>
            /// </param>
            /// <param name='orderby'>
            /// Order items by property values
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            /// <param name='customHeaders'>
            /// Headers that will be added to request.
            /// </param>
            public static HttpOperationResponse<MicrosoftDynamicsCRMadoxioApplicationCollection> GetWithHttpMessages(this ILicencesadoxioapplicationrelatedlicence operations, string adoxioLicencesid, int? top = default(int?), int? skip = default(int?), string search = default(string), string filter = default(string), bool? count = default(bool?), IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.GetWithHttpMessagesAsync(adoxioLicencesid, top, skip, search, filter, count, orderby, select, expand, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get adoxio_licences_adoxio_application_RelatedLicence from
            /// adoxio_licenceses
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioLicencesid'>
            /// key: adoxio_licencesid of adoxio_licences
            /// </param>
            /// <param name='adoxioApplicationid'>
            /// key: adoxio_applicationid of adoxio_application
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            public static MicrosoftDynamicsCRMadoxioApplication RelatedLicenceByKey(this ILicencesadoxioapplicationrelatedlicence operations, string adoxioLicencesid, string adoxioApplicationid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>))
            {
                return operations.RelatedLicenceByKeyAsync(adoxioLicencesid, adoxioApplicationid, select, expand).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get adoxio_licences_adoxio_application_RelatedLicence from
            /// adoxio_licenceses
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioLicencesid'>
            /// key: adoxio_licencesid of adoxio_licences
            /// </param>
            /// <param name='adoxioApplicationid'>
            /// key: adoxio_applicationid of adoxio_application
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<MicrosoftDynamicsCRMadoxioApplication> RelatedLicenceByKeyAsync(this ILicencesadoxioapplicationrelatedlicence operations, string adoxioLicencesid, string adoxioApplicationid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.RelatedLicenceByKeyWithHttpMessagesAsync(adoxioLicencesid, adoxioApplicationid, select, expand, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get adoxio_licences_adoxio_application_RelatedLicence from
            /// adoxio_licenceses
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioLicencesid'>
            /// key: adoxio_licencesid of adoxio_licences
            /// </param>
            /// <param name='adoxioApplicationid'>
            /// key: adoxio_applicationid of adoxio_application
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            /// <param name='customHeaders'>
            /// Headers that will be added to request.
            /// </param>
            public static HttpOperationResponse<MicrosoftDynamicsCRMadoxioApplication> RelatedLicenceByKeyWithHttpMessages(this ILicencesadoxioapplicationrelatedlicence operations, string adoxioLicencesid, string adoxioApplicationid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.RelatedLicenceByKeyWithHttpMessagesAsync(adoxioLicencesid, adoxioApplicationid, select, expand, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

    }
}