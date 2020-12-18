/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class OperatingNameType {
    
    private string operatingNameField;
    
    private string operatingNamesequenceNumberField;
    
    /// <remarks/>
    public string operatingName {
        get {
            return this.operatingNameField;
        }
        set {
            this.operatingNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string operatingNamesequenceNumber {
        get {
            return this.operatingNamesequenceNumberField;
        }
        set {
            this.operatingNamesequenceNumberField = value;
        }
    }
}