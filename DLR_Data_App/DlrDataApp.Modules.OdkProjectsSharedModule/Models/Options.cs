﻿using System.Collections.Generic;

namespace DlrDataApp.Modules.OdkProjects.Shared.Models
{
    public class Options
    {
        public Dictionary<string, string> Text { get; set; }
        public List<object> Cascade { get; set; }
        public string Val { get; set; }
    }
}
