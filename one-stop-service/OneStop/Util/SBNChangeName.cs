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
    public partial class SBNChangeName
    {

        private SBNChangeNameHeader headerField;

        private SBNChangeNameBody bodyField;

        /// <remarks/>
        public SBNChangeNameHeader header
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
        public SBNChangeNameBody body
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
    public partial class SBNChangeNameHeader
    {

        private string requestModeField;

        private string documentSubTypeField;

        private string senderIDField;

        private string receiverIDField;

        private string partnerNoteField;

        private SBNChangeNameHeaderCCRAHeader cCRAHeaderField;

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
        public SBNChangeNameHeaderCCRAHeader CCRAHeader
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
    public partial class SBNChangeNameHeaderCCRAHeader
    {

        private string userApplicationField;

        private string userRoleField;

        private SBNChangeNameHeaderCCRAHeaderUserCredentials userCredentialsField;

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
        public SBNChangeNameHeaderCCRAHeaderUserCredentials userCredentials
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
    public partial class SBNChangeNameHeaderCCRAHeaderUserCredentials
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
    public partial class SBNChangeNameBody
    {

        private string businessRegistrationNumberField;

        private string businessProgramIdentifierField;

        private string businessProgramAccountReferenceNumberField;

        private SBNChangeNameBodyName nameField;

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
        public SBNChangeNameBodyName name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
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
    public partial class SBNChangeNameBodyName
    {

        private string clientNameTypeCodeField;

        private string nameField;

        private byte operatingNamesequenceNumberField;

        private string updateReasonCodeField;

        private string[] textField;

        /// <remarks/>
        public string clientNameTypeCode
        {
            get
            {
                return this.clientNameTypeCodeField;
            }
            set
            {
                this.clientNameTypeCodeField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public byte operatingNamesequenceNumber
        {
            get
            {
                return this.operatingNamesequenceNumberField;
            }
            set
            {
                this.operatingNamesequenceNumberField = value;
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


}
