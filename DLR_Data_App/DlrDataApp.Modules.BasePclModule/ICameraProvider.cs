using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DlrDataApp.Modules.SharedModule
{
    public interface ICameraProvider
    {
        Task<byte[]> OpenCameraApp();
    }
}
