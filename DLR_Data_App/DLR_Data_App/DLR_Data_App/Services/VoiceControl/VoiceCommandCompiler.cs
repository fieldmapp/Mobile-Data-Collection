using DLR_Data_App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DLR_Data_App.Services.VoiceControl
{
    public static class VoiceCommandCompiler
    {
        public enum KeywordSymbol
        {
            invalid,
            anfang, ende, abbrechen,
            gering, mittel, hoch, 
            hang, nass, maus, wild, lehm, sand, kuppe, ton, verdichtung, wende, 
            zone, spur, unk,
            number0, number1, number2, number3, number4, number5, number6, number7, number8, number9
        }
        static KeywordSymbol[] DamageTypes = new[] { KeywordSymbol.gering, KeywordSymbol.mittel, KeywordSymbol.hoch };
        static KeywordSymbol[] DamageCauses = new[]
        {
            KeywordSymbol.hang, KeywordSymbol.nass, KeywordSymbol.maus, KeywordSymbol.wild, KeywordSymbol.lehm, KeywordSymbol.sand, KeywordSymbol.kuppe,
            KeywordSymbol.ton, KeywordSymbol.verdichtung, KeywordSymbol.wende
        };
        public enum Type
        {
            gering, mittel, hoch
        }
        public enum Cause
        {
            hang, nass, maus, wild, lehm, sand, kuppe, ton, verdichtung, wende
        }

        public static bool IsNumber(this KeywordSymbol symbol) => NumberSymbols.Contains(symbol);
        public static int ToNumber(this KeywordSymbol symbol) => NumberSymbols.IndexOf(symbol);
        public static bool TryToNumber(this KeywordSymbol symbol, out int digit)
        {
            digit = -1;
            if (!symbol.IsNumber())
                return false;

            digit = symbol.ToNumber();
            return true;
        }
        public static bool IsDamageType(this KeywordSymbol symbol) => DamageTypes.Contains(symbol);
        public static bool IsDamageCause(this KeywordSymbol symbol) => DamageCauses.Contains(symbol);


        readonly static Dictionary<string, KeywordSymbol> KeywordStringToSymbol = new Dictionary<string, KeywordSymbol>
        {
            { "anfang", KeywordSymbol.anfang },
            { "ende", KeywordSymbol.ende },
            { "abbrechen", KeywordSymbol.abbrechen },
            { "gering", KeywordSymbol.gering },
            { "mittel", KeywordSymbol.mittel },
            { "hoch", KeywordSymbol.hoch },
            { "hang", KeywordSymbol.hang },
            { "nass", KeywordSymbol.nass },
            { "nässe", KeywordSymbol.nass },
            { "maus", KeywordSymbol.maus },
            { "wild", KeywordSymbol.wild },
            { "lehmig", KeywordSymbol.lehm },
            { "sand", KeywordSymbol.sand },
            { "kuppe", KeywordSymbol.kuppe },
            { "ton", KeywordSymbol.ton },
            { "verdichtung", KeywordSymbol.verdichtung },
            { "wende", KeywordSymbol.wende },
            { "zone", KeywordSymbol.zone },
            { "spur", KeywordSymbol.spur },
            { "[unk]", KeywordSymbol.unk },
            { "null", KeywordSymbol.number0 },
            { "eins", KeywordSymbol.number1 },
            { "zwei", KeywordSymbol.number2 },
            { "drei", KeywordSymbol.number3 },
            { "vier", KeywordSymbol.number4 },
            { "fünf", KeywordSymbol.number5 },
            { "sechs", KeywordSymbol.number6 },
            { "sieben", KeywordSymbol.number7 },
            { "acht", KeywordSymbol.number8 },
            { "neun", KeywordSymbol.number9 }
        };

        public abstract class VoiceAction { }
        public abstract class ZonesAction : VoiceAction 
        { 
            public List<int> LaneIndices = new List<int>();
            public override string ToString()
            {
                return '{' + string.Join(", ", LaneIndices.Select(i => i.ToString())) + '}'; 
            }
        }
        public class StartZonesAction : ZonesAction 
        {
            public override string ToString()
            {
                return "start " + base.ToString();
            }
        }
        public class EndZonesAction : ZonesAction 
        {
            public override string ToString()
            {
                return "end " + base.ToString();
            }
        }
        public class SetZonesCauseAction : ZonesAction 
        {
            public KeywordSymbol DamageCause;
            public override string ToString()
            {
                return base.ToString() + " cause: " + KeywordStringToSymbol.FirstOrDefault(kv => kv.Value == DamageCause).Key;
            }
        }
        public class SetZonesTypeAction : ZonesAction 
        { 
            public KeywordSymbol DamageType;
            public override string ToString()
            {
                return base.ToString() + " type: " + KeywordStringToSymbol.FirstOrDefault(kv => kv.Value == DamageType).Key;
            }
        }
        public class CancelAction : VoiceAction 
        {
            public override string ToString()
            {
                return "cancel";
            }
        }
        public class InvalidAction : VoiceAction 
        {
            public override string ToString()
            {
                return "invalid";
            }
        }

        readonly static KeywordSymbol[] NumberSymbols = new[]
        {
            KeywordSymbol.number0, KeywordSymbol.number1, KeywordSymbol.number2, KeywordSymbol.number3, KeywordSymbol.number4, 
            KeywordSymbol.number5, KeywordSymbol.number6, KeywordSymbol.number7, KeywordSymbol.number8, KeywordSymbol.number9
        };

        readonly static List<KeywordSymbol> ExcludedNumberSymbols = NumberSymbols.Skip(DrivingPage.LaneCount).ToList();

        public readonly static List<string> KeywordStrings =
            KeywordStringToSymbol
            .Where(kv => !ExcludedNumberSymbols.Contains(kv.Value))
            .Select(kv => kv.Key)
            .ToList();

        public static VoiceAction Compile(List<string> recognizedKeywords) => Parse(Scan(recognizedKeywords));
             
        static List<KeywordSymbol> Scan(List<string> recognizedKeywords) => recognizedKeywords.Select(k => KeywordStringToSymbol[k]).ToList();

        static VoiceAction Parse(List<KeywordSymbol> symbols)
        {
            var symbolsEnumerable = symbols.AsEnumerable();
            var firstSymbol = symbolsEnumerable.FirstOrDefault();
            symbolsEnumerable = symbolsEnumerable.Skip(1);

            if (firstSymbol == KeywordSymbol.anfang || firstSymbol == KeywordSymbol.ende || firstSymbol == KeywordSymbol.zone)
            {
                ZonesAction action;
                if (firstSymbol == KeywordSymbol.anfang)
                    action = new StartZonesAction();
                else if (firstSymbol == KeywordSymbol.ende)
                    action = new EndZonesAction();
                else if (firstSymbol == KeywordSymbol.zone)
                {
                    if (symbolsEnumerable.LastOrDefault().IsDamageCause())
                        action = new SetZonesCauseAction();
                    else if (symbols.LastOrDefault().IsDamageType())
                        action = new SetZonesTypeAction();
                    else
                        return new InvalidAction();
                }
                else
                    return new InvalidAction();

                if (symbolsEnumerable.FirstOrDefault() == KeywordSymbol.zone)
                    symbolsEnumerable = symbolsEnumerable.Skip(1);

                while (symbolsEnumerable.FirstOrDefault().IsNumber())
                {
                    int number = symbolsEnumerable.FirstOrDefault().ToNumber();
                    if (!action.LaneIndices.Contains(number))
                        action.LaneIndices.Add(number);
                    
                    symbolsEnumerable = symbolsEnumerable.Skip(1);
                }
                if (firstSymbol == KeywordSymbol.zone)
                {
                    var lastElement = symbolsEnumerable.FirstOrDefault();
                    symbolsEnumerable = symbolsEnumerable.Skip(1);
                    if (lastElement.IsDamageCause() && action is SetZonesCauseAction causeAction)
                        causeAction.DamageCause = lastElement;
                    else if (lastElement.IsDamageType() && action is SetZonesTypeAction typeAction)
                        typeAction.DamageType = lastElement;
                    else
                        return new InvalidAction();
                }

                if (symbolsEnumerable.Any())
                    return new InvalidAction();

                return action;
            }
            else if (firstSymbol == KeywordSymbol.abbrechen)
                return new CancelAction();
            else
                return new InvalidAction();
        }

    }
}
