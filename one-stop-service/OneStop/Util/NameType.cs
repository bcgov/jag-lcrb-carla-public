/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class NameType {
    
    private object clientNameTypeCodeField;
    
    private string nameField;
    
    private string operatingNamesequenceNumberField;
    
    private string updateReasonCodeField;
    
    /// <remarks/>
    public object clientNameTypeCode {
        get {
            return this.clientNameTypeCodeField;
        }
        set {
            this.clientNameTypeCodeField = value;
        }
    }
    
    /// <remarks/>
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
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
    
    /// <remarks/>
    public string updateReasonCode {
        get {
            return this.updateReasonCodeField;
        }
        set {
            this.updateReasonCodeField = value;
        }
    }
}