using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;

namespace one_stop_service.Controllers
{
    [Route("api/[controller]")]
    public class OneStopController : Controller
    {
        [Route("[action]")]
        public async Task<IActionResult> SendLicenceCreationMessage()
        {
            var cred = new System.ServiceModel.Description.ClientCredentials() {

            };
            cred.UserName.UserName = "HubPartnerLCRB";
            cred.UserName.Password = "LCRBtest@0904";
            var serviceClient = new OneStopServiceReference.receiveFromPartner_PortTypeClient();
            var basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            serviceClient.Endpoint.Binding = basicHttpBinding;

            using (new OperationContextScope(serviceClient.InnerChannel))
            {
                //Create message header containing the credentials
                var header = new OneStopServiceReference.SoapSecurityHeader("", "HubPartnerLCRB", "LCRBtest@0904", "");
                //Add the credentials message header to the outgoing request
                OperationContext.Current.OutgoingMessageHeaders.Add(header);


                //try
                //{
                    var version = await serviceClient.receiveFromPartnerAsync("");
                //}
                //catch (Exception ex)
                //{
                //    throw;
                //}
            }



            


            return Ok();
        }

    }
}