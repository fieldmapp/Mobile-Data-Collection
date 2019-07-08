using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using DLR_Data_App.Models.ProjectModel;

namespace DLR_Data_App.Services
{
  /**
   * Parse form definition files for project
   */
  class Parser
  {
    private Project workingProject;
    
    /**
     * Constructor
     */
    public Parser(ref Project project)
    {
      workingProject = project;
    }

    /**
     * Executes all processes to execute zip archive and add forms to project
     */
    public async Task<bool> ParseZip(string zipfile, string unzipfolder)
    {
      int countProjectForms = workingProject.Formlist.Count;
      bool status = await Helpers.UnzipFileAsync(zipfile, unzipfolder);

      if (!status)
      {
        return false;
      }
      string[] unzipContent = Directory.GetFiles(unzipfolder);

      foreach (string file in unzipContent)
      {
        workingProject = ParseJson(workingProject, file);
      }

      // check if new forms are added to formlist and return state
      if (countProjectForms < workingProject.Formlist.Count)
      {
        return true;
      } else {
        return false;
      }
    }

    /**
     * Parse Json files
     * @param filename Path and File name for JSON file to parse
     * @see <a href="https://stackoverflow.com/questions/12676746/parse-json-string-in-c-sharp">Stackoverflow</a>
     */
    public Project ParseJson(Project project, string filename)
    {

      ProjectForm form = new ProjectForm();
      var json = System.IO.File.ReadAllText(filename);

      var objects = JObject.Parse(json); // parse as object  

      // Walk through the root object
      foreach (KeyValuePair<String, JToken> app in objects)
      {
        var key = app.Key;
        var value = app.Value;

        // Form odkbuild file
        if (key == "controls")
        {
          // parsing the elements of a form
          foreach (JObject controlElement in value)
          {
            ProjectFormElements projectFormElements = new ProjectFormElements();
            foreach (KeyValuePair<String, JToken> control in controlElement)
            {
              var controlKey = control.Key;
              var controlValue = control.Value;
              var result = false;

              if (controlKey == "name")
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
                Boolean.TryParse(controlValue.ToString(), out result);
                projectFormElements.ReadOnly = result;
              }

              if (controlKey == "required")
              {
                Boolean.TryParse(controlValue.ToString(), out result);
                projectFormElements.Required = result;
              }

              if (controlKey == "requiredText")
              {
                //projectFormElements.ReadOnly = Boolean.TryParse(controlValue.First;
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
                Boolean.TryParse(controlValue.ToString(), out result);
                projectFormElements.Cascading = result;
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
            form.ElementList.Add(projectFormElements);
          }
        }
        else if (key == "metadata")
        {
          ProjectFormMetadata projectFormMetadata = new ProjectFormMetadata();

          // parsing the metadata of the form
          foreach (KeyValuePair<String, JToken> metadata in (JObject)value)
          {
            var metadataKey = metadata.Key;
            var metadataValue = metadata.Value;
            var result = 0;

            if (metadataKey == "version")
            {
              int.TryParse(metadataValue.ToString(), out result);
              projectFormMetadata.Version = result;
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

        // Project json file
        else if(key == "Project")
        {
          project.Title = value.ToString();
        }
        else if (key == "Author")
        {
          project.Authors = value.ToString();
        }
        else if (key == "Description")
        {
          project.Description = value.ToString();
        }
        else if (key == "Secret")
        {
          project.Secret = value.ToString();
        }
        else if (key == "Languages")
        {
          project.Languages = value.ToString();
        }
        else
        {
          // get title of form
          form.Title = value.ToString();
        }
      }

      if (filename.EndsWith(".odkbuild"))
      {
        project.Formlist.Add(form);
      }
      
      return project;
    }

    /**
     * Parsing string in the correct language from json string
     */
    public static string LanguageJSON(string jsonList, string languageList)
    {
      // get current language
      string currentLanguage = CultureInfo.CurrentUICulture.EnglishName;
      string result = "Unable to parse language from json";

      try
      {
        // check which languages are available
        var objects = JObject.Parse(languageList); // parse as object  
        foreach (KeyValuePair<String, JToken> app in objects)
        {
          var key = app.Key;
          var value = app.Value.ToString();

          // if local language matches available language store key in result
          if (currentLanguage.Contains(value)) {
            result = key;
            break;
          }
        }

        // get string from jsonlist in the correct language
        objects = JObject.Parse(jsonList); // parse as object  
        foreach (KeyValuePair<String, JToken> app in objects)
        {
          var key = app.Key;
          var value = app.Value.ToString();

          // if key is found set matching string as result
          if (key == result)
          {
            result = value;
            break;
          }
        }
      } catch (Exception e)
      {

      }
      
      return result;
    }
  }
}
