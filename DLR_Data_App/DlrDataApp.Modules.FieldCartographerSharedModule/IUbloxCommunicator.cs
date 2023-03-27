using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    public interface IUbloxCommunicator
    {
        DateTime LatestReceivedNMEADate { get; }
    }
}
