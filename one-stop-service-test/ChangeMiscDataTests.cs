using System;
using System.Xml.Linq;
using Gov.Jag.Lcrb.OneStopService;
using Gov.Jag.Lcrb.OneStopService.OneStop;
using Xunit;

namespace Gov.Jag.Lcrb.OneStopService.Test
{
    /// <summary>
    /// Pins the endorsement (SBNChangeMiscData) message. The expected values come from
    /// the messages LDB confirmed it accepts; the messages themselves are not committed,
    /// because they carry a real business number and licence number.
    /// </summary>
    public class ChangeMiscDataTests
    {
        // Synthetic fixtures. The values are arbitrary - the tests assert the message
        // carries whatever is on the licence record, not any particular number - so no
        // real business number or licence number is committed here.
        private const string TestBn9 = "123456789";
        private const string TestLicenceNumber = "999001";
        private const string TestProgramAccountRef = "001";

        private const string Winery = EndorsementMiscDataMap.LICENCE_SUB_CATEGORY_WINERY;
        private const string Lounge = EndorsementMiscDataMap.LOUNGE_AREA_ENDORSEMENT;
        private const string OnSiteStore = EndorsementMiscDataMap.ON_SITE_STORE_ENDORSEMENT;
        private const string SpecialEventArea = EndorsementMiscDataMap.SPECIAL_EVENT_AREA_ENDORSEMENT;

        private static OneStopLicenceData Licence(string subCategory = null,
            string programAccountRef = TestProgramAccountRef)
        {
            return new OneStopLicenceData
            {
                LicenceId = Guid.NewGuid().ToString(),
                LicenceNumber = TestLicenceNumber,
                BusinessProgramAccountReferenceNumber = programAccountRef,
                LicenceSubCategory = subCategory,
                Licencee = new OneStopAccount { AccountNumber = TestBn9, Name = "Test Licensee" }
            };
        }

        private static OneStopEndorsement Endorsement(string name)
            => new OneStopEndorsement { EndorsementId = Guid.NewGuid().ToString(), Name = name };

        private static XElement Build(string endorsementName, string subCategory,
            string updateReasonCode = OneStopUtils.UPDATE_REASON_CODE_MISCDATA_NEW,
            string programAccountRef = TestProgramAccountRef)
        {
            var xml = new ChangeMiscData().CreateXML(
                Licence(subCategory, programAccountRef), Endorsement(endorsementName), updateReasonCode);
            return XElement.Parse(xml);
        }

        private static string Field(XElement doc, string section, string name)
            => doc.Element(section)?.Element(name)?.Value;

        [Theory]
        // endorsement, sub-category, expected miscDataTypeCode, expected documentSubType.
        // The three pairings confirmed accepted by LDB.
        [InlineData(Lounge, Winery, "WINELNG", "300")]
        [InlineData(OnSiteStore, "Brewery", "MFRONS", "302")]
        [InlineData(SpecialEventArea, "Brewery", "MFRSEA", "303")]
        public void CreateXML_UsesTheConfirmedCodes(string endorsementName, string subCategory,
            string expectedCode, string expectedSubType)
        {
            var actual = Build(endorsementName, subCategory);

            Assert.Equal(expectedSubType, Field(actual, "header", "documentSubType"));
            Assert.Equal(expectedCode, Field(actual, "body", "miscDataTypeCode"));
        }

        [Theory]
        [InlineData(Lounge, Winery, "WINELNG", "300")]
        [InlineData(OnSiteStore, "Brewery", "MFRONS", "302")]
        [InlineData(SpecialEventArea, "Brewery", "MFRSEA", "303")]
        public void CreateXML_SetsEveryFieldTheMessageNeeds(string endorsementName, string subCategory,
            string expectedCode, string expectedSubType)
        {
            var actual = Build(endorsementName, subCategory);

            // Header. No CCRAHeader, and addressed to LDB rather than the Hub.
            Assert.Equal("A", Field(actual, "header", "requestMode"));
            Assert.Equal(expectedSubType, Field(actual, "header", "documentSubType"));
            Assert.Equal("LCRB", Field(actual, "header", "senderID"));
            Assert.Equal("LDB", Field(actual, "header", "receiverID"));

            // Body.
            Assert.Equal(TestBn9, Field(actual, "body", "businessRegistrationNumber"));
            Assert.Equal("BB", Field(actual, "body", "businessProgramIdentifier"));
            Assert.Equal(TestProgramAccountRef, Field(actual, "body", "businessProgramAccountReferenceNumber"));
            Assert.Equal(expectedCode, Field(actual, "body", "miscDataTypeCode"));
            Assert.Equal("03", Field(actual, "body", "updateReasonCode"));
            Assert.Equal(TestLicenceNumber, Field(actual, "body", "miscDataValue"));
            Assert.Equal(TestLicenceNumber, Field(actual, "body", "partnerInfo1"));
            Assert.Equal("LCRB", Field(actual, "body", "partnerInfo3"));

            // partnerInfo2 is deliberately not set: optional, not stored in the BNI,
            // and its meaning has not been confirmed by LDB or the Hub.
            Assert.Null(actual.Element("body")?.Element("partnerInfo2"));
        }

