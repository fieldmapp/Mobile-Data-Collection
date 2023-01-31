using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IUser
    {
        int? Id { get; }
        string Username { get; }
    }
}
