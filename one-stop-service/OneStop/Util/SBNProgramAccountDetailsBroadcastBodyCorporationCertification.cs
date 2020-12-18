/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNProgramAccountDetailsBroadcastBodyCorporationCertification {
    
    private System.DateTime incorporationDateField;
    
    private string incorporationCertificateIdentifierField;
    
    private string jurisdictionCodeField;
    
    private string jurisdictionProvinceCodeField;
    
    private string jurisdictionCountryCodeField;
    
    private string jurisdictionStateCodeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
    public System.DateTime incorporationDate {
        get {
            return this.incorporationDateField;
        }
        set {
            this.incorporationDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string incorporationCertificateIdentifier {
        get {
            return this.incorporationCertificateIdentifierField;
        }
        set {
            this.incorporationCertificateIdentifierField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string jurisdictionCode {
        get {
            return this.jurisdictionCodeField;
        }
        set {
            this.jurisdictionCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string jurisdictionProvinceCode {
        get {
            return this.jurisdictionProvinceCodeField;
        }
        set {
            this.jurisdictionProvinceCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string jurisdictionCountryCode {
        get {
            return this.jurisdictionCountryCodeField;
        }
        set {
            this.jurisdictionCountryCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string jurisdictionStateCode {
        get {
            return this.jurisdictionStateCodeField;
        }
        set {
            this.jurisdictionStateCodeField = value;
        }
    }
}