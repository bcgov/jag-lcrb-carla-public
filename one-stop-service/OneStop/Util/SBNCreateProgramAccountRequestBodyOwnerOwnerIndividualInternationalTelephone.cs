/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestBodyOwnerOwnerIndividualInternationalTelephone {
    
    private string deviceLocationCdField;
    
    private string deviceTypeCdField;
    
    private string worldWideDialingPhoneCountryCodeField;
    
    private string worldWideDialingPhoneCityCodeField;
    
    private string worldWideDialingPhoneLocalNumberField;
    
    private string worldWideDialingExtensionNumberField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string deviceLocationCd {
        get {
            return this.deviceLocationCdField;
        }
        set {
            this.deviceLocationCdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string deviceTypeCd {
        get {
            return this.deviceTypeCdField;
        }
        set {
            this.deviceTypeCdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string worldWideDialingPhoneCountryCode {
        get {
            return this.worldWideDialingPhoneCountryCodeField;
        }
        set {
            this.worldWideDialingPhoneCountryCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string worldWideDialingPhoneCityCode {
        get {
            return this.worldWideDialingPhoneCityCodeField;
        }
        set {
            this.worldWideDialingPhoneCityCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
    public string worldWideDialingPhoneLocalNumber {
        get {
            return this.worldWideDialingPhoneLocalNumberField;
        }
        set {
            this.worldWideDialingPhoneLocalNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
    public string worldWideDialingExtensionNumber {
        get {
            return this.worldWideDialingExtensionNumberField;
        }
        set {
            this.worldWideDialingExtensionNumberField = value;
        }
    }
}