/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestBodyOwnerOwnerIndividual {
    
    private string lastNameField;
    
    private string givenNameField;
    
    private SBNCreateProgramAccountRequestBodyOwnerOwnerIndividualDomesticTelephone[] domesticTelephoneField;
    
    private SBNCreateProgramAccountRequestBodyOwnerOwnerIndividualInternationalTelephone[] internationalTelephoneField;
    
    private string positionTitleField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string lastName {
        get {
            return this.lastNameField;
        }
        set {
            this.lastNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string givenName {
        get {
            return this.givenNameField;
        }
        set {
            this.givenNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("domesticTelephone", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public SBNCreateProgramAccountRequestBodyOwnerOwnerIndividualDomesticTelephone[] domesticTelephone {
        get {
            return this.domesticTelephoneField;
        }
        set {
            this.domesticTelephoneField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("internationalTelephone", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public SBNCreateProgramAccountRequestBodyOwnerOwnerIndividualInternationalTelephone[] internationalTelephone {
        get {
            return this.internationalTelephoneField;
        }
        set {
            this.internationalTelephoneField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string positionTitle {
        get {
            return this.positionTitleField;
        }
        set {
            this.positionTitleField = value;
        }
    }
}