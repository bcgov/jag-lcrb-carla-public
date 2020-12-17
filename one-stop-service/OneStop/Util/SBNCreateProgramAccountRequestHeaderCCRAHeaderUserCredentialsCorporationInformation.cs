/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentialsCorporationInformation {
    
    private string incorporationCertificateIdentifierField;
    
    private string jurisdictionCodeField;
    
    private string jurisdictionProvinceCodeField;
    
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
}