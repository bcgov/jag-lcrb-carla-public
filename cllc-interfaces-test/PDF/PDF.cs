
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace PDF.Tests
{
    public class PDF 
    {

        IConfiguration Configuration;

        PdfClient _pdfClient;

        string baseUri;
        string jwtToken;

        /// <summary>
        /// Setup the test
        /// </summary>        
        public PDF()
        {
            Configuration = new ConfigurationBuilder()
                // The following line is the only reason we have a project reference for cllc-public-app.
                // If you were to use this code on a different project simply add user secrets as appropriate to match the environment / secret variables below.
                .AddUserSecrets<Startup>() // Add secrets from the cllc-public-app
                .AddEnvironmentVariables()
                .Build();

            baseUri = Configuration["PDF_SERVICE_BASE_URI"];
            jwtToken = Configuration["PDF_JWT_TOKEN"];

            _pdfClient = new PdfClient(baseUri, $"Bearer {jwtToken}");

        }


        [Fact]
        public async void WorkerLicenceTest()
        {
            var parameters = new Dictionary<string, string>
            {
                { "title", "Worker_Qualification" },
                { "currentDate", DateTime.Now.ToLongDateString() },
                { "firstName", "FirstName" },
                { "middleName", "MiddleName" },
                { "lastName", "LastName" },
                { "dateOfBirth", DateTime.Now.ToString("dd/MM/yyyy")},
                { "address", "123 Main St." },
                { "city", "City" },
                { "province", "Province"},
                { "postalCode", "V091K9"},
                { "effectiveDate", DateTime.Now.ToString("dd/MM/yyyy") },
                { "expiryDate", DateTime.Now.ToString("dd/MM/yyyy") },
                { "border", "{ \"top\": \"40px\", \"right\": \"40px\", \"bottom\": \"0px\", \"left\": \"40px\" }" }
            };

            byte[] data = await _pdfClient.GetPdf(parameters, "worker_qualification_letter");
            Assert.NotNull(data);

        }

        

    }
}
