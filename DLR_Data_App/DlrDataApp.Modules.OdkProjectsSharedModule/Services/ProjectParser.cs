﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using DlrDataApp.Modules.OdkProjects.Shared.Models.ProjectModel;
using DlrDataApp.Modules.OdkProjects.Shared.Models;

namespace DlrDataApp.Modules.OdkProjects.Shared.Services
{
    /// <summary>
    /// Handles the parsing of form definition files for project
    /// </summary>
    public class ProjectParser
    {
        private Project _workingProject;
        
        /// <summary>
        /// Project which will be used
        /// </summary>
        /// <param name="project"></param>
        public ProjectParser(Project project)
        {
            _workingProject = project;
        }

        /// <summary>
        /// Executes all steps to unpack zip archive and add forms to project.
        /// </summary>
        /// <param name="zipFile">Path to ZIP archive</param>
        /// <param name="unzipFolder">Path to extract folder</param>
        public async Task<bool> ParseZip(string zipFile, string unzipFolder)
        {
            // Extract zip archive
            var status = await Base.Shared.Helpers.UnzipFileAsync(zipFile, unzipFolder);
            if (!status)
            {
                return false;
            }

            // Get files
            var unzipContent = Directory.GetFiles(unzipFolder).OrderBy(s => s).ToArray();
            foreach (var file in unzipContent)
            {
                // Parse file
                ParseJson(_workingProject, file);
            }

            // check if new forms are added to form list and return state
            return _workingProject.FormList.Count > 0;
        }

        /// <summary>
        /// Parses Json files.
        /// </summary>
        /// <param name="project">Project which will be filled with parsed information</param>
        /// <param name="filename">Path and File name for JSON file to parse</param>
        /// <see cref="https://stackoverflow.com/questions/12676746/parse-json-string-in-c-sharp"/>
        public void ParseJson(Project project, string filename)
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
                    //Form odkbuild file
                    case "controls":
                        {
                            foreach (var jToken in value)
                            {
                                var controlElement = (JObject)jToken;
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
                                        case "range":
                                            projectFormElements.Range = controlValue.ToString();
                                            break;
                                        case "kind":
                                            projectFormElements.Kind = controlValue.ToString();
                                            break;
                                        case "length":
                                            projectFormElements.Length = controlValue.ToString();
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
                
                    case "title":
                        form.Title = value.ToString();
                        break;
                    case "ProfilingId":
                        project.ProfilingId = value.ToString();
                        break;
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
                    case "PreviewPattern":
                        project.PreviewPattern = value.ToString();
                        break;
                }
            }

            if (filename.EndsWith(".odkbuild"))
            {
                project.FormList.Add(form);
            }
        }

        /// <summary>
        /// Deserializes string to List of <see cref="Options"/>
        /// </summary>
        /// <param name="inputString">String to be deserialized</param>
        /// <returns>List of <see cref="Options"/> that were serialized in the given string</returns>
        public static List<Options> ParseOptionsFromJson(string inputString)
        {
            return JsonConvert.DeserializeObject<List<Options>>(inputString);
        }
    }
}