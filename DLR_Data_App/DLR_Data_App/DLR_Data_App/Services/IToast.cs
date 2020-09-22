using System;
using System.Collections.Generic;
using System.Text;

// see https://stackoverflow.com/a/44126899

namespace DLR_Data_App.Services
{
    public interface IToast
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
