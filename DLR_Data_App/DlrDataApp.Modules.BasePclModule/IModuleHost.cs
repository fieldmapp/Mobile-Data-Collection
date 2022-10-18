﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IModuleHost
    {
        List<ISharedModule> Modules { get; }
        List<Func<string, string>> RedirectionFuncs { get; }
        IApp App { get; }
        ISharedMethodProvider SharedMethodProvider { get; }
        void AddNavigationValidityCheck(Func<string, string> redirectionFunc);
    }
}
