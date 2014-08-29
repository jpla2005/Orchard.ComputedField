using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Orchard.ComputedField.Fields
{
    [OrchardFeature("ComputedField")]
    public class ComputedField : ContentField
    {
        public string SelectedValue
        {
            get { return Storage.Get<string>("SelectedValue"); }
            set { Storage.Set("SelectedValue", value ?? string.Empty); }
        }
    }
}