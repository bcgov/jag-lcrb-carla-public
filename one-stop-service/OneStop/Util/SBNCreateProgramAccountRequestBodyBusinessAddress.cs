/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestBodyBusinessAddress {
    
    private SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy foreignLegacyField;
    
    private SBNCreateProgramAccountRequestBodyBusinessAddressCanadianCivic canadianCivicField;
    
    private string municipalityField;
    
    private string provinceStateCodeField;
    
    private string postalCodeField;
    
    private string countryCodeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy foreignLegacy {
        get {
            return this.foreignLegacyField;
        }
        set {
            this.foreignLegacyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountRequestBodyBusinessAddressCanadianCivic canadianCivic {
        get {
            return this.canadianCivicField;
        }
        set {
            this.canadianCivicField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string municipality {
        get {
            return this.municipalityField;
        }
        set {
            this.municipalityField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string provinceStateCode {
        get {
            return this.provinceStateCodeField;
        }
        set {
            this.provinceStateCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string postalCode {
        get {
            return this.postalCodeField;
        }
        set {
            this.postalCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string countryCode {
        get {
            return this.countryCodeField;
        }
        set {
            this.countryCodeField = value;
        }
    }
}