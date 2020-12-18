namespace OneStopHubService
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://142.32.211.230/", ConfigurationName="OneStopHubService.http___SOAP_BCPartnerPortType")]
    public interface http___SOAP_BCPartnerPortType
    {
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        System.Threading.Tasks.Task<OneStopHubService.receiveFromPartnerResponse> receiveFromPartnerAsync(OneStopHubService.receiveFromPartnerRequest request);
    }
}