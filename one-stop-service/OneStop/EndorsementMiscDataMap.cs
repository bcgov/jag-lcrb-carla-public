using System;
using System.Collections.Generic;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    /// <summary>
    /// The OneStop miscellaneous data codes for one endorsement.
    /// </summary>
    public class EndorsementMiscDataType
    {
        public string MiscDataTypeCode { get; set; }
        public string DocumentSubType { get; set; }
    }

    /// <summary>
    /// Maps an LCRB endorsement onto the OneStop miscDataTypeCode / documentSubType
    /// pair that LDB expects in an SBNChangeMiscData message.
    /// <para>
    /// Three endorsement codes are confirmed: WINELNG (subtype 300), MFRONS (302) and
    /// MFRSEA (303). Each subtype belongs to exactly one miscellaneous data type.
    /// Sending a different miscDataTypeCode under an existing subtype was tried and
    /// rejected, so a subtype is never reused here.
    /// </para>
    /// <para>
    /// LCRB has more endorsement types than OneStop has confirmed codes for, so
    /// <see cref="Resolve"/> returns null for anything not listed and the caller skips
    /// it rather than sending a guess. Two miscellaneous data types are known but not
    /// sendable, because neither has a confirmed documentSubType of its own:
    /// MFRLNG (Manufacturer Lounge Endorsement) and MFREND (Manufacturer Endorsement).
    /// Once LDB and the HUB confirm a subtype for either, add the constant in
    /// OneStopUtils and a row here.
    /// </para>
    /// </summary>
    public static class EndorsementMiscDataMap
    {
        // Endorsement names as they appear in adoxio_endorsement.adoxio_name.
        public const string LOUNGE_AREA_ENDORSEMENT = "Lounge Area Endorsement";
        public const string ON_SITE_STORE_ENDORSEMENT = "On-Site Store Endorsement";
        public const string SPECIAL_EVENT_AREA_ENDORSEMENT = "Special Event Area Endorsement";

        // Manufacturer sub-category that takes the winery-specific lounge code.
        public const string LICENCE_SUB_CATEGORY_WINERY = "Winery";

        /// <summary>
        /// Endorsement name -> codes, for endorsements whose code does not depend on
        /// the licence sub-category.
        /// </summary>
        private static readonly Dictionary<string, EndorsementMiscDataType> Map =
            new Dictionary<string, EndorsementMiscDataType>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    ON_SITE_STORE_ENDORSEMENT,
                    new EndorsementMiscDataType
                    {
                        MiscDataTypeCode = OneStopUtils.MISC_DATA_TYPE_MANUFACTURER_ONSITE,
                        DocumentSubType = OneStopUtils.DOCUMENT_SUBTYPE_MISCDATA_ONSITE
                    }
                },
                {
                    SPECIAL_EVENT_AREA_ENDORSEMENT,
                    new EndorsementMiscDataType
                    {
                        MiscDataTypeCode = OneStopUtils.MISC_DATA_TYPE_MANUFACTURER_SEA,
                        DocumentSubType = OneStopUtils.DOCUMENT_SUBTYPE_MISCDATA_SEA
                    }
                }
            };

        /// <summary>
        /// Resolves the codes for an endorsement, or null when the endorsement has no
        /// confirmed OneStop code and must not be sent.
        /// </summary>
        /// <param name="endorsementName">adoxio_endorsement.adoxio_name</param>
        /// <param name="licenceSubCategory">Manufacturer sub-category of the licence, may be null.</param>
        public static EndorsementMiscDataType Resolve(string endorsementName, string licenceSubCategory)
        {
            if (string.IsNullOrWhiteSpace(endorsementName))
                return null;

            var name = endorsementName.Trim();

            if (string.Equals(name, LOUNGE_AREA_ENDORSEMENT, StringComparison.OrdinalIgnoreCase))
            {
                // Only the winery lounge code is confirmed. A lounge on a brewery,
                // distillery or co-packer licence would need MFRLNG, which has no
                // confirmed subtype of its own, so it is not sent.
                var isWinery = string.Equals(licenceSubCategory?.Trim(),
                    LICENCE_SUB_CATEGORY_WINERY, StringComparison.OrdinalIgnoreCase);

                return isWinery
                    ? new EndorsementMiscDataType
                    {
                        MiscDataTypeCode = OneStopUtils.MISC_DATA_TYPE_WINERY_LOUNGE,
                        DocumentSubType = OneStopUtils.DOCUMENT_SUBTYPE_MISCDATA_WINERY_LOUNGE
                    }
                    : null;
            }

            return Map.TryGetValue(name, out var codes) ? codes : null;
        }

        /// <summary>
        /// True when the endorsement has a confirmed OneStop code.
        /// </summary>
        public static bool IsSupported(string endorsementName, string licenceSubCategory)
            => Resolve(endorsementName, licenceSubCategory) != null;
    }
}
