using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Jag.Lcrb.OneStopService.OneStop.Util
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class SBNChangeAddress
    {

        private SBNChangeAddressHeader headerField;

        private SBNChangeAddressBody bodyField;

        /// <remarks/>
        public SBNChangeAddressHeader header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        public SBNChangeAddressBody body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeAddressHeader
    {

        private string requestModeField;

        private string documentSubTypeField;

        private string senderIDField;

        private string receiverIDField;

        private string partnerNoteField;

        private SBNChangeAddressHeaderCCRAHeader cCRAHeaderField;

        /// <remarks/>
        public string requestMode
        {
            get
            {
                return this.requestModeField;
            }
            set
            {
                this.requestModeField = value;
            }
        }

        /// <remarks/>
        public string documentSubType
        {
            get
            {
                return this.documentSubTypeField;
            }
            set
            {
                this.documentSubTypeField = value;
            }
        }

        /// <remarks/>
        public string senderID
        {
            get
            {
                return this.senderIDField;
            }
            set
            {
                this.senderIDField = value;
            }
        }

        /// <remarks/>
        public string receiverID
        {
            get
            {
                return this.receiverIDField;
            }
            set
            {
                this.receiverIDField = value;
            }
        }

        /// <remarks/>
        public string partnerNote
        {
            get
            {
                return this.partnerNoteField;
            }
            set
            {
                this.partnerNoteField = value;
            }
        }

        /// <remarks/>
        public SBNChangeAddressHeaderCCRAHeader CCRAHeader
        {
            get
            {
                return this.cCRAHeaderField;
            }
            set
            {
                this.cCRAHeaderField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeAddressHeaderCCRAHeader
    {

        private string userApplicationField;

        private string userRoleField;

        private SBNChangeAddressHeaderCCRAHeaderUserCredentials userCredentialsField;

        /// <remarks/>
        public string userApplication
        {
            get
            {
                return this.userApplicationField;
            }
            set
            {
                this.userApplicationField = value;
            }
        }

        /// <remarks/>
        public string userRole
        {
            get
            {
                return this.userRoleField;
            }
            set
            {
                this.userRoleField = value;
            }
        }

        /// <remarks/>
        public SBNChangeAddressHeaderCCRAHeaderUserCredentials userCredentials
        {
            get
            {
                return this.userCredentialsField;
            }
            set
            {
                this.userCredentialsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeAddressHeaderCCRAHeaderUserCredentials
    {

        private string businessRegistrationNumberField;

        private string legalNameField;

        private string postalCodeField;

        /// <remarks/>
        public string businessRegistrationNumber
        {
            get
            {
                return this.businessRegistrationNumberField;
            }
            set
            {
                this.businessRegistrationNumberField = value;
            }
        }

        /// <remarks/>
        public string legalName
        {
            get
            {
                return this.legalNameField;
            }
            set
            {
                this.legalNameField = value;
            }
        }

        /// <remarks/>
        public string postalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeAddressBody
    {

        private string businessRegistrationNumberField;

        private string businessProgramIdentifierField;

        private string businessProgramAccountReferenceNumberField;

        private string addressTypeCodeField;

        private string updateReasonCodeField;

        private SBNChangeAddressBodyAddress addressField;

        private string timeStampField;

        private string partnerInfo1Field;

        private string[] textField;

        /// <remarks/>
        public string businessRegistrationNumber
        {
            get
            {
                return this.businessRegistrationNumberField;
            }
            set
            {
                this.businessRegistrationNumberField = value;
            }
        }

        /// <remarks/>
        public string businessProgramIdentifier
        {
            get
            {
                return this.businessProgramIdentifierField;
            }
            set
            {
                this.businessProgramIdentifierField = value;
            }
        }

        /// <remarks/>
        public string businessProgramAccountReferenceNumber
        {
            get
            {
                return this.businessProgramAccountReferenceNumberField;
            }
            set
            {
                this.businessProgramAccountReferenceNumberField = value;
            }
        }

        /// <remarks/>
        public string addressTypeCode
        {
            get
            {
                return this.addressTypeCodeField;
            }
            set
            {
                this.addressTypeCodeField = value;
            }
        }

        /// <remarks/>
        public string updateReasonCode
        {
            get
            {
                return this.updateReasonCodeField;
            }
            set
            {
                this.updateReasonCodeField = value;
            }
        }

        /// <remarks/>
        public SBNChangeAddressBodyAddress address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        public string timeStamp
        {
            get
            {
                return this.timeStampField;
            }
            set
            {
                this.timeStampField = value;
            }
        }

        /// <remarks/>
        public string partnerInfo1
        {
            get
            {
                return this.partnerInfo1Field;
            }
            set
            {
                this.partnerInfo1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeAddressBodyAddress
    {

        private SBNChangeAddressBodyAddressForeignLegacy foreignLegacyField;

        private string municipalityField;

        private string provinceStateCodeField;

        private string postalCodeField;

        private string countryCodeField;

        private System.DateTime effectiveDateField;

        /// <remarks/>
        public SBNChangeAddressBodyAddressForeignLegacy foreignLegacy
        {
            get
            {
                return this.foreignLegacyField;
            }
            set
            {
                this.foreignLegacyField = value;
            }
        }

        /// <remarks/>
        public string municipality
        {
            get
            {
                return this.municipalityField;
            }
            set
            {
                this.municipalityField = value;
            }
        }

        /// <remarks/>
        public string provinceStateCode
        {
            get
            {
                return this.provinceStateCodeField;
            }
            set
            {
                this.provinceStateCodeField = value;
            }
        }

        /// <remarks/>
        public string postalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public string countryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime effectiveDate
        {
            get
            {
                return this.effectiveDateField;
            }
            set
            {
                this.effectiveDateField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeAddressBodyAddressForeignLegacy
    {

        private string addressDetailLine1Field;

        /// <remarks/>
        public string addressDetailLine1
        {
            get
            {
                return this.addressDetailLine1Field;
            }
            set
            {
                this.addressDetailLine1Field = value;
            }
        }
    }


}
