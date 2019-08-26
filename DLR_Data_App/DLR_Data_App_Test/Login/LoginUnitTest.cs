using DLR_Data_App.Models;
using DLR_Data_App.Services;
using DLR_Data_App.ViewModels.Login;
using Xunit;

namespace DLR_Data_App_Test.Login
{
  public class LoginUnitTest
  {
    [Fact]
    public void Check_Information_Test()
    {
      var testUser = new User
      {
        Username = "TestUser",
        Password = Helpers.Encrypt_password("TestPassword")
      };

      Database.Insert(ref testUser);

      LoginViewModel viewModel = new LoginViewModel();
      bool result = viewModel.Check_Information(testUser.Username, testUser.Password);
      Assert.True(result);

      Database.Delete(ref testUser);
    }
  }
  
}
