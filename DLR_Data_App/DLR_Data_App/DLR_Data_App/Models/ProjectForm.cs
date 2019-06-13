using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Models
{
  class ProjectForm
  {
    public string Title { get; set; }

    public ProjectFormControls Controls { get; set; }

    public ProjectFormMetadata Metadata { get; set; }
  }
}
