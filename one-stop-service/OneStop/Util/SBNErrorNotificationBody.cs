/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNErrorNotificationBody {
    
    private string businessRegistrationNumberField;
    
    private string businessProgramIdentifierField;
    
    private string businessProgramAccountReferenceNumberField;
    
    private SBNErrorNotificationBodyValidationErrors[] validationErrorsField;
    
    private SBNErrorNotificationBodySystemError systemErrorField;
    
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
    [System.Xml.Serialization.XmlElementAttribute("validationErrors", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNErrorNotificationBodyValidationErrors[] validationErrors {
        get {
            return this.validationErrorsField;
        }
        set {
            this.validationErrorsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNErrorNotificationBodySystemError systemError {
        get {
            return this.systemErrorField;
        }
        set {
            this.systemErrorField = value;
        }
    }
}