namespace OneStopHubService
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class http___SOAP_BCPartnerPortTypeClient : System.ServiceModel.ClientBase<OneStopHubService.http___SOAP_BCPartnerPortType>, OneStopHubService.http___SOAP_BCPartnerPortType
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public http___SOAP_BCPartnerPortTypeClient() : 
            base(http___SOAP_BCPartnerPortTypeClient.GetDefaultBinding(), http___SOAP_BCPartnerPortTypeClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.http___SOAP_BCPartnerPort0.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public http___SOAP_BCPartnerPortTypeClient(EndpointConfiguration endpointConfiguration) : 
            base(http___SOAP_BCPartnerPortTypeClient.GetBindingForEndpoint(endpointConfiguration), http___SOAP_BCPartnerPortTypeClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public http___SOAP_BCPartnerPortTypeClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
            base(http___SOAP_BCPartnerPortTypeClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public http___SOAP_BCPartnerPortTypeClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(http___SOAP_BCPartnerPortTypeClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public http___SOAP_BCPartnerPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<OneStopHubService.receiveFromPartnerResponse> receiveFromPartnerAsync(OneStopHubService.receiveFromPartnerRequest request)
        {
            return base.Channel.receiveFromPartnerAsync(request);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.http___SOAP_BCPartnerPort0))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.http___SOAP_BCPartnerPort0))
            {
                return new System.ServiceModel.EndpointAddress("https://twmgateway.gov.bc.ca:4443/soap/rpc");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return http___SOAP_BCPartnerPortTypeClient.GetBindingForEndpoint(EndpointConfiguration.http___SOAP_BCPartnerPort0);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return http___SOAP_BCPartnerPortTypeClient.GetEndpointAddress(EndpointConfiguration.http___SOAP_BCPartnerPort0);
        }
        
        public enum EndpointConfiguration
        {
            
            http___SOAP_BCPartnerPort0,
        }
    }
}