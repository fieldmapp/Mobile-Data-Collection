using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using DLR_Data_App.Models;
using DLR_Data_App.Models.ProjectModel;
using Newtonsoft.Json;

namespace DLR_Data_App.Services
{
  /**
   * Parse form definition files for project
   */
  public class Parser
  {
    private Project _workingProject;
    
    /**
     * Constructor
     * @param project project which will be used
     */
    public Parser(ref Project project)
    {
      _workingProject = project;
    }

    /**
     * Executes all processes to execute zip archive and add forms to project
     * @param zipFile Path to ZIP archive
     * @param unzipFolder Path to extract folder
     */
    public async Task<bool> ParseZip(string zipFile, string unzipFolder)
    {
      // Extract zip archive
      var status = await Helpers.UnzipFileAsync(zipFile, unzipFolder);
      if (!status)
      {
        return false;
      }

      // Get files
      var unzipContent = Directory.GetFiles(unzipFolder);
      foreach (var file in unzipContent)
      {
        // Parse file
        _workingProject = ParseJson(_workingProject, file);
      }

      // check if new forms are added to form list and return state
      return _workingProject.FormList.Count > 0;
    }

    /**
     * Parse Json files
     * @param project Project which will be filled with parsed information
     * @param filename Path and File name for JSON file to parse
     * @returns Project with parsed information
     * @see <a href="https://stackoverflow.com/questions/12676746/parse-json-string-in-c-sharp">Stackoverflow</a>
     */
    public Project ParseJson(Project project, string filename)
    {
      var form = new ProjectForm
      {
        ElementList = new List<ProjectFormElements>()
      };
      var json = File.ReadAllText(filename);

      var objects = JObject.Parse(json); // parse as object  

      // Walk through the root object
      foreach (var app in objects)
      {
        var key = app.Key;
        var value = app.Value;

        switch (key)
        {
          // Form odkbuild file
          case "controls":
          {
            // parsing the elements of a form
            foreach (var jToken in value)
            {
              var controlElement = (JObject) jToken;
              ProjectFormElements projectFormElements = new ProjectFormElements();
              foreach (var control in controlElement)
              {
                var controlKey = control.Key;
                var controlValue = control.Value;
                bool result;

                switch (controlKey)
                {
                  case "name":
                    projectFormElements.Name = controlValue.ToString();
                    break;
                  case "label":
                    projectFormElements.Label = controlValue.ToString();
                    break;
                  case "hint":
                    projectFormElements.Hint = controlValue.ToString();
                    break;
                  case "defaultValue":
                    projectFormElements.DefaultValue = controlValue.ToString();
                    break;
                  case "readOnly":
                    bool.TryParse(controlValue.ToString(), out result);
                    projectFormElements.ReadOnly = result;
                    break;
                  case "required":
                    bool.TryParse(controlValue.ToString(), out result);
                    projectFormElements.Required = result;
                    break;
                  case "requiredText":
                    projectFormElements.RequiredText = controlValue.ToString();
                    break;
                  case "relevance":
                    projectFormElements.Relevance = controlValue.ToString();
                    break;
                  case "constraint":
                    projectFormElements.Constraint = controlValue.ToString();
                    break;
                  case "invalidText":
                    projectFormElements.InvalidText = controlValue.ToString();
                    break;
                  case "calculate":
                    projectFormElements.Calculate = controlValue.ToString();
                    break;
                  case "options":
                    projectFormElements.Options = controlValue.ToString();
                    break;
                  case "cascading":
                    bool.TryParse(controlValue.ToString(), out result);
                    projectFormElements.Cascading = result;
                    break;
                  case "other":
                    projectFormElements.Other = controlValue.ToString();
                    break;
                  case "appearance":
                    projectFormElements.Appearance = controlValue.ToString();
                    break;
                  case "metadata":
                    projectFormElements.Metadata = controlValue.ToString();
                    break;
                  case "type":
                    projectFormElements.Type = controlValue.ToString();
                    break;
                }
              }
              form.ElementList.Add(projectFormElements);
            }

            break;
          }

          // Project json file
          case "metadata":
          {
            ProjectFormMetadata projectFormMetadata = new ProjectFormMetadata();

            // parsing the metadata of the form
            foreach (KeyValuePair<string, JToken> metadata in (JObject)value)
            {
              var metadataKey = metadata.Key;
              var metadataValue = metadata.Value;

              switch (metadataKey)
              {
                case "version":
                  int.TryParse(metadataValue.ToString(), out var result);
                  projectFormMetadata.Version = result;
                  break;
                case "activeLanguages":
                  projectFormMetadata.Languages = metadataValue.ToString();
                  break;
                case "optionsPresets":
                  projectFormMetadata.OptionsPresets = metadataValue.ToString();
                  break;
                case "instance_name":
                  projectFormMetadata.InstanceName = metadataValue.ToString();
                  break;
                case "htitle":
                  projectFormMetadata.Htitle = metadataValue.ToString();
                  break;
                case "public_key":
                  projectFormMetadata.PublicKey = metadataValue.ToString();
                  break;
                case "submission_url":
                  projectFormMetadata.SubmissionUrl = metadataValue.ToString();
                  break;
              }
            }

            form.Metadata = projectFormMetadata;
            break;
          }

          // Title of form
          case "title":
            form.Title = value.ToString();
            break;

          // Project Information Elements
          case "Project":
            project.Title = value.ToString();
            break;
          case "Author":
            project.Authors = value.ToString();
            break;
          case "Description":
            project.Description = value.ToString();
            break;
          case "Languages":
            project.Languages = value.ToString();
            break;
        }
      }

      if (filename.EndsWith(".odkbuild"))
      {
        project.FormList.Add(form);
      }
      
      return project;
    }

