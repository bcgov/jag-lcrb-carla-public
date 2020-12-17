/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestBodyProgramAccountStatus {
    
    private string programAccountStatusCodeField;
    
    private string programAccountReasonCodeField;
    
    private string bCPartnerReasonCodeField;
    
    private string bCPartnerReasonTextField;
    
    private System.DateTime effectiveDateField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string programAccountStatusCode {
        get {
            return this.programAccountStatusCodeField;
        }
        set {
            this.programAccountStatusCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string programAccountReasonCode {
        get {
            return this.programAccountReasonCodeField;
        }
        set {
            this.programAccountReasonCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string BCPartnerReasonCode {
        get {
            return this.bCPartnerReasonCodeField;
        }
        set {
            this.bCPartnerReasonCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string BCPartnerReasonText {
        get {
            return this.bCPartnerReasonTextField;
        }
        set {
            this.bCPartnerReasonTextField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="date")]
    public System.DateTime effectiveDate {
        get {
            return this.effectiveDateField;
        }
        set {
            this.effectiveDateField = value;
        }
    }
}