/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class CRATelephoneDeviceType {
    
    private string deviceLocationCdField;
    
    private string deviceTypeCdField;
    
    private string areaCodeField;
    
    private string telephoneNumberField;
    
    private string telephoneExtensionNumberField;
    
    /// <remarks/>
    public string deviceLocationCd {
        get {
            return this.deviceLocationCdField;
        }
        set {
            this.deviceLocationCdField = value;
        }
    }
    
    /// <remarks/>
    public string deviceTypeCd {
        get {
            return this.deviceTypeCdField;
        }
        set {
            this.deviceTypeCdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string areaCode {
        get {
            return this.areaCodeField;
        }
        set {
            this.areaCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string telephoneNumber {
        get {
            return this.telephoneNumberField;
        }
        set {
            this.telephoneNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string telephoneExtensionNumber {
        get {
            return this.telephoneExtensionNumberField;
        }
        set {
            this.telephoneExtensionNumberField = value;
        }
    }
}