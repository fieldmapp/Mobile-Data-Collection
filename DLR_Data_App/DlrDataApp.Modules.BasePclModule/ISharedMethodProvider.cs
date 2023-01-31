using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface ISharedMethodProvider
    {
        R Call<I, R>(string ModuleIdentifier, string MethodIdentifier, I input);
        bool Register(string ModuleIdentifier, string MethodIdentifier, Func<object, object> method);
        bool IsRegistered(string ModuleIdentifier, string MethodIdentifier);
    }
}
