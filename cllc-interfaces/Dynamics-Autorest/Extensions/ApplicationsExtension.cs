using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    // Intentional typo in this class name to match name in MS Dynamics
    public partial class Applications : IServiceOperations<DynamicsClient>, IApplications
    {
       
        /// <summary>
        /// Async version of Get Next Link.  Based on the regular Get and intended for use when paging.
        /// </summary>
        /// <param name="nextLink"></param>
        /// <param name="customHeaders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpOperationResponse<MicrosoftDynamicsCRMadoxioApplicationCollection>> GetNextLinkAsync(string nextLink, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            if (nextLink == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "nextLink");
            }

            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("nextLink", nextLink);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "Get", tracingParameters);
            }
            // extract the last portion of the nextlink.
            int questionPos = nextLink.IndexOf("?");
            int slashPos = nextLink.LastIndexOf("/", questionPos) + 1;
            string adjustedNextLink = nextLink.Substring(slashPos);

            // Construct URL
            var _baseUrl = Client.BaseUri.AbsoluteUri;

            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("GET");
            _httpRequest.RequestUri = new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + adjustedNextLink);
            // Set Headers


            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            string _requestContent = null;
            // Set Credentials
            if (Client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            }
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            HttpStatusCode _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 200)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null)
                {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    _responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<MicrosoftDynamicsCRMadoxioApplicationCollection>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<MicrosoftDynamicsCRMadoxioApplicationCollection>(_responseContent, Client.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }

       
    }

    public partial interface IApplications
    {
        /// <summary>
        /// Get the results of a next link
        /// </summary>
        /// <param name="nextLink"></param>
        /// <param name="customHeaders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HttpOperationResponse<MicrosoftDynamicsCRMadoxioApplicationCollection>> GetNextLinkAsync(string nextLink,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default);

    }


    /// <summary>
    /// Extension methods for Applications.
    /// </summary>
    public static partial class ApplicationsExtensions
    {
        /// <summary>
        /// Add reference to adoxio_licenceses
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='licenceId'>
        /// key: adoxio_licenceid
        /// </param>
        /// <param name='fieldname'>
        /// key: fieldname
        /// </param>
        /// <param name='odataid'>
        /// reference value
        /// </param>
        public static HttpOperationResponse<MicrosoftDynamicsCRMadoxioApplicationCollection> GetNextLink(this IApplications operations, string nextLink,
            Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            return operations.GetNextLinkAsync(nextLink, customHeaders, cancellationToken).GetAwaiter().GetResult();
        }

        

    }
}
