/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestBodyBusinessActivityDeclaration {
    
    private string businessActivityDescriptionField;
    
    private SBNCreateProgramAccountRequestBodyBusinessActivityDeclarationMainProductService[] mainProductServiceField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string businessActivityDescription {
        get {
            return this.businessActivityDescriptionField;
        }
        set {
            this.businessActivityDescriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("mainProductService", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNCreateProgramAccountRequestBodyBusinessActivityDeclarationMainProductService[] mainProductService {
        get {
            return this.mainProductServiceField;
        }
        set {
            this.mainProductServiceField = value;
        }
    }
}