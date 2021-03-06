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
    public partial class SBNChangeStatus
    {

        private SBNChangeStatusHeader headerField;

        private SBNChangeStatusBody bodyField;

        /// <remarks/>
        public SBNChangeStatusHeader header
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
        public SBNChangeStatusBody body
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
    public partial class SBNChangeStatusHeader
    {

        private string requestModeField;

        private string documentSubTypeField;

        private string senderIDField;

        private string receiverIDField;

        private string partnerNoteField;

        private SBNChangeStatusHeaderCCRAHeader cCRAHeaderField;

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
        public SBNChangeStatusHeaderCCRAHeader CCRAHeader
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
    public partial class SBNChangeStatusHeaderCCRAHeader
    {

        private string userApplicationField;

        private string userRoleField;

        private SBNChangeStatusHeaderCCRAHeaderUserCredentials userCredentialsField;

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
        public SBNChangeStatusHeaderCCRAHeaderUserCredentials userCredentials
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
    public partial class SBNChangeStatusHeaderCCRAHeaderUserCredentials
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
    public partial class SBNChangeStatusBody
    {

        private SBNChangeStatusBodyStatusData statusDataField;

        private string partnerInfo1Field;

        private System.DateTime partnerInfo2Field;

        /// <remarks/>
        public SBNChangeStatusBodyStatusData statusData
        {
            get
            {
                return this.statusDataField;
            }
            set
            {
                this.statusDataField = value;
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
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime partnerInfo2
        {
            get
            {
                return this.partnerInfo2Field;
            }
            set
            {
                this.partnerInfo2Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeStatusBodyStatusData
    {

        private string businessRegistrationNumberField;
        
        private string businessProgramIdentifierField;

        private string businessProgramAccountReferenceNumberField;

        private SBNChangeStatusBodyStatusDataProgramAccountStatus programAccountStatusField;

        private string timeStampField;

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
        public SBNChangeStatusBodyStatusDataProgramAccountStatus programAccountStatus
        {
            get
            {
                return this.programAccountStatusField;
            }
            set
            {
                this.programAccountStatusField = value;
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeStatusBodyStatusDataProgramAccountStatus
    {

        private string programAccountStatusCodeField;


        private string programAccountReasonCodeField;

        private System.DateTime effectiveDateField;

        /// <remarks/>
        public string programAccountStatusCode
        {
            get
            {
                return this.programAccountStatusCodeField;
            }
            set
            {
                this.programAccountStatusCodeField = value;
            }
        }

        /// <remarks/>
        public string programAccountReasonCode
        {
            get
            {
                return this.programAccountReasonCodeField;
            }
            set
            {
                this.programAccountReasonCodeField = value;
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


}
