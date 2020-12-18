/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class FaxDeviceType {
    
    private string areaCodeField;
    
    private string telephoneNumberField;
    
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
}