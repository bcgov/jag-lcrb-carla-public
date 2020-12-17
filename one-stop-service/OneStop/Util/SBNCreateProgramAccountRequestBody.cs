/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestBody {
    
    private string businessRegistrationNumberField;
    
    private string businessProgramIdentifierField;
    
    private string sBNProgramTypeCodeField;
    
    private SBNCreateProgramAccountRequestBodyBusinessCore businessCoreField;
    
    private SBNCreateProgramAccountRequestBodyProgramAccountStatus programAccountStatusField;
    
    private string legalNameField;
    
    private SBNCreateProgramAccountRequestBodyOperatingName operatingNameField;
    
    private string assumedNameField;
    
    private SBNCreateProgramAccountRequestBodyBusinessAddress businessAddressField;
    
    private string addressSameIndicatorField;
    
    private SBNCreateProgramAccountRequestBodyMailingAddress mailingAddressField;
    
    private SBNCreateProgramAccountRequestBodyCorporationCertification corporationCertificationField;
    
    private SBNCreateProgramAccountRequestBodyOwner[] ownerField;
    
    private SBNCreateProgramAccountRequestBodyBusinessActivityDeclaration businessActivityDeclarationField;
    
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
    public SBNCreateProgramAccountRequestBodyBusinessCore businessCore {
        get {
            return this.businessCoreField;
        }
        set {
            this.businessCoreField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountRequestBodyProgramAccountStatus programAccountStatus {
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
    public SBNCreateProgramAccountRequestBodyOperatingName operatingName {
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
    public SBNCreateProgramAccountRequestBodyBusinessAddress businessAddress {
        get {
            return this.businessAddressField;
        }
        set {
            this.businessAddressField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressSameIndicator {
        get {
            return this.addressSameIndicatorField;
        }
        set {
            this.addressSameIndicatorField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountRequestBodyMailingAddress mailingAddress {
        get {
            return this.mailingAddressField;
        }
        set {
            this.mailingAddressField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountRequestBodyCorporationCertification corporationCertification {
        get {
            return this.corporationCertificationField;
        }
        set {
            this.corporationCertificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("owner", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountRequestBodyOwner[] owner {
        get {
            return this.ownerField;
        }
        set {
            this.ownerField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountRequestBodyBusinessActivityDeclaration businessActivityDeclaration {
        get {
            return this.businessActivityDeclarationField;
        }
        set {
            this.businessActivityDeclarationField = value;
        }
    }
}