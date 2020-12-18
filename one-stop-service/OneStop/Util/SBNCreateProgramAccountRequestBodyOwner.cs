/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNCreateProgramAccountRequestBodyOwner {
    
    private SBNCreateProgramAccountRequestBodyOwnerOwnerIndividual ownerIndividualField;
    
    private string ownerBNField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public SBNCreateProgramAccountRequestBodyOwnerOwnerIndividual ownerIndividual {
        get {
            return this.ownerIndividualField;
        }
        set {
            this.ownerIndividualField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
    public string ownerBN {
        get {
            return this.ownerBNField;
        }
        set {
            this.ownerBNField = value;
        }
    }
}