using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IStorageAccessProvider
    {
        Stream OpenAsset(string path);
        FileStream OpenFileRead(string path);
        FileStream OpenFileWrite(string path);
        FileStream OpenFileAppend(string path);
        FileStream OpenFileWriteExternal(string path);
        FileStream OpenFileAppendExternal(string path);
    }
}
