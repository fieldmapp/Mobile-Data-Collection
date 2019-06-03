using DLR_Data_App.Localizations;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Xamarin.Forms;

namespace DLR_Data_App.Services
{
  class Helpers
  {
    public Helpers()
    {

    }

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
  }
}
