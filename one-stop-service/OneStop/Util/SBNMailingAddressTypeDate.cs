/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class SBNMailingAddressTypeDate : SBNAddressType {
    
    private string effectiveDateField;
    
    private string careOfLineField;
    
    /// <remarks/>
    public string effectiveDate {
        get {
            return this.effectiveDateField;
        }
        set {
            this.effectiveDateField = value;
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