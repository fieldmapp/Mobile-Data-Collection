using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models
{
  public enum MenuItemType
  {
    Current_Project,
    Projects,
    Sensortest,
    Settings,
    About,
    Logout
  }
  public class HomeMenuItem
  {
    public MenuItemType Id { get; set; }

    public string Title { get; set; }
  }
}
