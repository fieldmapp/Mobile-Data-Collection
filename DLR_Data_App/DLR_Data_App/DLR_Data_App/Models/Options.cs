using System.Collections.Generic;

namespace DLR_Data_App.Models
{
  public class Options
  {
    public Dictionary<string, string> Text { get; set; }
    public List<object> Cascade { get; set; }
    public string Val { get; set; }
  }
}
