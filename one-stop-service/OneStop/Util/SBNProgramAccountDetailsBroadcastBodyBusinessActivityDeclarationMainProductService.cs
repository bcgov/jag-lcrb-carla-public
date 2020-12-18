/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNProgramAccountDetailsBroadcastBodyBusinessActivityDeclarationMainProductService {
    
    private string mainProductServiceDescriptionField;
    
    private string revenuePerProductPercentField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string mainProductServiceDescription {
        get {
            return this.mainProductServiceDescriptionField;
        }
        set {
            this.mainProductServiceDescriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
    public string revenuePerProductPercent {
        get {
            return this.revenuePerProductPercentField;
        }
        set {
            this.revenuePerProductPercentField = value;
        }
    }
}