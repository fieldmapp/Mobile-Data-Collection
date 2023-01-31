using DlrDataApp.Modules.Base.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared
{
    /// <summary>
    /// Interface which defines properties of an app which hosts the FieldMApp.
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// Acces to information from the module handling class (including other modules)
        /// </summary>
        IModuleHost ModuleHost { get; }

        /// <summary>
        /// Provides information about the current user.
        /// </summary>
        IUser CurrentUser { get; }

        /// <summary>
        /// Reference to the currently displayed page
        /// </summary>
        Page CurrentPage { get; }

        /// <summary>
        /// Wrapper around shared SQLite database
        /// </summary>
        Database Database { get; }

        /// <summary>
        /// Easy and safe way to generate random numbers
        /// </summary>
        ThreadSafeRandom RandomProvider { get; }

        /// <summary>
        /// Wrapper around Sensors that takes into account user prefrences
        /// </summary>
        Sensor Sensor { get; }

        /// <summary>
        /// The absolute Path of the App on the internal storage
        /// </summary>
        string FolderLocation { get; }

        /// <summary>
        /// The absolute path of the media directory. The media directory will be included when the user exports the database.
        /// </summary>
        string MediaPath { get; }


        /// <summary>
        /// Item to which the modules should append their navigation items to
        /// </summary>
        FlyoutItem FlyoutItem { get; }

        /// <summary>
        /// Storage for all localization resources that should be accessible via <see cref="DlrDataApp.Modules.Base.Shared.Localization.TranslateExtension"/>
        /// </summary>
        ResourceDictionary Resources { get; }
    }
}
