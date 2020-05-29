using DLR_Data_App.Controls;
using DLR_Data_App.Localizations;
using DLR_Data_App.Services;
using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms.FormCreators
{
    class MediaSelectorFactory : ElementFactory
    {
        public override FormElement CreateForm(FormCreationParams parms)
        {
            var grid = CreateStandardBaseGrid(parms);
            var formElement = new MediaSelectorElement(grid, parms.Element, parms.Type);

            var pickFileButton = new Button { Text = AppResources.select };
            var fileSelectedLabel = new Label { Text = AppResources.fileselected };
            //HACK: bad way to store an image. blob would be better
            var dataHolder = new DataHolder { StyleId = parms.Element.Name };
            formElement.DataHolder = dataHolder;
            pickFileButton.Clicked += async (a, b) =>
            {
                var file = await CrossFilePicker.Current.PickFile();
                if (file != null)
                {
                    dataHolder.Data = Convert.ToBase64String(file.DataArray);
                    var length = dataHolder.Data.Length;
                }
                formElement.OnContentChange();
            };

            grid.Children.Add(pickFileButton, 0, 1);
            Grid.SetColumnSpan(pickFileButton, 2);
            grid.Children.Add(fileSelectedLabel, 0, 2);
            grid.Children.Add(dataHolder, 1, 2);
            return formElement;
        }
    }
}
