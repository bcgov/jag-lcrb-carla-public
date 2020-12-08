
namespace Gov.Lclb.Cllb.Interfaces.Models
{
    public partial class MicrosoftDynamicsCRMadoxioSpddatarow
    {
        //enable accessing properties using string names
        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

    }
}
