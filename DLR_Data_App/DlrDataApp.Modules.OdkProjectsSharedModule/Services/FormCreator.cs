using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectForms;
using DlrDataApp.Modules.OdkProjectsSharedModule.Models.ProjectModel;
using DlrDataApp.Modules.SharedModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.OdkProjectsSharedModule.Services
{
    /// <summary>
    /// Class containing the parameters passed to a view creator.
    /// </summary>
    class FormCreationParams
    {
        public ProjectFormElements Element;
        public Project CurrentProject;
        public Func<string, string, string, Task> DisplayAlertFunc;
        public string Type;

        public FormCreationParams(string type, ProjectFormElements element, Project currentProject, Func<string, string, string, Task> displayAlertFunc)
        {
            Type = type;
            Element = element;
            CurrentProject = currentProject;
            DisplayAlertFunc = displayAlertFunc;
        }
    }

    /// <summary>
    /// Class containing a single form page and its elements
    /// </summary>
    class FormContent
    {
        public FormContent(ContentPage form, IReadOnlyList<FormElement> elements)
        {
            Form = form;
            Elements = elements;
        }

        public ContentPage Form { get; }
        public IReadOnlyList<FormElement> Elements { get; }
    }

    /// <summary>
    /// Static class providing functions to create a forms pages and elements.
    /// </summary>
    static class FormCreator
    {
        private static Dictionary<string, Func<FormCreationParams, FormElement>> TypeToViewCreator = new Dictionary<string, Func<FormCreationParams, FormElement>>()
        {
            { "inputText", TextInputElement.CreateForm },
            { "inputSelectOne", PickerElement.CreateForm },
            { "inputNumeric", NumericInputElement.CreateForm },
            { "inputLocation", LocationDetectorElement.CreateForm },
            { "inputDate", TimeSelectorElement.CreateForm },
            { "inputMedia", MediaSelectorElement.CreateForm }
        };

        private static Dictionary<string, Func<FormCreationParams, FormElement>> SpecialTypeToViewCreator = new Dictionary<string, Func<FormCreationParams, FormElement>>
        {
            { "compass", CompassElement.CreateForm }
        };

        /// <summary>
        /// Generates a single form page and its elements.
        /// </summary>
        /// <param name="form">Information of current form</param>
        /// <param name="currentProject">Information of current project</param>
        /// <param name="displayAlert">Async function which will display an alert.</param>
        /// <returns></returns>
        public static FormContent GenerateForm(ProjectForm form, Project currentProject, Func<string, string, string, Task> displayAlert)
        {
            var contentPage = new ContentPage();
            var scrollView = new ScrollView();
            var stack = new StackLayout();
            var elements = new List<FormElement>();

            contentPage.Padding = new Thickness(10, 10, 10, 10);

            contentPage.Title = form.Title;
            //walk through list of elements and generate form containing elements
            foreach (var element in form.ElementList)
            {
                IEnumerable<char> findSpecialType(string name) => name.SkipWhileIncluding(c => c != '{').TakeWhile(c => c != '}');

                FormElement formElement = null;

                bool specialType = false;
                if (element.Label != null)
                {
                    var translatedLabel = OdkDataExtractor.GetCurrentLanguageStringFromJsonList(element.Label, currentProject.Languages);
                    if (!string.IsNullOrWhiteSpace(translatedLabel) && findSpecialType(translatedLabel).Any())
                    {
                        //Special element
                        var specialElementType = new string(findSpecialType(translatedLabel).ToArray());
                        if (SpecialTypeToViewCreator.TryGetValue(specialElementType, out var viewCreator))
                        {
                            var formCreationParams = new FormCreationParams(specialElementType, element, currentProject, displayAlert);
                            specialType = true;
                            formElement = viewCreator(formCreationParams);
                        }
                    }
                }
                
                if (!specialType)
                {
                    if (TypeToViewCreator.TryGetValue(element.Type, out var viewCreator))
                    {
                        var formCreationParams = new FormCreationParams(element.Type, element, currentProject, displayAlert);
                        formElement = viewCreator(formCreationParams);
                    }
                }
                if (formElement != null)
                {
                    elements.Add(formElement);
                    stack.Children.Add(formElement.Grid);
                }
            }

            scrollView.Content = stack;
            contentPage.Content = scrollView;
            return new FormContent(contentPage, elements.AsReadOnly());
        }

    }
}
