using System;
using System.Collections.Generic;
using System.Text;
using DlrDataApp.Modules.Base.Shared;

namespace DLR_Data_App.Services
{
    public class SharedMethodProvider : ISharedMethodProvider
    {
        Dictionary<(string ModuleIdentifier, string MethodIdentifier), Func<object, object>> KnownMethods 
            = new Dictionary<(string ModuleIdentifier, string MethodIdentifier), Func<object, object>>();
        public R Call<I, R>(string ModuleIdentifier, string MethodIdentifier, I input)
        {
            if (!KnownMethods.TryGetValue((ModuleIdentifier, MethodIdentifier), out Func<object, object> method))
                throw new Exception("Method not registered with SharedMethodProvider");

            object result = method(input);
            if (!(result is R castResult))
                throw new Exception("Method does not return expected type");

            return castResult;
        }

        public bool IsRegistered(string ModuleIdentifier, string MethodIdentifier)
        {
            return KnownMethods.ContainsKey((ModuleIdentifier, MethodIdentifier));
        }

        public bool Register(string ModuleIdentifier, string MethodIdentifier, Func<object, object> method)
        {
            if (KnownMethods.ContainsKey((ModuleIdentifier, MethodIdentifier)))
                return false;

            KnownMethods.Add((ModuleIdentifier, MethodIdentifier), method);
            return true;
        }
    }
}
