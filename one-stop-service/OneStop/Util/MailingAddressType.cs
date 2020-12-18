/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class MailingAddressType : AddressType {
    
    private string returnMailIndicatorField;
    
    private string careOfLineField;
    
    /// <remarks/>
    public string returnMailIndicator {
        get {
            return this.returnMailIndicatorField;
        }
        set {
            this.returnMailIndicatorField = value;
        }
    }
    
    /// <remarks/>
    public string careOfLine {
        get {
            return this.careOfLineField;
        }
        set {
            this.careOfLineField = value;
        }
    }
}