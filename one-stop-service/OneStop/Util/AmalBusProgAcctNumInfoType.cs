/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class AmalBusProgAcctNumInfoType {
    
    private string businessRegistrationNumberField;
    
    private string businessProgramIdentifierField;
    
    private string businessProgramAccountReferenceNumberField;
    
    private string incorporationCertificateIdentifierField;
    
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
    public string businessProgramIdentifier {
        get {
            return this.businessProgramIdentifierField;
        }
        set {
            this.businessProgramIdentifierField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string businessProgramAccountReferenceNumber {
        get {
            return this.businessProgramAccountReferenceNumberField;
        }
        set {
            this.businessProgramAccountReferenceNumberField = value;
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
}