/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class CorporationCertificationType {
    
    private System.DateTime incorporationDateField;
    
    private string incorporationCertificateIdentifierField;
    
    private string jurisdictionCodeField;
    
    private string jurisdictionProvinceCodeField;
    
    private string jurisdictionCountryCodeField;
    
    private string jurisdictionStateCodeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
    public System.DateTime incorporationDate {
        get {
            return this.incorporationDateField;
        }
        set {
            this.incorporationDateField = value;
        }
    }
    
    /// <remarks/>
    public string incorporationCertificateIdentifier {
        get {
            return this.incorporationCertificateIdentifierField;
        }
        set {
            this.incorporationCertificateIdentifierField = value;
        }
    }
    
    /// <remarks/>
    public string jurisdictionCode {
        get {
            return this.jurisdictionCodeField;
        }
        set {
            this.jurisdictionCodeField = value;
        }
    }
    
    /// <remarks/>
    public string jurisdictionProvinceCode {
        get {
            return this.jurisdictionProvinceCodeField;
        }
        set {
            this.jurisdictionProvinceCodeField = value;
        }
    }
    
    /// <remarks/>
    public string jurisdictionCountryCode {
        get {
            return this.jurisdictionCountryCodeField;
        }
        set {
            this.jurisdictionCountryCodeField = value;
        }
    }
    
    /// <remarks/>
    public string jurisdictionStateCode {
        get {
            return this.jurisdictionStateCodeField;
        }
        set {
            this.jurisdictionStateCodeField = value;
        }
    }
}