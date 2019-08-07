using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Gov.Lclb.Cllb.Interfaces
{

    /*
     *  SharePoint Authentication functions.
     *  
     *  The following references were used for these functions:
     *  
     *  https://github.com/SharePoint/PnP/tree/master/Solutions/AspNetCore.Authentication/src
     *  https://github.com/jwillmer/SharePointAuthentication/blob/master/SharePointAuthenticationSkeleton/SharePointAuthenticationSkeleton/Helpers/SharePointAuthentication.cs
     *
     */

    static class Authentication
    {

        private static string GetXMLInnerText(XmlDocument doc, string tagName)
        {
            string result = "";
            var items = doc.GetElementsByTagName(tagName);
            if (items.Count > 0)
            {
                result = items[0].InnerText;
            }
            return result;
        }

        /// <summary>
        /// Convert a RequestedSecurityToken into a RequestSecurityTokenResponse
        /// </summary>
        /// <param name="stsResponse"></param>
        /// <param name="relyingPartyIdentifier"></param>
        /// <returns></returns>
        private static string WrapInSoapMessage(string stsResponse, string relyingPartyIdentifier)
        {
            XmlDocument samlAssertion = new XmlDocument();
            samlAssertion.PreserveWhitespace = true;
            samlAssertion.LoadXml(stsResponse);

            //Select the book node with the matching attribute value.

            string notBefore = GetXMLInnerText(samlAssertion, "wsu:Created");
            string notOnOrAfter = GetXMLInnerText(samlAssertion, "wsu:Expires");
            var requestedTokenRaw = samlAssertion.GetElementsByTagName("t:RequestedSecurityToken")[0];

            var requestedTokenData = samlAssertion.ImportNode(requestedTokenRaw, true);

            XmlDocument requestedTokenXml = new XmlDocument();
            requestedTokenXml.PreserveWhitespace = true;
            requestedTokenXml.LoadXml(requestedTokenData.InnerXml);

            XmlDocument soapMessage = new XmlDocument();
            XmlElement soapEnvelope = soapMessage.CreateElement("t", "RequestSecurityTokenResponse", "http://schemas.xmlsoap.org/ws/2005/02/trust");
            soapMessage.AppendChild(soapEnvelope);
            XmlElement lifeTime = soapMessage.CreateElement("t", "Lifetime", soapMessage.DocumentElement.NamespaceURI);
            soapEnvelope.AppendChild(lifeTime);
            XmlElement created = soapMessage.CreateElement("wsu", "Created", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            XmlText createdValue = soapMessage.CreateTextNode(notBefore);
            created.AppendChild(createdValue);
            lifeTime.AppendChild(created);
            XmlElement expires = soapMessage.CreateElement("wsu", "Expires", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
            XmlText expiresValue = soapMessage.CreateTextNode(notOnOrAfter);
            expires.AppendChild(expiresValue);
            lifeTime.AppendChild(expires);
            XmlElement appliesTo = soapMessage.CreateElement("wsp", "AppliesTo", "http://schemas.xmlsoap.org/ws/2004/09/policy");
            soapEnvelope.AppendChild(appliesTo);
            XmlElement endPointReference = soapMessage.CreateElement("wsa", "EndpointReference", "http://www.w3.org/2005/08/addressing");
            appliesTo.AppendChild(endPointReference);
            XmlElement address = soapMessage.CreateElement("wsa", "Address", endPointReference.NamespaceURI);
            XmlText addressValue = soapMessage.CreateTextNode(relyingPartyIdentifier);
            address.AppendChild(addressValue);
            endPointReference.AppendChild(address);
            XmlElement requestedSecurityToken = soapMessage.CreateElement("t", "RequestedSecurityToken", soapMessage.DocumentElement.NamespaceURI);

            XmlText createdRstValue = soapMessage.CreateTextNode("[RST]");

            requestedSecurityToken.AppendChild(createdRstValue);

            soapEnvelope.AppendChild(requestedSecurityToken);
            XmlElement tokenType = soapMessage.CreateElement("t", "TokenType", soapMessage.DocumentElement.NamespaceURI);
            XmlText tokenTypeValue = soapMessage.CreateTextNode("urn:oasis:names:tc:SAML:1.0:assertion");
            tokenType.AppendChild(tokenTypeValue);
            soapEnvelope.AppendChild(tokenType);
            XmlElement requestType = soapMessage.CreateElement("t", "RequestType", soapMessage.DocumentElement.NamespaceURI);
            XmlText requestTypeValue = soapMessage.CreateTextNode("http://schemas.xmlsoap.org/ws/2005/02/trust/Issue");
            requestType.AppendChild(requestTypeValue);
            soapEnvelope.AppendChild(requestType);
            XmlElement keyType = soapMessage.CreateElement("t", "KeyType", soapMessage.DocumentElement.NamespaceURI);
            XmlText keyTypeValue = soapMessage.CreateTextNode("http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey");
            keyType.AppendChild(keyTypeValue);
            soapEnvelope.AppendChild(keyType);

            string result = soapMessage.OuterXml;
            result = result.Replace("[RST]", requestedTokenRaw.InnerXml);
            return result;
        }



        public static async Task GetFedAuth(string samlSite, string token, string relyingPartyIdentifier, HttpClient client, CookieContainer cookieContainer)
        {
            // Encoding.UTF8.GetString(token.Token, 0, token.Token.Length)
            string samlToken = WrapInSoapMessage(token, relyingPartyIdentifier);

            string samlServer = samlSite.EndsWith("/") ? samlSite : samlSite + "/";
            Uri samlServerRoot = new Uri(samlServer);

            var sharepointSite = new
            {
                Wctx = samlServer + "_layouts/Authenticate.aspx?Source=%2F",
                Wtrealm = samlServer,
                Wreply = $"{samlServer}_trust/"
            };

            // create the body of the POST
            string stringData = $"wa=wsignin1.0&wctx={HttpUtility.UrlEncode(sharepointSite.Wctx)}&wresult={HttpUtility.UrlEncode(samlToken)}";

            var content = new StringContent(stringData, Encoding.UTF8, "application/x-www-form-urlencoded");

            var _httpPostResponse = await client.PostAsync(sharepointSite.Wreply, content);

            var cookieUri = new Uri(sharepointSite.Wreply);

            // if we are using an API gateway we need to restructure the fedAuth cookie.
            if (!string.Equals(sharepointSite.Wreply, $"{cookieUri.Scheme}://{cookieUri.Authority}"))
            {
                var cookies = cookieContainer.GetCookies(cookieUri);
                string fedAuthCookieValue = cookies["FedAuth"].Value;

                cookieContainer.Add(new Uri($"{cookieUri.Scheme}://{cookieUri.Authority}"), new Cookie("FedAuth", fedAuthCookieValue, "/"));
            }

        }


        /// <summary>
        /// Create a SOAP request for a SAML token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="toUrl"></param>
        /// <returns></returns>
        private static string ParameterizeSoapRequestTokenMsgWithUsernamePassword(string url, string username, string password, string toUrl)
        {
            string samlRTString = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://www.w3.org/2005/08/addressing\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">"
                + "<s:Header>"
                + "<a:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue</a:Action>"
                + "<a:ReplyTo><a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address></a:ReplyTo>"
                + "<a:To s:mustUnderstand=\"1\">[toUrl]</a:To>"
                + "<o:Security s:mustUnderstand=\"1\" xmlns:o=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">"
                + "<o:UsernameToken u:Id=\"uuid-6a13a244-dac6-42c1-84c5-cbb345b0c4c4-1\"><o:Username>[username]</o:Username><o:Password>[password]</o:Password></o:UsernameToken></o:Security></s:Header><s:Body>"
                + "<t:RequestSecurityToken xmlns:t=\"http://schemas.xmlsoap.org/ws/2005/02/trust\"><wsp:AppliesTo xmlns:wsp=\"http://schemas.xmlsoap.org/ws/2004/09/policy\">"
                + "<a:EndpointReference><a:Address>[url]</a:Address></a:EndpointReference></wsp:AppliesTo><t:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</t:KeyType>"
                + "<t:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType><t:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</t:TokenType>"
                + "</t:RequestSecurityToken></s:Body></s:Envelope>";


            samlRTString = samlRTString.Replace("[username]", username);
            samlRTString = samlRTString.Replace("[password]", password);
            samlRTString = samlRTString.Replace("[url]", url);
            samlRTString = samlRTString.Replace("[toUrl]", toUrl);

            return samlRTString;
        }

        public async static Task<string> GetStsSamlToken(string spSiteUrl, string username, string password, string stsUrl)
        {
            // Makes a request that conforms with the WS-Trust standard to 
            // the Security Token Service to get a SAML security token back 


            // generate the WS-Trust security token request SOAP message 
            string saml11RT = ParameterizeSoapRequestTokenMsgWithUsernamePassword(
                    spSiteUrl,
                    username,
                    password,
                    stsUrl);

            string response = null;

            if (saml11RT != null)
            {
                var client = new HttpClient();
                var content = new StringContent(saml11RT, System.Text.Encoding.UTF8, "application/soap+xml");
                var result = await client.PostAsync(stsUrl, content);
                response = await result.Content.ReadAsStringAsync();
            }

            return response;
        }

    }
}
