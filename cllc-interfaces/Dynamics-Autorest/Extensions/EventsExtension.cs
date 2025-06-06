using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Rest;

using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    // Intentional typo in this class name to match name in MS Dynamics
    public partial class Events : IServiceOperations<DynamicsClient>, IEvents
    {

        /// <summary>
        /// Add reference to adoxio_events
        /// </summary>
        /// <param name='eventId'>
        /// key: adoxio_eventid
        /// </param>
        /// <param name='fieldname'>
        /// key: fieldname
        /// </param>
        /// <param name='odataid'>
        /// reference value
        /// </param>
        /// <param name='customHeaders'>
        /// Headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        public async Task<HttpOperationResponse> AddReferenceWithHttpMessagesAsync(string eventId, string fieldname, Odataid odataid = default, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            if (eventId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "eventId");
            }
            if (fieldname == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "fieldname");
            }
            if (odataid != null)
            {
                odataid.Validate();
            }
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("adoxio_eventid", eventId);
                tracingParameters.Add("fieldname", fieldname);
                tracingParameters.Add("odataid", odataid);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "AddReference", tracingParameters);
            }
            // Construct URL
            var _baseUrl = Client.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "adoxio_events({adoxio_eventid})/{fieldname}/$ref").ToString();
            _url = _url.Replace("{adoxio_eventid}", System.Uri.EscapeDataString(eventId));
            _url = _url.Replace("{fieldname}", System.Uri.EscapeDataString(fieldname));
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("POST");
            _httpRequest.RequestUri = new System.Uri(_url);
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
            if (odataid != null)
            {
                _requestContent = Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(odataid, Client.SerializationSettings);
                _httpRequest.Content = new StringContent(_requestContent, System.Text.Encoding.UTF8);
                _httpRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }
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
            if ((int)_statusCode != 204)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                try
                {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Odataerror _errorBody = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<Odataerror>(_responseContent, Client.DeserializationSettings);
                    if (_errorBody != null)
                    {
                        ex.Body = _errorBody;
                    }
                }
                catch (JsonException)
                {
                    // Ignore the exception
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
            var _result = new HttpOperationResponse();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }


        /// <summary>
        /// Get adoxio_event_schedules from adoxio_events
        /// </summary>
        /// <param name='adoxioEventid'>
        /// key: adoxio_eventid of adoxio_event
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
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        /// <return>
        /// A response object containing the response body and response headers.
        /// </return>
        public async Task<HttpOperationResponse<MicrosoftDynamicsCRMadoxioEventscheduleCollection>> GetEventscheduleByEventWithHttpMessagesAsync(string adoxioEventid, int? top = default, int? skip = default, string search = default, string filter = default, bool? count = default, IList<string> orderby = default, IList<string> select = default, IList<string> expand = default, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            if (adoxioEventid == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "adoxioEventid");
            }
            // Tracing
            bool _shouldTrace = ServiceClientTracing.IsEnabled;
            string _invocationId = null;
            if (_shouldTrace)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                Dictionary<string, object> tracingParameters = new Dictionary<string, object>();
                tracingParameters.Add("adoxioEventid", adoxioEventid);
                tracingParameters.Add("top", top);
                tracingParameters.Add("skip", skip);
                tracingParameters.Add("search", search);
                tracingParameters.Add("filter", filter);
                tracingParameters.Add("count", count);
                tracingParameters.Add("orderby", orderby);
                tracingParameters.Add("select", select);
                tracingParameters.Add("expand", expand);
                tracingParameters.Add("cancellationToken", cancellationToken);
                ServiceClientTracing.Enter(_invocationId, this, "Get", tracingParameters);
            }
            // Construct URL
            var _baseUrl = Client.BaseUri.AbsoluteUri;
            var _url = new System.Uri(new System.Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), "adoxio_events({adoxio_eventid})/adoxio_event_schedules").ToString();
            _url = _url.Replace("{adoxio_eventid}", System.Uri.EscapeDataString(adoxioEventid));
            List<string> _queryParameters = new List<string>();
            if (top != null)
            {
                _queryParameters.Add(string.Format("$top={0}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(top, Client.SerializationSettings).Trim('"'))));
            }
            if (skip != null)
            {
                _queryParameters.Add(string.Format("$skip={0}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(skip, Client.SerializationSettings).Trim('"'))));
            }
            if (search != null)
            {
                _queryParameters.Add(string.Format("$search={0}", System.Uri.EscapeDataString(search)));
            }
            if (filter != null)
            {
                _queryParameters.Add(string.Format("$filter={0}", System.Uri.EscapeDataString(filter)));
            }
            if (count != null)
            {
                _queryParameters.Add(string.Format("$count={0}", System.Uri.EscapeDataString(Microsoft.Rest.Serialization.SafeJsonConvert.SerializeObject(count, Client.SerializationSettings).Trim('"'))));
            }
            if (orderby != null)
            {
                _queryParameters.Add(string.Format("$orderby={0}", System.Uri.EscapeDataString(string.Join(",", orderby))));
            }
            if (select != null)
            {
                _queryParameters.Add(string.Format("$select={0}", System.Uri.EscapeDataString(string.Join(",", select))));
            }
            if (expand != null)
            {
                _queryParameters.Add(string.Format("$expand={0}", System.Uri.EscapeDataString(string.Join(",", expand))));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("GET");
            _httpRequest.RequestUri = new System.Uri(_url);
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
            var _result = new HttpOperationResponse<MicrosoftDynamicsCRMadoxioEventscheduleCollection>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;
            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<MicrosoftDynamicsCRMadoxioEventscheduleCollection>(_responseContent, Client.DeserializationSettings);
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

    public partial interface IEvents
    {
        /// <summary>
        /// Add reference to adoxio_events
        /// </summary>
        /// <param name='eventId'>
        /// key: adoxio_eventid
        /// </param>
        /// <param name='fieldname'>
        /// key: fieldname
        /// </param>
        /// <param name='odataid'>
        /// reference value
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse> AddReferenceWithHttpMessagesAsync(string eventId, string fieldname, Odataid odataid = default, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default);


        /// <summary>
        /// Get adoxio_event_schedules from adoxio_events
        /// </summary>
        /// <param name='adoxioEventid'>
        /// key: adoxio_eventid of adoxio_event
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
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        /// <exception cref="Microsoft.Rest.HttpOperationException">
        /// Thrown when the operation returned an invalid status code
        /// </exception>
        /// <exception cref="Microsoft.Rest.SerializationException">
        /// Thrown when unable to deserialize the response
        /// </exception>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown when a required parameter is null
        /// </exception>
        Task<HttpOperationResponse<MicrosoftDynamicsCRMadoxioEventscheduleCollection>> GetEventscheduleByEventWithHttpMessagesAsync(string adoxioEventid, int? top = default, int? skip = default, string search = default, string filter = default, bool? count = default, IList<string> orderby = default, IList<string> select = default, IList<string> expand = default, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default);

    }


    /// <summary>
    /// Extension methods for Events.
    /// </summary>
    public static partial class EventsExtensions
    {
        /// <summary>
        /// Add reference to adoxio_events
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='eventId'>
        /// key: adoxio_eventid
        /// </param>
        /// <param name='fieldname'>
        /// key: fieldname
        /// </param>
        /// <param name='odataid'>
        /// reference value
        /// </param>
        public static void AddReference(this IEvents operations, string eventId, string fieldname, Odataid odataid = default)
        {
            operations.AddReferenceAsync(eventId, fieldname, odataid).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Add reference to adoxio_events
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='eventId'>
        /// key: adoxio_eventid
        /// </param>
        /// <param name='fieldname'>
        /// key: fieldname
        /// </param>
        /// <param name='odataid'>
        /// reference value
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        public static async Task AddReferenceAsync(this IEvents operations, string eventId, string fieldname, Odataid odataid = default, CancellationToken cancellationToken = default)
        {
            (await operations.AddReferenceWithHttpMessagesAsync(eventId, fieldname, odataid, null, cancellationToken).ConfigureAwait(false)).Dispose();
        }

        /// <summary>
        /// Get Events schedule by event id
        /// </summary>
        /// <param name='operations'>
        /// The operations group for this extension method.
        /// </param>
        /// <param name='eventId'>
        /// key: adoxio_eventid
        /// </param>

        public static MicrosoftDynamicsCRMadoxioEventscheduleCollection GetEventscheduleByEvent(this IEvents operations, string eventId)
        {
            return operations.GetEventscheduleByEventAsync(eventId).GetAwaiter().GetResult();
        }


        public static async Task<MicrosoftDynamicsCRMadoxioEventscheduleCollection> GetEventscheduleByEventAsync(this IEvents operations, string eventId)
        {
            var result = await operations.GetEventscheduleByEventWithHttpMessagesAsync(eventId, null);
            return result.Body;
        }
    }
}
