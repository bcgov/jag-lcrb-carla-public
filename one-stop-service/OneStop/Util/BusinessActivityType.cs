/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class BusinessActivityType {
    
    private string businessActivityDescriptionField;
    
    private MainProductServiceType[] mainProductServiceField;
    
    private string businessActivityCertificationCodeField;
    
    private System.DateTime effectiveDateField;
    
    private bool effectiveDateFieldSpecified;
    
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
    
    /// <remarks/>
    public string businessActivityCertificationCode {
        get {
            return this.businessActivityCertificationCodeField;
        }
        set {
            this.businessActivityCertificationCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
    public System.DateTime effectiveDate {
        get {
            return this.effectiveDateField;
        }
        set {
            this.effectiveDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool effectiveDateSpecified {
        get {
            return this.effectiveDateFieldSpecified;
        }
        set {
            this.effectiveDateFieldSpecified = value;
        }
    }
}