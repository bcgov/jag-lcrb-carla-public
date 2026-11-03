using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Jag.Lcrb.OneStopService.OneStop.Util
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("SBNChangeMiscData", Namespace = "", IsNullable = false)]
    public partial class SBNChangeMiscData
    {

        private SBNChangeMiscDataHeader headerField;

        private SBNChangeMiscDataBody bodyField;

        /// <remarks/>
        public SBNChangeMiscDataHeader header
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
        public SBNChangeMiscDataBody body
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
    /// <para>
    /// Note: the Miscellaneous Data Update document omits the CCRAHeader. Per the Messaging
    /// Technical Specification Guide v2, "CCRA Header - Should be omitted" for this document,
    /// and none of the LDB-accepted sample messages carry one.
    /// </para>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeMiscDataHeader
    {

        private string requestModeField;

        private string documentSubTypeField;

        private string senderIDField;

        private string receiverIDField;

        private string partnerNoteField;

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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SBNChangeMiscDataBody
    {

        private string businessRegistrationNumberField;

        private string businessProgramIdentifierField;

        private string businessProgramAccountReferenceNumberField;

        private string miscDataTypeCodeField;

        private string updateReasonCodeField;

        private string miscDataValueField;

        private string partnerInfo1Field;

        private string partnerInfo2Field;

        private string partnerInfo3Field;

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
        public string miscDataTypeCode
        {
            get
            {
                return this.miscDataTypeCodeField;
            }
            set
            {
                this.miscDataTypeCodeField = value;
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
        public string miscDataValue
        {
            get
            {
                return this.miscDataValueField;
            }
            set
            {
                this.miscDataValueField = value;
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
        public string partnerInfo2
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

        /// <remarks/>
        public string partnerInfo3
        {
            get
            {
                return this.partnerInfo3Field;
            }
            set
            {
                this.partnerInfo3Field = value;
            }
        }
    }
}
