namespace OneStopHubService
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="receiveFromPartnerResponse", WrapperNamespace="http://142.32.211.230/SOAP.BCPartner", IsWrapped=true)]
    public partial class receiveFromPartnerResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string outputXML;
        
        public receiveFromPartnerResponse()
        {
        }
        
        public receiveFromPartnerResponse(string outputXML)
        {
            this.outputXML = outputXML;
        }
    }
}