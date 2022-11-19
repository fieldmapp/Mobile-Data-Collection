using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DlrDataApp.Modules.Base.Shared.DependencyServices
{
    /// <summary>
    /// Provides a method to access the camera to take a photo that can be directly used in the app.
    /// </summary>
    public interface ICameraProvider
    {
        /// <summary>
        /// Will open the camera and prompt the user to take a photo.
        /// </summary>
        /// <returns>Task which will result in the image which the user has taken as a <see cref="byte[]"/></returns>
        Task<byte[]> OpenCameraApp();
    }
}
