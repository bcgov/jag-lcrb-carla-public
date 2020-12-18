/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNProgramAccountDetailsBroadcastBody {
    
    private string businessRegistrationNumberField;
    
    private string businessProgramIdentifierField;
    
    private string businessProgramAccountReferenceNumberField;
    
    private string sBNProgramTypeCodeField;
    
    private SBNProgramAccountDetailsBroadcastBodyBusinessCore businessCoreField;
    
    private SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus programAccountStatusField;
    
    private string legalNameField;
    
    private SBNProgramAccountDetailsBroadcastBodyOperatingName operatingNameField;
    
    private string assumedNameField;
    
    private SBNProgramAccountDetailsBroadcastBodyBusinessAddress businessAddressField;
    
    private SameIndicatorType addressSameIndicatorField;
    
    private bool addressSameIndicatorFieldSpecified;
    
    private SBNProgramAccountDetailsBroadcastBodyMailingAddress mailingAddressField;
    
    private SBNProgramAccountDetailsBroadcastBodyCorporationCertification corporationCertificationField;
    
    private SBNProgramAccountDetailsBroadcastBodyOwner[] ownerField;
    
    private SBNProgramAccountDetailsBroadcastBodyBusinessActivityDeclaration businessActivityDeclarationField;
    
    private string partnerInfo1Field;
    
    private string partnerInfo2Field;
    
    private string partnerInfo3Field;
    
    private string expiryDateField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
    public string businessRegistrationNumber {
        get {
            return this.businessRegistrationNumberField;
        }
        set {
            this.businessRegistrationNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string businessProgramIdentifier {
        get {
            return this.businessProgramIdentifierField;
        }
        set {
            this.businessProgramIdentifierField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
    public string businessProgramAccountReferenceNumber {
        get {
            return this.businessProgramAccountReferenceNumberField;
        }
        set {
            this.businessProgramAccountReferenceNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SBNProgramTypeCode {
        get {
            return this.sBNProgramTypeCodeField;
        }
        set {
            this.sBNProgramTypeCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastBodyBusinessCore businessCore {
        get {
            return this.businessCoreField;
        }
        set {
            this.businessCoreField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus programAccountStatus {
        get {
            return this.programAccountStatusField;
        }
        set {
            this.programAccountStatusField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string legalName {
        get {
            return this.legalNameField;
        }
        set {
            this.legalNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastBodyOperatingName operatingName {
        get {
            return this.operatingNameField;
        }
        set {
            this.operatingNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string assumedName {
        get {
            return this.assumedNameField;
        }
        set {
            this.assumedNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastBodyBusinessAddress businessAddress {
        get {
            return this.businessAddressField;
        }
        set {
            this.businessAddressField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SameIndicatorType addressSameIndicator {
        get {
            return this.addressSameIndicatorField;
        }
        set {
            this.addressSameIndicatorField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool addressSameIndicatorSpecified {
        get {
            return this.addressSameIndicatorFieldSpecified;
        }
        set {
            this.addressSameIndicatorFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastBodyMailingAddress mailingAddress {
        get {
            return this.mailingAddressField;
        }
        set {
            this.mailingAddressField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastBodyCorporationCertification corporationCertification {
        get {
            return this.corporationCertificationField;
        }
        set {
            this.corporationCertificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("owner", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastBodyOwner[] owner {
        get {
            return this.ownerField;
        }
        set {
            this.ownerField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastBodyBusinessActivityDeclaration businessActivityDeclaration {
        get {
            return this.businessActivityDeclarationField;
        }
        set {
            this.businessActivityDeclarationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string partnerInfo1 {
        get {
            return this.partnerInfo1Field;
        }
        set {
            this.partnerInfo1Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string partnerInfo2 {
        get {
            return this.partnerInfo2Field;
        }
        set {
            this.partnerInfo2Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string partnerInfo3 {
        get {
            return this.partnerInfo3Field;
        }
        set {
            this.partnerInfo3Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string expiryDate {
        get {
            return this.expiryDateField;
        }
        set {
            this.expiryDateField = value;
        }
    }
}