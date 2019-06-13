using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models
{
  class ProjectFormMetadata
  {
    public int Version { get; set; }
    public List<string> Languages { get; set; }
    public List<string> OptionsPresets { get; set; }
    public string Htitle { get; set; }
    public string InstanceName { get; set; }
    public string PublicKey { get; set; }
    public string SubmissionURL { get; set; }
  }
}
