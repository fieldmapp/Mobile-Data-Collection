using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.SharedModule
{
    public interface IUser
    {
        int Id { get; }
        string Username { get; }
    }
}