        [Fact]
        public void CreateXML_NeverReusesAConfirmedSubtypeForAnotherDataType()
        {
            // Each confirmed subtype belongs to exactly one miscDataTypeCode. Pairing a
            // subtype with a different data type was tried and rejected, so assert the
            // three confirmed pairings cannot drift apart.
            var pairs = new[]
            {
                (Endorsement: Lounge, SubCategory: Winery,
                    Code: OneStopUtils.MISC_DATA_TYPE_WINERY_LOUNGE,
                    SubType: OneStopUtils.DOCUMENT_SUBTYPE_MISCDATA_WINERY_LOUNGE),
                (Endorsement: OnSiteStore, SubCategory: "Brewery",
                    Code: OneStopUtils.MISC_DATA_TYPE_MANUFACTURER_ONSITE,
                    SubType: OneStopUtils.DOCUMENT_SUBTYPE_MISCDATA_ONSITE),
                (Endorsement: SpecialEventArea, SubCategory: "Brewery",
                    Code: OneStopUtils.MISC_DATA_TYPE_MANUFACTURER_SEA,
                    SubType: OneStopUtils.DOCUMENT_SUBTYPE_MISCDATA_SEA)
            };

            foreach (var p in pairs)
            {
                var actual = Build(p.Endorsement, p.SubCategory);
                Assert.Equal(p.Code, Field(actual, "body", "miscDataTypeCode"));
                Assert.Equal(p.SubType, Field(actual, "header", "documentSubType"));
            }
        }

        [Fact]
        public void CreateXML_AddressesTheMessageToLdb()
        {
            // SBNChangeMiscData goes to LDB and is forwarded by the Hub, unlike every
            // other message we send, which is addressed to BCSBNHUB.
            Assert.Equal("LDB", Field(Build(OnSiteStore, "Brewery"), "header", "receiverID"));
        }

        [Fact]
        public void CreateXML_OmitsTheCcraHeader()
        {
            // The Messaging Technical Specification Guide requires CCRAHeader be omitted
            // for the Miscellaneous Data Update document.
            Assert.Null(Build(OnSiteStore, "Brewery").Element("header")?.Element("CCRAHeader"));
        }

        [Fact]
        public void CreateXML_UsesLicenceNumberAsMiscDataValue()
        {
            // LDB matches on miscDataValue and it must be a licence number it already holds.
            Assert.Equal(TestLicenceNumber, Field(Build(OnSiteStore, "Brewery"), "body", "miscDataValue"));
        }

        [Theory]
        [InlineData(OneStopUtils.UPDATE_REASON_CODE_MISCDATA_NEW, "03")]
        [InlineData(OneStopUtils.UPDATE_REASON_CODE_MISCDATA_CANCEL, "05")]
        public void CreateXML_CarriesTheUpdateReasonCode(string reasonCode, string expected)
        {
            Assert.Equal(expected, Field(Build(OnSiteStore, "Brewery", reasonCode), "body", "updateReasonCode"));
        }

        [Theory]
        [InlineData("1", "001")]
        [InlineData("001", "001")]
        [InlineData("10", "010")]
        public void CreateXML_ZeroPadsTheProgramAccountReferenceNumber(string stored, string expected)
        {
            var actual = Build(OnSiteStore, "Brewery", OneStopUtils.UPDATE_REASON_CODE_MISCDATA_NEW, stored);
            Assert.Equal(expected, Field(actual, "body", "businessProgramAccountReferenceNumber"));
        }

        [Fact]
        public void CreateXML_UsesTheSamePartnerNotePatternAsTheOtherMessages()
        {
            var partnerNote = Field(Build(OnSiteStore, "Brewery"), "header", "partnerNote");

            Assert.StartsWith(TestLicenceNumber + "-", partnerNote);
            Assert.True(long.TryParse(partnerNote.Substring(TestLicenceNumber.Length + 1), out _),
                "partnerNote should be the licence number plus ticks, was " + partnerNote);
        }

        [Fact]
        public void CreateXML_ThrowsWhenTheLicenceHasNoLicenceNumber()
        {
            var licence = Licence("Brewery");
            licence.LicenceNumber = null;

            var ex = Assert.Throws<Exception>(() => new ChangeMiscData().CreateXML(
                licence, Endorsement(OnSiteStore), OneStopUtils.UPDATE_REASON_CODE_MISCDATA_NEW));
            Assert.Contains("LicenceNumber", ex.Message);
        }

