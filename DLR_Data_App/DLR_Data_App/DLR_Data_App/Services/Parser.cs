using DLR_Data_App.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
  /*
   * Parse form definition files for project
   */
  class Parser
  {
    /*
     * Constructor
     */
    public Parser()
    {

    }

    /*
     * Parse Json files
     * @param filename Path and File name for JSON file to parse
     * @see <a href="https://stackoverflow.com/questions/12676746/parse-json-string-in-c-sharp">Stackoverflow</a>
     */
    private Project ParseJson(Project project, string filename)
    {

      ProjectForm form = new ProjectForm();
      var json = System.IO.File.ReadAllText(@"C:\Users\betar\workspace\ConsoleJsonParser\ConsoleJsonParser\Vorfrucht.odkbuild");

      var objects = JObject.Parse(json); // parse as object  

      // Walk through the root object
      foreach (KeyValuePair<String, JToken> app in objects)
      {
        var key = app.Key;
        var value = app.Value;
        
        if (key == "controls")
        {
          ProjectFormControls projectFormControls = new ProjectFormControls();
          
          // parsing the elements of a form
          foreach (JObject controlElement in value)
          {
            ProjectFormElements projectFormElements = new ProjectFormElements();
            foreach (KeyValuePair<String, JToken> control in controlElement)
            {
              var controlKey = control.Key;
              var controlValue = control.Value;
              
              if(controlKey == "name")
              {
                projectFormElements.Name = controlValue.ToString();
              }

              if (controlKey == "label")
              {
                //projectFormElements.Label = controlValue.ToString();
              }

              if (controlKey == "hint")
              {
                //projectFormElements.Hint = controlValue.ToString();
              }

              if (controlKey == "defaultValue")
              {
                projectFormElements.DefaultValue = controlValue.ToString();
              }

              if (controlKey == "readOnly")
              {
                projectFormElements.ReadOnly = (bool)controlValue.First;
              }

              if (controlKey == "required")
              {
                projectFormElements.Required = (bool)controlValue.First;
              }

              if (controlKey == "requiredText")
              {
                //projectFormElements.ReadOnly = (bool)controlValue.First;
              }

              if (controlKey == "relevance")
              {
                projectFormElements.Relevance = controlValue.ToString();
              }

              if (controlKey == "constraint")
              {
                projectFormElements.Constraint = controlValue.ToString();
              }

              if (controlKey == "invalidText")
              {
                //projectFormElements.Relevance = controlValue.ToString();
              }

              if (controlKey == "calculate")
              {
                projectFormElements.Calculate = controlValue.ToString();
              }

              if (controlKey == "options")
              {
                //projectFormElements.Relevance = controlValue.ToString();
              }

              if (controlKey == "cascading")
              {
                projectFormElements.Cascading = (bool)controlValue.First;
              }

              if (controlKey == "other")
              {
                //projectFormElements.Other = controlValue.First;
              }

              if (controlKey == "relevance")
              {
                projectFormElements.Relevance = controlValue.ToString();
              }

              if (controlKey == "appearance")
              {
                projectFormElements.Appearance = controlValue.ToString();
              }

              if (controlKey == "metadata")
              {
                //projectFormElements.Metadata = controlValue.ToString();
              }

              if (controlKey == "type")
              {
                projectFormElements.Type = controlValue.ToString();
              }
            }
            projectFormControls.ElementList.Add(projectFormElements);
          }

          form.Controls = projectFormControls;
        }
        else if (key == "metadata")
        {
          ProjectFormMetadata projectFormMetadata = new ProjectFormMetadata();
          
          // parsing the metadata of the form
          foreach (KeyValuePair<String, JToken> metadata in (JObject)value)
          {
            var metadataKey = metadata.Key;
            var metadataValue = metadata.Value;

            if (metadataKey == "version")
            {
              projectFormMetadata.Version = (int)metadataValue.First;
            }

            if (metadataKey == "activeLanguages")
            {
              //projectFormMetadata.Languages = metadataValue;
            }

            if (metadataKey == "optionsPresets")
            {
              //projectFormMetadata.OptionsPresets = metadataValue;
            }

            if (metadataKey == "instance_name")
            {
              projectFormMetadata.InstanceName = metadataValue.ToString();
            }

            if (metadataKey == "public_key")
            {
              projectFormMetadata.PublicKey = metadataValue.ToString();
            }

            if (metadataKey == "submission_url")
            {
              projectFormMetadata.SubmissionURL = metadataValue.ToString();
            }
          }

          form.Metadata = projectFormMetadata;
        }
        else
        {
          // get title of project
          form.Title = value.ToString();
        }
      }

      project.Formlist.Add(form);
      return project;
    }

    /*
     * Parse XForm files
     */
    private void ParseXForm()
    {

    }
  }
}
