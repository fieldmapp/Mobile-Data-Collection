using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DLR_Data_App.Services
{
    public interface IStorageAccessProvider
    {
        Stream OpenAsset(string path);
        
        Stream OpenFileRead(string path);

        Stream OpenFileWrite(string path);

        Stream OpenFileWriteExternal(string path);
    }
}
