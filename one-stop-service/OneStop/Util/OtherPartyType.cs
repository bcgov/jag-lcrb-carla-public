/// <remarks/>
[System.Xml.Serialization.XmlIncludeAttribute(typeof(ContactType))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(OwnerType))]
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class OtherPartyType {
    
    private string lastNameField;
    
    private string givenNameField;
    
    private DomesticTelephoneType domesticTelephoneField;
    
    private InternationalTelephoneType internationalTelephoneField;
    
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
    public DomesticTelephoneType domesticTelephone {
        get {
            return this.domesticTelephoneField;
        }
        set {
            this.domesticTelephoneField = value;
        }
    }
    
    /// <remarks/>
    public InternationalTelephoneType internationalTelephone {
        get {
            return this.internationalTelephoneField;
        }
        set {
            this.internationalTelephoneField = value;
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