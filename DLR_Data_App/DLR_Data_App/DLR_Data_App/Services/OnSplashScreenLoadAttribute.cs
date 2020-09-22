using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
    /// <summary>
    /// Attribute which causes the method being called during splash screen loading. Must be placed on a static method with no parameters to work.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class OnSplashScreenLoadAttribute : Attribute
    {
    }
}
