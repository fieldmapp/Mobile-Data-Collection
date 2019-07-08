using DLR_Data_App.Localizations;
using DLR_Data_App.Models.ProjectModel;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DLR_Data_App.Services
{
  /**
   * Class contains static helper functions
   */
  class Helpers
  {
    /*
     * Encrypts passphrases in SHA512
     */
    public static string Encrypt_password(string password)
    {
      var data = Encoding.UTF8.GetBytes(password);
      using (SHA512 shaM = new SHA512Managed())
      {
        data = shaM.ComputeHash(data);
      }

      return Encoding.UTF8.GetString(data);
    }

    /**
     * Extracts files from zip folder
     * @param filepath path to zip folder
     * @returns Status of unzipping
     * @see https://stackoverflow.com/questions/42118378/how-to-unzip-downloaded-zip-file-in-xamarin-forms
     */
    public static async Task<bool> UnzipFileAsync(string zipFilePath, string unzipFolderPath)
    {
      try
      {
        var entry = new ZipEntry(Path.GetFileNameWithoutExtension(zipFilePath));
        var fileStreamIn = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
        var zipInStream = new ZipInputStream(fileStreamIn);
        entry = zipInStream.GetNextEntry();
        //Directory.Delete(unzipFolderPath);

        while (entry != null && entry.CanDecompress)
        {
          var outputFile = unzipFolderPath + @"/" + entry.Name;
          var outputDirectory = Path.GetDirectoryName(outputFile);

          if (!Directory.Exists(outputDirectory))
          {
            Directory.CreateDirectory(outputDirectory);
          }

          if (entry.IsFile)
          {
            var fileStreamOut = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            int size;
            byte[] buffer = new byte[4096];
            do
            {
              size = await zipInStream.ReadAsync(buffer, 0, buffer.Length);
              await fileStreamOut.WriteAsync(buffer, 0, size);
            } while (size > 0);
            fileStreamOut.Close();
          }

          entry = zipInStream.GetNextEntry();
        }
        zipInStream.Close();
        fileStreamIn.Close();
      }
      catch (Exception e)
      {
        return false;
      }
      return true;
    }

    /**
     * Translating project details to system language
     * @param project Project
     */
    public static Project TranslateProjectDetails(Project project)
    {
      Project tempProject = new Project();

      tempProject.Authors = Parser.LanguageJSON(project.Authors, project.Languages);
      tempProject.Title = Parser.LanguageJSON(project.Title, project.Languages);
      tempProject.Description = Parser.LanguageJSON(project.Description, project.Languages);

      return tempProject;
    }

    /**
     * Translating project details to system language
     * @param projectList List of projects
     */
    public static List<Project> TranslateProjectDetails(List<Project> projectList)
    {
      List<Project> tempProjectList = new List<Project>();

      for (int i = 0; i < projectList.Count; i++)
      {
        tempProjectList.Add(TranslateProjectDetails(projectList[i]));
      }

      return tempProjectList;
    }
    
}
}