        [Fact]
        public void CreateXML_ThrowsForAnEndorsementWithNoMappedCode()
        {
            var ex = Assert.Throws<Exception>(() => new ChangeMiscData().CreateXML(
                Licence("Brewery"), Endorsement("Catering Endorsement"),
                OneStopUtils.UPDATE_REASON_CODE_MISCDATA_NEW));
            Assert.Contains("Catering Endorsement", ex.Message);
        }

        [Theory]
        [InlineData("Brewery")]
        [InlineData("Distillery")]
        [InlineData("Co-Packer")]
        [InlineData(null)]
        public void CreateXML_ThrowsForALoungeOnANonWineryLicence(string subCategory)
        {
            // Would need MFRLNG, which has no confirmed subtype of its own.
            var ex = Assert.Throws<Exception>(() => new ChangeMiscData().CreateXML(
                Licence(subCategory), Endorsement(Lounge),
                OneStopUtils.UPDATE_REASON_CODE_MISCDATA_NEW));
            Assert.Contains(Lounge, ex.Message);
        }
    }

    public class EndorsementMiscDataMapTests
    {
        private const string Winery = EndorsementMiscDataMap.LICENCE_SUB_CATEGORY_WINERY;
        private const string Lounge = EndorsementMiscDataMap.LOUNGE_AREA_ENDORSEMENT;

        [Fact]
        public void Resolve_SendsALoungeOnlyForAWineryLicence()
        {
            var winery = EndorsementMiscDataMap.Resolve(Lounge, Winery);
            Assert.Equal(OneStopUtils.MISC_DATA_TYPE_WINERY_LOUNGE, winery.MiscDataTypeCode);
            Assert.Equal(OneStopUtils.DOCUMENT_SUBTYPE_MISCDATA_WINERY_LOUNGE, winery.DocumentSubType);
        }

        [Theory]
        [InlineData("Brewery")]
        [InlineData("Distillery")]
        [InlineData("Co-Packer")]
        [InlineData(null)]
        public void Resolve_DoesNotSendALoungeOnANonWineryLicence(string subCategory)
        {
            // MFRLNG has no confirmed documentSubType of its own; reusing 300 was
            // tried and rejected, so a non-winery lounge must not be sent.
            Assert.Null(EndorsementMiscDataMap.Resolve(Lounge, subCategory));
            Assert.False(EndorsementMiscDataMap.IsSupported(Lounge, subCategory));
        }

        [Theory]
        // Endorsements LCRB holds that OneStop has no confirmed code for. These must be
        // skipped rather than sent under a guessed code.
        [InlineData("Catering Endorsement")]
        [InlineData("Picnic Area Endorsement")]
        [InlineData("Temporary Use Area Endorsement")]
        [InlineData("Patron Participation Entertainment Endorsement")]
        [InlineData("Off-Site Store Endorsement")]
        [InlineData("Temporary Off-Site Sales Endorsement")]
        [InlineData("")]
        [InlineData(null)]
        public void Resolve_ReturnsNullForUnmappedEndorsements(string endorsementName)
        {
            Assert.Null(EndorsementMiscDataMap.Resolve(endorsementName, "Brewery"));
            Assert.False(EndorsementMiscDataMap.IsSupported(endorsementName, "Brewery"));
        }

        [Fact]
        public void Resolve_NeverReturnsAnUnsendableDataType()
        {
            // MFRLNG and MFREND have no confirmed subtype, so nothing may resolve to them.
            var unsendable = new[]
            {
                OneStopUtils.MISC_DATA_TYPE_MANUFACTURER_LOUNGE,
                OneStopUtils.MISC_DATA_TYPE_MANUFACTURER_ENDORSEMENT
            };

            var endorsements = new[]
            {
                Lounge,
                EndorsementMiscDataMap.ON_SITE_STORE_ENDORSEMENT,
                EndorsementMiscDataMap.SPECIAL_EVENT_AREA_ENDORSEMENT
            };

            foreach (var endorsement in endorsements)
            foreach (var subCategory in new[] { Winery, "Brewery", "Distillery", "Co-Packer", null })
            {
                var codes = EndorsementMiscDataMap.Resolve(endorsement, subCategory);
                if (codes != null)
                    Assert.DoesNotContain(codes.MiscDataTypeCode, unsendable);
            }
        }

        [Fact]
        public void Resolve_IgnoresCasingAndSurroundingWhitespace()
        {
            Assert.NotNull(EndorsementMiscDataMap.Resolve("  on-site store endorsement  ", "Brewery"));
        }
    }
}
