using DlrDataApp.Modules.Base.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms.Xaml;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    public static class VoiceCommandCompiler
    {
        public enum KeywordSymbol
        {
            invalid,
            anfang, ende, abbrechen,
            gering, mittel, hoch, 
            hang, nass, maus, wild, trocken, sand, kuppe, ton, verdichtung, wende, waldrand,
            zone,
            number0, number1, number2, number3, number4, number5, number6, number7, number8, number9,
            unk, endOfStream
        }
        static KeywordSymbol[] DamageTypes = new[] { KeywordSymbol.gering, KeywordSymbol.mittel, KeywordSymbol.hoch };
        static KeywordSymbol[] DamageCauses = new[]
        {
            KeywordSymbol.hang, KeywordSymbol.nass, KeywordSymbol.maus, KeywordSymbol.wild, KeywordSymbol.trocken, KeywordSymbol.sand, KeywordSymbol.kuppe,
            KeywordSymbol.ton, KeywordSymbol.verdichtung, KeywordSymbol.wende, KeywordSymbol.waldrand
        };

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
            { "start", KeywordSymbol.anfang },
            { "stop", KeywordSymbol.ende },
            { "abbrechen", KeywordSymbol.abbrechen },
            { "gering", KeywordSymbol.gering },
            { "mittel", KeywordSymbol.mittel },
            { "hoch", KeywordSymbol.hoch },
            { "hang", KeywordSymbol.hang },
            { "nass", KeywordSymbol.nass },
            { "nässe", KeywordSymbol.nass },
            { "maus", KeywordSymbol.maus },
            { "mäuse", KeywordSymbol.maus },
            { "wild", KeywordSymbol.wild },
            { "trocken", KeywordSymbol.trocken },
            { "sand", KeywordSymbol.sand },
            { "kuppe", KeywordSymbol.kuppe },
            { "ton", KeywordSymbol.ton },
            { "verdichtung", KeywordSymbol.verdichtung },
            { "wald", KeywordSymbol.waldrand },
            { "wende", KeywordSymbol.wende },
            { "zone", KeywordSymbol.zone },
            { "spur", KeywordSymbol.number0 },
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
        public class SetZonesDetailAction : ZonesAction 
        {
            public KeywordSymbol DamageCause = KeywordSymbol.invalid;
            public KeywordSymbol DamageType = KeywordSymbol.invalid;
            public bool ShouldEndZone;
            public override string ToString()
            {
                string result = base.ToString();
                if (DamageCause != KeywordSymbol.invalid)
                    result += " cause: " + KeywordStringToSymbol.First(kv => kv.Value == DamageCause).Key;
                if (DamageType != KeywordSymbol.invalid)
                    result += " type: " + KeywordStringToSymbol.First(kv => kv.Value == DamageType).Key;
                result += $"; end zone: {ShouldEndZone.ToString(CultureInfo.InvariantCulture)}";
                return result;
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

        // TODO
        readonly static List<KeywordSymbol> ExcludedNumberSymbols = NumberSymbols.Skip(0/* TODO DrivingPage.MaxTotalLaneCount*/).ToList();

        public readonly static List<string> KeywordStrings =
            KeywordStringToSymbol
            .Where(kv => !ExcludedNumberSymbols.Contains(kv.Value))
            .Select(kv => kv.Key)
            .ToList();

        public static VoiceAction Compile(List<string> recognizedKeywords) => Parse(Scan(recognizedKeywords));
             
        static List<KeywordSymbol> Scan(List<string> recognizedKeywords) => recognizedKeywords.Select(k => KeywordStringToSymbol[k]).ToList();

        class SymbolStreamAccessor
        {
            public SymbolStreamAccessor(List<KeywordSymbol> symbols)
            {
                Symbols = symbols;
                Index = 0;
            }
            List<KeywordSymbol> Symbols;
            int Index;
            public KeywordSymbol Peek()
            {
                if (Index == Symbols.Count)
                    return KeywordSymbol.endOfStream;

                return Symbols[Index];
            }
            public KeywordSymbol Next()
            {
                if (Index == Symbols.Count)
                    return KeywordSymbol.endOfStream;

                return Symbols[Index++];
            }
        }

        static VoiceAction Parse(List<KeywordSymbol> symbols)
        {
            List<int> GetLaneIndexList(SymbolStreamAccessor acc)
            {
                if (acc.Peek() == KeywordSymbol.zone)
                    acc.Next();

                List<int> result = new List<int>();
                while (IsNumber(acc.Peek()))
                {
                    var laneIndex = acc.Next().ToNumber() - 1;
                    if (!result.Contains(laneIndex))
                        result.Add(laneIndex);
                }
                return result;
            }

            var accessor = new SymbolStreamAccessor(symbols);
            var firstSymbol = accessor.Peek();

            if (firstSymbol == KeywordSymbol.anfang || firstSymbol == KeywordSymbol.ende)
            {
                ZonesAction action = firstSymbol == KeywordSymbol.anfang ? (ZonesAction)new StartZonesAction() : new EndZonesAction();
                accessor.Next();
                action.LaneIndices = GetLaneIndexList(accessor);
                if (accessor.Next() != KeywordSymbol.endOfStream)
                    return new InvalidAction();
                return action;
            }
            else if (firstSymbol.IsDamageCause() || firstSymbol.IsDamageType() || firstSymbol == KeywordSymbol.zone || firstSymbol.IsNumber())
            {
                var action = new SetZonesDetailAction();

                void tryReadOneLineDetail()
                {
                    if (accessor.Peek().IsDamageCause())
                        action.DamageCause = accessor.Next();
                    else if (accessor.Peek().IsDamageType())
                        action.DamageType = accessor.Next();
                }

                tryReadOneLineDetail();
                tryReadOneLineDetail();
                action.LaneIndices = GetLaneIndexList(accessor);
                tryReadOneLineDetail();
                tryReadOneLineDetail();
                
                if (accessor.Peek() == KeywordSymbol.ende)
                {
                    action.ShouldEndZone = true;
                    accessor.Next();
                }

                if (accessor.Next() != KeywordSymbol.endOfStream
                    || action.LaneIndices.Count == 0
                    || (action.DamageCause == KeywordSymbol.invalid && action.DamageType == KeywordSymbol.invalid))
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
