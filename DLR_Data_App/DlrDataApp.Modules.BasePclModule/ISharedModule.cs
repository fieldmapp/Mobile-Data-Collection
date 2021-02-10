using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.SharedModule
{
    public interface ISharedModule
    {
        Task Initialize(IModuleHost moduleHost);
        Task PostInitialize();
    }
}
