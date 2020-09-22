using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DLR_Data_App.Services
{
    public interface ICameraProvider
    {
        Task<byte[]> OpenCameraApp();
    }
}
