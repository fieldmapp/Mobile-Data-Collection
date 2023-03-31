using System.Globalization;
using System.Linq;
using static DlrDataApp.Modules.FieldCartographer.Shared.VoiceCommandCompiler;

namespace DlrDataApp.Modules.FieldCartographer.Shared.VoiceActions
{
    public class SetZonesDetailAction : ZonesAction
    {
        public KeywordSymbol DamageCause = KeywordSymbol.invalid;
        public KeywordSymbol DamageType = KeywordSymbol.invalid;
        public bool ShouldEndZone;
        public bool ShouldStartZone;
        public override string ToString()
        {
            string result = base.ToString();
            if (DamageCause != KeywordSymbol.invalid)
                result += " cause: " + KeywordStringToSymbol.First(kv => kv.Value == DamageCause).Key;
            if (DamageType != KeywordSymbol.invalid)
                result += " type: " + KeywordStringToSymbol.First(kv => kv.Value == DamageType).Key;
            result += $"; end zone: {ShouldEndZone.ToString(CultureInfo.InvariantCulture)}";
            result += $"; start zone: {ShouldStartZone.ToString(CultureInfo.InvariantCulture)}";
            return result;
        }
    }
}
