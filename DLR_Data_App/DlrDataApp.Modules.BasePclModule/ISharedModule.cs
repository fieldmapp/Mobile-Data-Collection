using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    /// <summary>
    /// Defines which properties and functions every Module needs to implement
    /// </summary>
    public interface ISharedModule
    {
        /// <summary>
        /// The modules name
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// Called on all modules during splashscreen load.
        /// </summary>
        /// <param name="moduleHost">The <see cref="IModuleHost"/> which gives access to provided core functionality</param>
        /// <returns>Task representing the initialization of this module</returns>
        Task Initialize(IModuleHost moduleHost);

        /// <summary>
        /// Called on all modules during splashscreen load AFTER all modules have finished intializing
        /// </summary>
        /// <returns>Task representing the post-initialization of this module</returns>
        Task PostInitialize();
    }
}
