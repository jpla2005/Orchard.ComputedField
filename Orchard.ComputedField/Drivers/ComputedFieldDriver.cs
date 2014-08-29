using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Orchard.ComputedField.Settings;
using Orchard.ComputedField.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace Orchard.ComputedField.Drivers
{
    public class ComputedFieldDriver : ContentFieldDriver<global::Orchard.ComputedField.Fields.ComputedField>
    {
        private const string TemplateName = "Fields/Custom.Computed";

        public IOrchardServices OrchardServices { get; set; }
        public Localizer T { get; set; }

        public ComputedFieldDriver(IOrchardServices orchardServices)
        {
            OrchardServices = orchardServices;
            T = NullLocalizer.Instance;
        }

        private static string GetPrefix(ContentField field, ContentPart part)
        {
            return part.PartDefinition.Name + "." + field.Name;
        }

        protected override DriverResult Display(ContentPart part, global::Orchard.ComputedField.Fields.ComputedField field, string displayType, dynamic shapeHelper)
        {
            var value = field.SelectedValue;

            return ContentShape("Fields_Custom_Computed", field.Name, s => s.Name(field.Name).SelectedValue(value));
        }

        protected override DriverResult Editor(ContentPart part, global::Orchard.ComputedField.Fields.ComputedField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<ComputedFieldSettings>();

            var model = new ComputedFieldViewModel {Required = settings.Required};
            updater.TryUpdateModel(model, GetPrefix(field, part), null, null);

            field.SelectedValue = model.SelectedValue;
            if (settings.Required && string.IsNullOrWhiteSpace(field.SelectedValue))
            {
                updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
            }

            return Editor(part, field, shapeHelper);
        }

        protected override DriverResult Editor(ContentPart part, global::Orchard.ComputedField.Fields.ComputedField field, dynamic shapeHelper)
        {
            var settings = field.PartFieldDefinition.Settings.GetModel<ComputedFieldSettings>();
            var value = field.SelectedValue;

            var serviceUrl = new StringBuilder(settings.ServiceUrl);
            if (!settings.ServiceUrl.StartsWith("http"))
            {
                var requestUrl = OrchardServices.WorkContext.HttpContext.Request;
                var baseUrl = new StringBuilder();
                baseUrl.AppendFormat("{0}://{1}:{2}{3}", requestUrl.Url.Scheme, requestUrl.Url.Host, requestUrl.Url.Port, requestUrl.ApplicationPath);
                if (settings.ServiceUrl.StartsWith("~"))
                {
                    serviceUrl.Remove(0, 1);
                }

                serviceUrl.Insert(0, baseUrl);
            }

            var model = new ComputedFieldViewModel
            {
                Name = field.Name,
                Required = settings.Required,
                SelectedValue = value,
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(serviceUrl.ToString()).Result;
                if (response.IsSuccessStatusCode)
                {
                    var items = new List<dynamic>();
                    if (!string.IsNullOrWhiteSpace(settings.SelectItem))
                    {
                        items.Add(new {Text = T(settings.SelectItem), Value = string.Empty});
                    }

                    items.AddRange(response.Content.ReadAsAsync<IEnumerable<dynamic>>().Result);

                    model.Options = items;
                }
            }

            return ContentShape("Fields_Custom_Computed_Edit", () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName, Model: model, Prefix: GetPrefix(field, part)));
        }

        protected override void Importing(ContentPart part, global::Orchard.ComputedField.Fields.ComputedField field, ImportContentContext context)
        {
            var importedText = context.Attribute(GetPrefix(field, part), "Computed");
            if (importedText != null)
            {
                field.Storage.Set(null, importedText);
            }
        }

        protected override void Exporting(ContentPart part, global::Orchard.ComputedField.Fields.ComputedField field, ExportContentContext context)
        {
            context.Element(GetPrefix(field, part)).SetAttributeValue("Computed", field.Storage.Get<string>(null));
        }
    }
}