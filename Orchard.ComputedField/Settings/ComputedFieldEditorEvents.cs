using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Orchard.ComputedField.Settings
{
    public class ComputedFieldEditorEvents : ContentDefinitionEditorEventsBase
    {
        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "ComputedField")
            {
                var model = definition.Settings.GetModel<ComputedFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            var model = new ComputedFieldSettings();
            if (builder.FieldType != "ComputedField")
            {
                yield break;
            }

            if (updateModel.TryUpdateModel(model, "ComputedFieldSettings", null, null))
            {
                builder.WithSetting("ComputedFieldSettings.ServiceUrl", model.ServiceUrl);
                builder.WithSetting("ComputedFieldSettings.Required", model.Required.ToString());
                builder.WithSetting("ComputedFieldSettings.SelectItem", model.SelectItem);
            }

            yield return DefinitionTemplate(model);
        }
    }
}