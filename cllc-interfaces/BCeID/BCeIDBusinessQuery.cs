using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public class BCeIDBusinessQuery
    {
        private string svcid;
        private string user;
        private string password;
        private string url;

		//private static readonly HttpClient client = new HttpClient();

        public BCeIDBusinessQuery(string svcid, string user, string password, string url)
        {
            this.svcid = svcid;
            this.user = user;
            this.password = password;
            this.url = url;
        }

        public string NormalizeGuid(string guid) 
        {
            return guid.ToUpper().Replace("-", "");
        }

        public async Task<BCeIDBusiness> ProcessBusinessQuery(string guid) 
        {
            if (String.IsNullOrEmpty(url)){
                var temp_business = new BCeIDBusiness();
                temp_business.contactEmail = "ian.costanzo@quartech.com";
                temp_business.contactPhone = "250-555-1234";

                temp_business.individualFirstname = "Chief";
                temp_business.individualMiddlename = "";
                temp_business.individualOtherMiddlename = "";
                temp_business.individualSurname = "Developer1";

                temp_business.businessTypeName = "";
                temp_business.businessTypeDescription = "";
                temp_business.businessTypeCode = "Other";
                temp_business.businessTypeOther = "Development Account for Cannabis Licensing System";
                temp_business.legalName = "ABC Cannabis Sales Dev";
                temp_business.businessNumber = "";
                temp_business.incorporationNumber = "";
                temp_business.jurisdictionOfIncorporation = "";
                temp_business.addressLine1 = "123 Any Street";
                temp_business.addressLine2 = "";
                temp_business.addressCity  = "Victoria";
                temp_business.addressProv  = "BC";
                temp_business.addressPostal = "V8W1P6";
                temp_business.addressCountry = "CA";
                return temp_business;
            }
                
            // create the SOAP client
            //var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            BasicHttpsBinding binding = new BasicHttpsBinding { MaxReceivedMessageSize = int.MaxValue };
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.CloseTimeout = new TimeSpan(0, 10, 0);
            EndpointAddress address = new EndpointAddress(url);
            var client = new BCeIDServiceSoapClient(binding, address);

            client.ClientCredentials.UserName.UserName = user;
            client.ClientCredentials.UserName.Password = password;

            var n_guid = NormalizeGuid(guid);

            // SOAP request and parameters
            var myparams = new AccountDetailRequest();
			myparams.onlineServiceId = svcid;
			myparams.requesterUserGuid = n_guid;
            myparams.requesterAccountTypeCode = BCeIDAccountTypeCode.Business;
            myparams.userGuid = n_guid;
            myparams.accountTypeCode = BCeIDAccountTypeCode.Business;

			try
            {
                var response = await client.getAccountDetailAsync(myparams);

    			if (response.code == ResponseCode.Success)
    			{
                	var business = new BCeIDBusiness();
                    BCeIDAccount account = response.account;

    				business.contactEmail = account.contact.email.value;
    				business.contactPhone = account.contact.telephone.value;

    				business.individualFirstname = account.individualIdentity.name.firstname.value;
    				business.individualMiddlename = account.individualIdentity.name.middleName.value;
    				business.individualOtherMiddlename = account.individualIdentity.name.otherMiddleName.value;
    				business.individualSurname = account.individualIdentity.name.surname.value;

    				business.businessTypeName = account.business.type.name;
    				business.businessTypeDescription = account.business.type.description;
    				business.businessTypeCode = account.business.type.code.ToString();
    				business.businessTypeOther = account.business.businessTypeOther.value;
    				business.legalName = account.business.legalName.value;
    				business.businessNumber = account.business.businessNumber.value;
    				business.incorporationNumber = account.business.incorporationNumber.value;
    				business.jurisdictionOfIncorporation = account.business.jurisdictionOfIncorporation.value;
    				business.addressLine1 = account.business.address.addressLine1.value;
    				business.addressLine2 = account.business.address.addressLine2.value;
    				business.addressCity  = account.business.address.city.value;
    				business.addressProv  = account.business.address.province.value;
    				business.addressPostal = account.business.address.postal.value;
    				business.addressCountry = account.business.address.country.value;

    				return business;
                }
            }
            catch (Exception e)
            {
                // ignore errors and just return null
            }

			return null;
        }
    }
}
