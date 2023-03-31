using System.Collections.Generic;
using System.Linq;

namespace DlrDataApp.Modules.FieldCartographer.Shared.VoiceActions
{
        public abstract class ZonesAction : VoiceAction
        { 
            public List<int> LaneIndices = new List<int>();
            public override string ToString()
            {
                return '{' + string.Join(", ", LaneIndices.Select(i => i.ToString())) + '}'; 
            }
        }
}
