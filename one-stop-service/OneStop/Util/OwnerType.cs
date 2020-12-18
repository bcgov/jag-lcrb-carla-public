/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.2558.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class OwnerType : OtherPartyType {
    
    private TelephoneDeviceType homePhoneField;
    
    private FaxDeviceType homeFaxField;
    
    private string itemField;
    
    private ItemChoiceType itemElementNameField;
    
    /// <remarks/>
    public TelephoneDeviceType homePhone {
        get {
            return this.homePhoneField;
        }
        set {
            this.homePhoneField = value;
        }
    }
    
    /// <remarks/>
    public FaxDeviceType homeFax {
        get {
            return this.homeFaxField;
        }
        set {
            this.homeFaxField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("noSINReasonCode", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("socialInsuranceNumber", typeof(string), DataType="integer")]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
    public string Item {
        get {
            return this.itemField;
        }
        set {
            this.itemField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemChoiceType ItemElementName {
        get {
            return this.itemElementNameField;
        }
        set {
            this.itemElementNameField = value;
        }
    }
}