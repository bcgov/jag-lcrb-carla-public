using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BCRegWrapper;

namespace BCRegWrapperTest
{
    [TestClass]
    public class CompanyQueryUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        	var svc_userid = Environment.GetEnvironmentVariable("BCREG_SERVICE_ACCT");
        	var svc_passwd = Environment.GetEnvironmentVariable("BCREG_SERVICE_PASSWD");

            var company = CompanyQuery.ProcessCompanyQuery(svc_userid, svc_passwd, "https://twmgateway.gov.bc.ca/rest/ltsa/v1/corporations/0410721").Result;
            Assert.IsNotNull(company);
            Assert.AreEqual(company.name, "A-ZANON'S GARDENING & LANDSCAPING LTD.");
            Assert.AreEqual(company.number, "0410721");
            Assert.AreEqual(company.corporationType, "BC");
            Assert.AreEqual(company.corporationClass, "BC");
            Assert.IsNull(company.bn9);
            Assert.IsNull(company.bn15);
            Assert.AreEqual(company.jurisdiction, "BC");
            Assert.AreEqual(company.stateCode, "LRS");
            Assert.AreEqual(company.stateActive, "true");
            Assert.AreEqual(company.stateDescription, "IN LIM RESTORE");
        }

        [TestMethod]
        public void TestMethod2()
        {
        	var svc_userid = Environment.GetEnvironmentVariable("BCREG_SERVICE_ACCT");
        	var svc_passwd = Environment.GetEnvironmentVariable("BCREG_SERVICE_PASSWD");

            var company = CompanyQuery.ProcessCompanyQuery(svc_userid, svc_passwd, "https://twmgateway.gov.bc.ca/rest/ltsa/v1/corporations/IDONTEXIST").Result;
            Assert.IsNull(company);
        }

        [TestMethod]
        public void TestMethod3()
        {
        	var svc_userid = Environment.GetEnvironmentVariable("BCREG_SERVICE_ACCT");

            var company = CompanyQuery.ProcessCompanyQuery(svc_userid, "bad_passwd", "https://twmgateway.gov.bc.ca/rest/ltsa/v1/corporations/0410721").Result;
            Assert.IsNull(company);
        }
    }
}
