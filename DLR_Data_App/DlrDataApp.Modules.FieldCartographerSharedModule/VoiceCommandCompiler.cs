using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.FieldCartographer.Shared.VoiceActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static DlrDataApp.Modules.Base.Shared.Services.FormattedStringSerializerHelper;

namespace DlrDataApp.Modules.FieldCartographer.Shared
{
    public static partial class VoiceCommandCompiler
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
        static KeywordSymbol[] DamageTypes;
        static KeywordSymbol[] DamageCauses;
        public static Dictionary<string, KeywordSymbol> KeywordStringToSymbol;
        public static Dictionary<string, List<string>> IdToVoiceCommands;
        public static Dictionary<string, FormattedString> IdToFormattedString;
        readonly static KeywordSymbol[] NumberSymbols;

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


        static VoiceCommandCompiler()
        {
            KeywordStringToSymbol = new Dictionary<string, KeywordSymbol>
            {
                { "anfang", KeywordSymbol.anfang },
                { "start", KeywordSymbol.anfang },
                { "stopp", KeywordSymbol.ende },
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
                { "[unk]", KeywordSymbol.unk },
                { "spur", KeywordSymbol.number0 },
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
            IdToVoiceCommands = new Dictionary<string, List<string>>
            {
                { "SandLens", new List<string>{ "sand" } },
                { "Compaction", new List<string>{ "verdichtung" } },
                { "Headland", new List<string>{ "wende" } },
                { "Dome", new List<string>{ "kuppe" } },
                { "Slope", new List<string>{ "hang" } },
                { "ForestEdge", new List<string>{ "wald" } },
                { "DryStress", new List<string>{ "trocken" } },
                { "WaterLogging", new List<string>{ "nass", "nässe" } },
                { "GameMouseDamage", new List<string>{ "maus", "mäuse", "wild" } },
                { "Clay", new List<string>{ "ton" } }
            };
            IdToFormattedString = new Dictionary<string, FormattedString>
            {
                { "SandLens", StringWithAnnotationsToFormattedString("*Sand*linse") },
                { "Compaction", StringWithAnnotationsToFormattedString("*Verdichtung*") },
                { "Headland", StringWithAnnotationsToFormattedString("Vorge*wende*") },
                { "Dome", StringWithAnnotationsToFormattedString("*Kuppe*") },
                { "Slope", StringWithAnnotationsToFormattedString("*Hang*") },
                { "ForestEdge", StringWithAnnotationsToFormattedString("*Wald*rand") },
                { "DryStress", StringWithAnnotationsToFormattedString("*Trocken*stress") },
                { "WaterLogging", StringWithAnnotationsToFormattedString("*Nass*stelle") },
                { "GameMouseDamage", StringWithAnnotationsToFormattedString("*Mäuse*fraß\\n*Wild*schaden") },
                { "Clay", StringWithAnnotationsToFormattedString("*Ton*") }
            };
            NumberSymbols = new[]
            {
                KeywordSymbol.number0, KeywordSymbol.number1, KeywordSymbol.number2, KeywordSymbol.number3, KeywordSymbol.number4,
                KeywordSymbol.number5, KeywordSymbol.number6, KeywordSymbol.number7, KeywordSymbol.number8, KeywordSymbol.number9
            };

            DamageTypes = new[] { KeywordSymbol.gering, KeywordSymbol.mittel, KeywordSymbol.hoch };
            DamageTypes = IdToVoiceCommands.SelectMany(kv => kv.Value).Select(v => KeywordStringToSymbol[v]).Distinct().ToArray();
        }

        public static VoiceAction Compile(List<string> recognizedKeywords)
        {
            try
            {
                return Parse(Scan(recognizedKeywords));
            }
            catch (Exception)
            {
                return new InvalidAction();
            }
        }
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
                else if (accessor.Peek() == KeywordSymbol.anfang)
                {
                    action.ShouldStartZone = true;
                    accessor.Next();
                }


                if (accessor.Next() != KeywordSymbol.endOfStream
                    || action.LaneIndices.Count == 0
                    || (action.DamageCause == KeywordSymbol.invalid && action.DamageType == KeywordSymbol.invalid && !action.ShouldEndZone && !action.ShouldStartZone))
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
