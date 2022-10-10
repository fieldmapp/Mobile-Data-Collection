using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IModuleHost
    {
        IApp App { get; }
    }
}
