/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class BusinessActivityDeclarationType {
    
    private string businessActivityDescriptionField;
    
    private MainProductServiceType[] mainProductServiceField;
    
    /// <remarks/>
    public string businessActivityDescription {
        get {
            return this.businessActivityDescriptionField;
        }
        set {
            this.businessActivityDescriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("mainProductService")]
    public MainProductServiceType[] mainProductService {
        get {
            return this.mainProductServiceField;
        }
        set {
            this.mainProductServiceField = value;
        }
    }
}