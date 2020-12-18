/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class MainProductServiceType {
    
    private string mainProductServiceDescriptionField;
    
    private string revenuePerProductPercentField;
    
    /// <remarks/>
    public string mainProductServiceDescription {
        get {
            return this.mainProductServiceDescriptionField;
        }
        set {
            this.mainProductServiceDescriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string revenuePerProductPercent {
        get {
            return this.revenuePerProductPercentField;
        }
        set {
            this.revenuePerProductPercentField = value;
        }
    }
}