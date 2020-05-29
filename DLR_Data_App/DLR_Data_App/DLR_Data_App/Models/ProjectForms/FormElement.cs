using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DLR_Data_App.Models.ProjectForms
{
    /// <summary>
    /// Class representing every part of a displayed element in a form.
    /// </summary>
    abstract class FormElement
    {
        public FormElement(Grid grid, ProjectFormElements data, string type)
        {
            Grid = grid;
            Data = data;
            ShouldBeShownExpression = string.IsNullOrWhiteSpace(data.Relevance) ? null : OdkDataExtractor.GetBooleanExpression(data.Relevance);
            Type = type;
        }

        public Grid Grid { get; }
        public ProjectFormElements Data { get; }
        public event EventHandler ValidContentChange;
        public event EventHandler InvalidContentChange;
        public OdkBooleanExpresion ShouldBeShownExpression { get; }
        public abstract bool IsValid { get; }
        public readonly string Type;

        public void OnContentChange()
        {
            if (IsValid)
                ValidContentChange?.Invoke(this, null);
            else
                InvalidContentChange?.Invoke(this, null);
        }

        /// <summary>
        /// Sets the grids content to the default value
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Retrieves a representation for this element
        /// </summary>
        /// <returns>Pair containing 1. the name and 2. the content of the element.</returns>
        public KeyValuePair<string, string> GetRepresentation()
        {
            return new KeyValuePair<string, string>(Data.Name, GetRepresentationValue());
        }

        public abstract string GetRepresentationValue();

        /// <summary>
        /// Sets grids content based on the given projetData
        /// </summary>
        /// <param name="projectData">Dictionary matching an elements name and its content</param>
        /// <returns><see cref="Boolean"/> indicating if this FormElement is set to a value which is making it valid</returns>
        public bool LoadContentFromProjectData(Dictionary<string, string> projectData)
        {
            if (projectData.TryGetValue(Data.Name, out string representation))
            {
                LoadFromSavedRepresentation(representation);
                return IsValid;
            }
            return false;
        }

        /// <returns><see cref="Boolean"/> indicating if this FormElement is set to a value which is making it valid</returns>
        public abstract void LoadFromSavedRepresentation(string representation);
    }
}
