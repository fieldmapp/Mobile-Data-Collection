using DLR_Data_App.Models.ProjectModel;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DLR_Data_App.Services
{
  /**
   * Class contains static helper functions
   */
  public class Helpers
  {
    /**
     * Encrypts passphrases in SHA512
     * @see https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=netframework-4.8#System_Security_Cryptography_HashAlgorithm_ComputeHash_System_Byte__
     * @see https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.sha512?view=netframework-4.8
     */
    public static string Encrypt_password(string password)
    {
      var data = Encoding.UTF8.GetBytes(password);
      byte[] resultBytes;
      var sBuilder = new StringBuilder();

      using (SHA512 shaM = new SHA512Managed())
      {
        resultBytes = shaM.ComputeHash(data);
      }

      foreach (var t in resultBytes)
      {
        sBuilder.Append(t.ToString("x2"));
      }

      return sBuilder.ToString();
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
        var fileStreamIn = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
        var zipInStream = new ZipInputStream(fileStreamIn);
        var entry = zipInStream.GetNextEntry();
        if(Directory.Exists(unzipFolderPath))
          Directory.Delete(unzipFolderPath, true);

        while (entry != null && entry.CanDecompress)
        {
          var outputFile = unzipFolderPath + @"/" + entry.Name;
          var outputDirectory = Path.GetDirectoryName(outputFile);

          if (outputDirectory != null)
          {
            if (!Directory.Exists(outputDirectory))
              Directory.CreateDirectory(outputDirectory);
          }
          else
          {
            return false;
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
      catch (Exception)
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
      var tempProject = new Project
      {
        Authors = Parser.LanguageJson(project.Authors, project.Languages),
        Title = Parser.LanguageJson(project.Title, project.Languages),
        Description = Parser.LanguageJson(project.Description, project.Languages)
      };


      return tempProject;
    }

    /**
     * Translating project details to system language
     * @param projectList List of projects
     */
    public static List<Project> TranslateProjectDetails(List<Project> projectList)
    {
      var tempProjectList = new List<Project>();

      foreach (var t in projectList)
      {
        tempProjectList.Add(TranslateProjectDetails(t));
      }

      return tempProjectList;
    }

    /**
     * Set current user
     */
    public static void SetCurrentUser()
    {
      // get user id
      if (App.CurrentUser.Id == 0)
      {
        var userList = Database.ReadUser();
        foreach (var user in userList)
        {
          if (user.Username == App.CurrentUser.Username
              && user.Password == App.CurrentUser.Password)
          {
            App.CurrentUser = user;
          }
        }
      }
    }
  }
}
