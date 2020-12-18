/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountResponseHeader {
    
    private string requestModeField;
    
    private string documentSubTypeField;
    
    private string senderIDField;
    
    private string receiverIDField;
    
    private string transactionIDField;
    
    private string transactionDateTimeField;
    
    private string partnerNoteField;
    
    private SBNCreateProgramAccountResponseHeaderCCRAHeader cCRAHeaderField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string requestMode {
        get {
            return this.requestModeField;
        }
        set {
            this.requestModeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string documentSubType {
        get {
            return this.documentSubTypeField;
        }
        set {
            this.documentSubTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string senderID {
        get {
            return this.senderIDField;
        }
        set {
            this.senderIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string receiverID {
        get {
            return this.receiverIDField;
        }
        set {
            this.receiverIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string transactionID {
        get {
            return this.transactionIDField;
        }
        set {
            this.transactionIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string transactionDateTime {
        get {
            return this.transactionDateTimeField;
        }
        set {
            this.transactionDateTimeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string partnerNote {
        get {
            return this.partnerNoteField;
        }
        set {
            this.partnerNoteField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountResponseHeaderCCRAHeader CCRAHeader {
        get {
            return this.cCRAHeaderField;
        }
        set {
            this.cCRAHeaderField = value;
        }
    }
}