using DLR_Data_App.Models.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    class TextInputElement : FormElement
    {
        public TextInputElement(Grid grid, ProjectFormElements data, string type) : base(grid, data, type) { }

        public Entry Entry;

        public override bool IsValid => !string.IsNullOrEmpty(Entry.Text);

        public override string GetRepresentationValue() => Entry.Text;

        public override void LoadFromSavedRepresentation(string representation) => Entry.Text = representation;

        public override void Reset() => Entry.Text = string.Empty;
    }
}
