/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class BatchEnvelopeHeaderType {
    
    private string senderIDField;
    
    private string groupIDField;
    
    private string sBNProgramTypeCodeField;
    
    private string batchCountField;
    
    /// <remarks/>
    public string senderID {
        get {
            return this.senderIDField;
        }
        set {
            this.senderIDField = value;
        }
    }
    
    /// <remarks/>
    public string groupID {
        get {
            return this.groupIDField;
        }
        set {
            this.groupIDField = value;
        }
    }
    
    /// <remarks/>
    public string SBNProgramTypeCode {
        get {
            return this.sBNProgramTypeCodeField;
        }
        set {
            this.sBNProgramTypeCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string batchCount {
        get {
            return this.batchCountField;
        }
        set {
            this.batchCountField = value;
        }
    }
}