/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class BusinessStatusType {
    
    private string bNStatusCodeField;
    
    private string bNStatusReasonCodeField;
    
    private System.DateTime effectiveDateField;
    
    /// <remarks/>
    public string BNStatusCode {
        get {
            return this.bNStatusCodeField;
        }
        set {
            this.bNStatusCodeField = value;
        }
    }
    
    /// <remarks/>
    public string BNStatusReasonCode {
        get {
            return this.bNStatusReasonCodeField;
        }
        set {
            this.bNStatusReasonCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
    public System.DateTime effectiveDate {
        get {
            return this.effectiveDateField;
        }
        set {
            this.effectiveDateField = value;
        }
    }
}