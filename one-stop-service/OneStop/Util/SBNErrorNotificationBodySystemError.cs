/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNErrorNotificationBodySystemError {
    
    private string errorMessageNumberField;
    
    private string errorMessageTextField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string errorMessageNumber {
        get {
            return this.errorMessageNumberField;
        }
        set {
            this.errorMessageNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string errorMessageText {
        get {
            return this.errorMessageTextField;
        }
        set {
            this.errorMessageTextField = value;
        }
    }
}