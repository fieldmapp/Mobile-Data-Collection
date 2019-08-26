using System.Collections.Generic;

namespace DLR_Data_App.Models
{
  public class Options
  {
    public Dictionary<string, string> text { get; set; }
    public List<object> cascade { get; set; }
    public string val { get; set; }
  }
}
