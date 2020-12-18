/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class ValidationErrorType {
    
    private string fieldNameField;
    
    private string errorMessageNumberField;
    
    private string errorMessageTextField;
    
    /// <remarks/>
    public string fieldName {
        get {
            return this.fieldNameField;
        }
        set {
            this.fieldNameField = value;
        }
    }
    
    /// <remarks/>
    public string errorMessageNumber {
        get {
            return this.errorMessageNumberField;
        }
        set {
            this.errorMessageNumberField = value;
        }
    }
    
    /// <remarks/>
    public string errorMessageText {
        get {
            return this.errorMessageTextField;
        }
        set {
            this.errorMessageTextField = value;
        }
    }
}