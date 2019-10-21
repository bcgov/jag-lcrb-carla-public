namespace Gov.Lclb.Cllb.Interfaces
{
    using Microsoft.Rest;

    public class SharePointRestException : RestException
    {

        /// <summary>
        /// Gets information about the associated HTTP request.
        /// </summary>
        public HttpRequestMessageWrapper Request { get; set; }

        /// <summary>
        /// Gets information about the associated HTTP response.
        /// </summary>
        public HttpResponseMessageWrapper Response { get; set; }


        /// <summary>
        /// Initializes a new instance of the HttpOperationException class.
        /// </summary>
        public SharePointRestException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpOperationException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SharePointRestException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpOperationException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public SharePointRestException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

    }
}