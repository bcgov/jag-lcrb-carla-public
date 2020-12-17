/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestBodyBusinessCore {
    
    private string programAccountTypeCodeField;
    
    private string crossReferenceProgramNumberField;
    
    private string businessTypeCodeField;
    
    private string businessSubTypeCodeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string programAccountTypeCode {
        get {
            return this.programAccountTypeCodeField;
        }
        set {
            this.programAccountTypeCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string crossReferenceProgramNumber {
        get {
            return this.crossReferenceProgramNumberField;
        }
        set {
            this.crossReferenceProgramNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string businessTypeCode {
        get {
            return this.businessTypeCodeField;
        }
        set {
            this.businessTypeCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string businessSubTypeCode {
        get {
            return this.businessSubTypeCodeField;
        }
        set {
            this.businessSubTypeCodeField = value;
        }
    }
}