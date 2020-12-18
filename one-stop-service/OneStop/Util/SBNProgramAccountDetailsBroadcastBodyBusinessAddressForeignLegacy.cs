/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy {
    
    private string addressDetailLine1Field;
    
    private string addressDetailLine2Field;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressDetailLine1 {
        get {
            return this.addressDetailLine1Field;
        }
        set {
            this.addressDetailLine1Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressDetailLine2 {
        get {
            return this.addressDetailLine2Field;
        }
        set {
            this.addressDetailLine2Field = value;
        }
    }
}