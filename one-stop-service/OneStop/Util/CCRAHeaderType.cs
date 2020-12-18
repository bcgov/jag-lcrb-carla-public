/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class CCRAHeaderType {
    
    private string userApplicationField;
    
    private string userRoleField;
    
    private CCRAUserCredentialsType userCredentialsField;
    
    /// <remarks/>
    public string userApplication {
        get {
            return this.userApplicationField;
        }
        set {
            this.userApplicationField = value;
        }
    }
    
    /// <remarks/>
    public string userRole {
        get {
            return this.userRoleField;
        }
        set {
            this.userRoleField = value;
        }
    }
    
    /// <remarks/>
    public CCRAUserCredentialsType userCredentials {
        get {
            return this.userCredentialsField;
        }
        set {
            this.userCredentialsField = value;
        }
    }
}