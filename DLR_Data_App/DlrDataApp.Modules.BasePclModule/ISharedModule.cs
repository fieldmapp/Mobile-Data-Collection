using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface ISharedModule
    {
        string ModuleName { get; }
        Task Initialize(IModuleHost moduleHost);
        Task PostInitialize();
    }
}
