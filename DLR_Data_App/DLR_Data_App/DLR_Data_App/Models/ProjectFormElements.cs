using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models
{
  class ProjectFormElements
  {
    public string Name { get; set; }
    public List<string> Label { get; set; }
    public string Hint { get; set; }
    public string DefaultValue { get; set; }
    public bool ReadOnly { get; set; }
    public bool Required { get; set; }
    public List<string> RequiredText { get; set; }
    public string Relevance { get; set; }
    public string Constraint { get; set; }
    public List<String> InvalidText { get; set; }
    public string Calculate { get; set; }
    public List<List<string>> Options { get; set; }
    public bool Cascading { get; set; }
    public List<string> Other { get; set; }
    public string Appearance { get; set; }
    public List<string> Metadata { get; set; }
    public string Type { get; set; }
  }
}
