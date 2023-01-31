using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DlrDataApp.Modules.Base.Shared.DependencyServices
{
    /// <summary>
    /// Provides functions to open paths on the filesystem as <see cref="Stream"/>
    /// </summary>
    public interface IStorageAccessProvider
    {
        /// <summary>
        /// Reads a file from the assets
        /// </summary>
        /// <param name="path">Path to the file in assets</param>
        /// <returns><see cref="Stream"/> that can be used to read the requested file</returns>
        Stream OpenAsset(string path);

        /// <summary>
        /// Opens a file in read mode
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <returns><see cref="FileStream"/> that can be used to read the file</returns>
        FileStream OpenFileRead(string path);

        /// <summary>
        /// Opens a file in write mode.
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <returns><see cref="FileStream"/> that can be used to write the file</returns>
        FileStream OpenFileWrite(string path);


        /// <summary>
        /// Opens a file in append mode.
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <returns><see cref="FileStream"/> that can be used to append to the file</returns>
        FileStream OpenFileAppend(string path);


        /// <summary>
        /// Opens a file which is stored in external storage in write mode.
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <returns><see cref="FileStream"/> that can be used to write the file</returns>
        FileStream OpenFileWriteExternal(string path);

        /// <summary>
        /// Opens a file which is stored in external storage in append mode.
        /// </summary>
        /// <param name="path">Path of file</param>
        /// <returns><see cref="FileStream"/> that can be used to append to the file</returns>
        FileStream OpenFileAppendExternal(string path);
    }
}