    /**
     * Return list of options parsed from string
     * @param inputString JSON Object which will be deserialized
     * @returns List with options
     */
    public static List<Options> ParseOptionsFromJson(string inputString)
    {
      return JsonConvert.DeserializeObject<List<Options>>(inputString);
    }

    /**
     * Parsing always the english string, if not available use the first language
     * @param jsonList JSON string containing multiple language versions
     * @param languageList List of available languages
     * @returns String with english text
     */
    public static string LanguageJsonStandard(string jsonList, string languageList)
    {
      var currentLanguage = CultureInfo.GetCultureInfo("en-GB").EnglishName;
      var result = "Unable to parse language from json";
      var temp = "0";

      try
      {
        // check which languages are available
        var objects = JObject.Parse(languageList); // parse as object  
        foreach (var app in objects)
        {
          var key = app.Key;
          var value = app.Value.ToString();

          // if local language matches available language store key in result
          if (!currentLanguage.Contains(value)) continue;
          temp = key;
          break;
        }

        // get string from json list in the correct language
        objects = JObject.Parse(jsonList); // parse as object  
        foreach (var app in objects)
        {
          var key = app.Key;
          var value = app.Value.ToString();

          // if key is found set matching string as result
          if (key != temp) continue;
          result = value;
          break;
        }
      }
      catch (Exception)
      {
        result = "Error in JSON file";
      }

      return result;
    }

    /**
     * Parsing string in the correct language from json string
     * @param jsonList JSON string containing multiple language versions
     * @param languageList List of available languages
     * @returns String with text in local language
     */
    public static string LanguageJson(string jsonList, string languageList)
    {
      // get current language
      var currentLanguage = CultureInfo.CurrentUICulture.EnglishName;
      var result = "Unable to parse language from json";
      var temp = "0";

      try
      {
        // check which languages are available
        var objects = JObject.Parse(languageList); // parse as object  
        foreach (var app in objects)
        {
          var key = app.Key;
          var value = app.Value.ToString();

          // if local language matches available language store key in result
          if (!currentLanguage.Contains(value)) continue;
          temp = key;
          break;
        }

        // get string from json list in the correct language
        objects = JObject.Parse(jsonList); // parse as object  
        foreach (var app in objects)
        {
          var key = app.Key;
          var value = app.Value.ToString();

          // if key is found set matching string as result
          if (key != temp) continue;
          result = value;
          break;
        }
      } catch (Exception)
      {
        result = "Error in JSON file";
      }
      
      return result;
    }
  }
}
