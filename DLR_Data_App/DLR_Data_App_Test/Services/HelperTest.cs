using System.Collections.Generic;
using DLR_Data_App.Models.ProjectModel;
using DLR_Data_App.Services;
using Xunit;

namespace DLR_Data_App_Test.Services
{
  public class HelperTest
  {
    [Fact]
    public void Encrypt_password_Test()
    {
      const string password = "Helloworld";
      const string expectedResult = "98b57e17fd890c8cd2abfaa8a180f7bec1e3d662a7f5dcdac9b69942865b9816dc5c747fc57ef24ba1323b8c8e3700af6fe97497f92eb656e33d408361679aa4";
      var result = Helpers.Encrypt_password(password);

      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void TranslateProjectDetailsTest()
    {
      List<Project> projectList = new List<Project>();

      // create example projects
      var exampleProject1 = new Project
      {
        Title = "{\"0\": \"Feldkampagne\",\"1\": \"Fieldcampagne\"}",
        Description =
          "{\"0\": \"Messungen des Wachstums von Pfanzen zur Kalibrierung von Satelliten\",\"1\": \"Measurements of the growth of plants for the calibration of satellites\"}",
        Languages = "{\"0\": \"German\",\"1\": \"English\"}",
        Authors =
          "{\"0\": \"Deutsches Zentrum für Luft-und Raumfahrt e.V. (DLR) Jena und Friedrich Schiller Universität Jena\",\"1\": \"German Aerospace Center(DLR e.V.) Jena and Friedrich Schiller University Jena\"}"
      };

      // add projects to list
      projectList.Add(exampleProject1);

      // run translating process
      var resultList = Helpers.TranslateProjectDetails(projectList);
      
      // check result
      if (resultList[0].Authors == "Deutsches Zentrum für Luft-und Raumfahrt e.V. (DLR) Jena und Friedrich Schiller Universität Jena" ||
          resultList[0].Title == "Feldkampagne" ||
          resultList[0].Description == "Messungen des Wachstums von Pfanzen zur Kalibrierung von Satelliten")
      {
        Assert.True(true);
      }
      else
      {
        Assert.True(false);
      }
      
    }
  }
}
