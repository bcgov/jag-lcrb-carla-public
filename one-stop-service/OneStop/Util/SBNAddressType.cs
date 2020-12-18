/// <remarks/>
[System.Xml.Serialization.XmlIncludeAttribute(typeof(SBNMailingAddressType))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(SBNMailingAddressTypeDate))]
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class SBNAddressType {
    
    private object itemField;
    
    private string municipalityField;
    
    private string provinceStateCodeField;
    
    private string postalCodeField;
    
    private string countryCodeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("canadianCivic", typeof(SBNAddressTypeCanadianCivic))]
    [System.Xml.Serialization.XmlElementAttribute("foreignLegacy", typeof(SBNAddressTypeForeignLegacy))]
    public object Item {
        get {
            return this.itemField;
        }
        set {
            this.itemField = value;
        }
    }
    
    /// <remarks/>
    public string municipality {
        get {
            return this.municipalityField;
        }
        set {
            this.municipalityField = value;
        }
    }
    
    /// <remarks/>
    public string provinceStateCode {
        get {
            return this.provinceStateCodeField;
        }
        set {
            this.provinceStateCodeField = value;
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
    public string countryCode {
        get {
            return this.countryCodeField;
        }
        set {
            this.countryCodeField = value;
        }
    }
}