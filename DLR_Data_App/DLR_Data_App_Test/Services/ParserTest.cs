using System.Globalization;
using DLR_Data_App.Services;
using Xunit;

namespace DLR_Data_App_Test.Services
{
  public class ParserTest
  {
    [Fact]
    public void LanguageJsonTest()
    {
      CultureInfo.CurrentCulture = new CultureInfo("de-DE");
      const string input = "{\"0\": \"ResultText\", \"1\": \"FalseText\"}";
      const string languages = "{\"0\": \"German\", \"1\": \"English\"}";
      const string expectedResult = "ResultText";
      var result = Parser.LanguageJson(input, languages);

      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void LanguageJsonStandardTest()
    {
      const string input = "{\"0\": \"FalseText\", \"1\": \"ResultText\"}";
      const string languages = "{\"0\": \"German\", \"1\": \"English\"}";
      const string expectedResult = "ResultText";
      var result = Parser.LanguageJsonStandard(input, languages);

      Assert.Equal(expectedResult, result);
    }
  }
}
