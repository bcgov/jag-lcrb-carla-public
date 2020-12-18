/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ProgramAccountStatusType {
    
    private string programAccountStatusCodeField;
    
    private string programAccountReasonCodeField;
    
    private string bCPartnerReasonCodeField;
    
    private string bCPartnerReasonTextField;
    
    private System.DateTime effectiveDateField;
    
    /// <remarks/>
    public string programAccountStatusCode {
        get {
            return this.programAccountStatusCodeField;
        }
        set {
            this.programAccountStatusCodeField = value;
        }
    }
    
    /// <remarks/>
    public string programAccountReasonCode {
        get {
            return this.programAccountReasonCodeField;
        }
        set {
            this.programAccountReasonCodeField = value;
        }
    }
    
    /// <remarks/>
    public string BCPartnerReasonCode {
        get {
            return this.bCPartnerReasonCodeField;
        }
        set {
            this.bCPartnerReasonCodeField = value;
        }
    }
    
    /// <remarks/>
    public string BCPartnerReasonText {
        get {
            return this.bCPartnerReasonTextField;
        }
        set {
            this.bCPartnerReasonTextField = value;
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