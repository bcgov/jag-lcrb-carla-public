using System.Net.Http;

/**
 * A DelegatingHandler implementation for unit testing purposes.
 */
public class TestHttpHandler : DelegatingHandler
{
    public TestHttpHandler() : base()
    {
    }

    public TestHttpHandler(HttpMessageHandler innerHandler) : base(innerHandler)
    {
    }
}