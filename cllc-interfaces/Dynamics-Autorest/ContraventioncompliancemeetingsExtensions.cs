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
    /// Extension methods for Contraventioncompliancemeetings.
    /// </summary>
    public static partial class ContraventioncompliancemeetingsExtensions
    {
            /// <summary>
            /// Get adoxio_contravention_compliancemeetings from adoxio_contraventions
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioContraventionid'>
            /// key: adoxio_contraventionid of adoxio_contravention
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
            public static MicrosoftDynamicsCRMadoxioCompliancemeetingCollection Get(this IContraventioncompliancemeetings operations, string adoxioContraventionid, int? top = default(int?), int? skip = default(int?), string search = default(string), string filter = default(string), bool? count = default(bool?), IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>))
            {
                return operations.GetAsync(adoxioContraventionid, top, skip, search, filter, count, orderby, select, expand).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get adoxio_contravention_compliancemeetings from adoxio_contraventions
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioContraventionid'>
            /// key: adoxio_contraventionid of adoxio_contravention
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
            public static async Task<MicrosoftDynamicsCRMadoxioCompliancemeetingCollection> GetAsync(this IContraventioncompliancemeetings operations, string adoxioContraventionid, int? top = default(int?), int? skip = default(int?), string search = default(string), string filter = default(string), bool? count = default(bool?), IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetWithHttpMessagesAsync(adoxioContraventionid, top, skip, search, filter, count, orderby, select, expand, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get adoxio_contravention_compliancemeetings from adoxio_contraventions
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioContraventionid'>
            /// key: adoxio_contraventionid of adoxio_contravention
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
            public static HttpOperationResponse<MicrosoftDynamicsCRMadoxioCompliancemeetingCollection> GetWithHttpMessages(this IContraventioncompliancemeetings operations, string adoxioContraventionid, int? top = default(int?), int? skip = default(int?), string search = default(string), string filter = default(string), bool? count = default(bool?), IList<string> orderby = default(IList<string>), IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.GetWithHttpMessagesAsync(adoxioContraventionid, top, skip, search, filter, count, orderby, select, expand, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get adoxio_contravention_compliancemeetings from adoxio_contraventions
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioContraventionid'>
            /// key: adoxio_contraventionid of adoxio_contravention
            /// </param>
            /// <param name='adoxioCompliancemeetingid'>
            /// key: adoxio_compliancemeetingid of adoxio_compliancemeeting
            /// </param>
            /// <param name='select'>
            /// Select properties to be returned
            /// </param>
            /// <param name='expand'>
            /// Expand related entities
            /// </param>
            public static MicrosoftDynamicsCRMadoxioCompliancemeeting CompliancemeetingsByKey(this IContraventioncompliancemeetings operations, string adoxioContraventionid, string adoxioCompliancemeetingid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>))
            {
                return operations.CompliancemeetingsByKeyAsync(adoxioContraventionid, adoxioCompliancemeetingid, select, expand).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get adoxio_contravention_compliancemeetings from adoxio_contraventions
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioContraventionid'>
            /// key: adoxio_contraventionid of adoxio_contravention
            /// </param>
            /// <param name='adoxioCompliancemeetingid'>
            /// key: adoxio_compliancemeetingid of adoxio_compliancemeeting
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
            public static async Task<MicrosoftDynamicsCRMadoxioCompliancemeeting> CompliancemeetingsByKeyAsync(this IContraventioncompliancemeetings operations, string adoxioContraventionid, string adoxioCompliancemeetingid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CompliancemeetingsByKeyWithHttpMessagesAsync(adoxioContraventionid, adoxioCompliancemeetingid, select, expand, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get adoxio_contravention_compliancemeetings from adoxio_contraventions
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='adoxioContraventionid'>
            /// key: adoxio_contraventionid of adoxio_contravention
            /// </param>
            /// <param name='adoxioCompliancemeetingid'>
            /// key: adoxio_compliancemeetingid of adoxio_compliancemeeting
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
            public static HttpOperationResponse<MicrosoftDynamicsCRMadoxioCompliancemeeting> CompliancemeetingsByKeyWithHttpMessages(this IContraventioncompliancemeetings operations, string adoxioContraventionid, string adoxioCompliancemeetingid, IList<string> select = default(IList<string>), IList<string> expand = default(IList<string>), Dictionary<string, List<string>> customHeaders = null)
            {
                return operations.CompliancemeetingsByKeyWithHttpMessagesAsync(adoxioContraventionid, adoxioCompliancemeetingid, select, expand, customHeaders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            }

    }
}
