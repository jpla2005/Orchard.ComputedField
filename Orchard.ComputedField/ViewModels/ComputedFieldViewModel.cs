using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Orchard.ComputedField.ViewModels
{
    public class ComputedFieldViewModel
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public string SelectedValue { get; set; }
        public IEnumerable<dynamic> Options { get; set; }

        public ComputedFieldViewModel() {
            Options = new List<dynamic>();
        }

        public IEnumerable<SelectListItem> GetList()
        {
            return Options.Select(opt => new SelectListItem
            {
                Value = opt.Value.ToString(),
                Text = opt.Text.ToString()
            });
        }
    }
}