/// <remarks/>
[System.Xml.Serialization.XmlIncludeAttribute(typeof(CRAContactType))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(CRAOwnerType))]
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class CRAOtherPartyType {
    
    private string lastNameField;
    
    private string givenNameField;
    
    private CRATelephoneDeviceType[] telephoneField;
    
    private string positionTitleField;
    
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
    public string givenName {
        get {
            return this.givenNameField;
        }
        set {
            this.givenNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("telephone")]
    public CRATelephoneDeviceType[] telephone {
        get {
            return this.telephoneField;
        }
        set {
            this.telephoneField = value;
        }
    }
    
    /// <remarks/>
    public string positionTitle {
        get {
            return this.positionTitleField;
        }
        set {
            this.positionTitleField = value;
        }
    }
}