/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class CCRAUserCredentialsType {
    
    private string businessRegistrationNumberField;
    
    private string legalNameField;
    
    private string postalCodeField;
    
    private string lastNameField;
    
    private CCRAUserCredentialsTypeCorporationInformation corporationInformationField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string businessRegistrationNumber {
        get {
            return this.businessRegistrationNumberField;
        }
        set {
            this.businessRegistrationNumberField = value;
        }
    }
    
    /// <remarks/>
    public string legalName {
        get {
            return this.legalNameField;
        }
        set {
            this.legalNameField = value;
        }
    }
    
    /// <remarks/>
    public string postalCode {
        get {
            return this.postalCodeField;
        }
        set {
            this.postalCodeField = value;
        }
    }
    
    /// <remarks/>
    public string lastName {
        get {
            return this.lastNameField;
        }
        set {
            this.lastNameField = value;
        }
    }
    
    /// <remarks/>
    public CCRAUserCredentialsTypeCorporationInformation corporationInformation {
        get {
            return this.corporationInformationField;
        }
        set {
            this.corporationInformationField = value;
        }
    }
}