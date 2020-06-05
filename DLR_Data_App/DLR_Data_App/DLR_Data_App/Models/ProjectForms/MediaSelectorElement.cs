using DLR_Data_App.Controls;
using DLR_Data_App.Models.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class MediaSelectorElement : FormElement
    {
        public MediaSelectorElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

        public DataHolder DataHolder;

        public override bool IsValid => !string.IsNullOrEmpty(DataHolder.Data) && base.IsValid;

        public override string GetRepresentationValue() => DataHolder.Data ?? string.Empty;

        public override void LoadFromSavedRepresentation(string representation) => DataHolder.Data = representation;

        public override void Reset() => DataHolder.Data = string.Empty;
    }
}
