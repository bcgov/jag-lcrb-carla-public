/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class SBNProgramAccountDetailsBroadcastHeaderCCRAHeader {
    
    private string userApplicationField;
    
    private string userRoleField;
    
    private SBNProgramAccountDetailsBroadcastHeaderCCRAHeaderUserCredentials userCredentialsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string userApplication {
        get {
            return this.userApplicationField;
        }
        set {
            this.userApplicationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string userRole {
        get {
            return this.userRoleField;
        }
        set {
            this.userRoleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public SBNProgramAccountDetailsBroadcastHeaderCCRAHeaderUserCredentials userCredentials {
        get {
            return this.userCredentialsField;
        }
        set {
            this.userCredentialsField = value;
        }
    }
}