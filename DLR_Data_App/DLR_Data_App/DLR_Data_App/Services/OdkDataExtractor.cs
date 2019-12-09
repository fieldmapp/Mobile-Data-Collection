using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DLR_Data_App.Services
{
    struct OdkRange
    {
        public bool MinInclusive;
        public bool MaxInclusive;

        public float? Min;
        public float? Max;

        public OdkRange(float? min, bool minInclusive, float? max, bool maxInclusive)
        {
            MinInclusive = minInclusive;
            MaxInclusive = maxInclusive;
            Min = min;
            Max = max;
        }

        bool IsValidDecimalInput(float input)
        {
            if (Min.HasValue)
            {
                if (MinInclusive && Min > input)
                    return false;
                if (!MinInclusive && Min >= input)
                    return false;
            }
            if (Max.HasValue)
            {
                if (MaxInclusive && Max < input)
                    return false;
                if (!MaxInclusive && Max <= input)
                    return false;
            }
            return true;
        }

        bool IsValidIntegerInput(float input)
        {
            if (Min.HasValue)
            {
                if (MinInclusive && Min > input)
                    return false;
                if (!MinInclusive && Min >= input)
                    return false;
            }
            if (Max.HasValue)
            {
                if (MaxInclusive && Max < input)
                    return false;
                if (!MaxInclusive && Max <= input)
                    return false;
            }
            return true;
        }

        bool IsValidIntegerInput(int input)
        {
            if (Min.HasValue)
            {
                var minInt = Math.Round(Min.Value);
                if (MinInclusive && minInt > input)
                    return false;
                if (!MinInclusive && minInt >= input)
                    return false;
            }
            if (Max.HasValue)
            {
                var maxInt = Math.Round(Max.Value);
                if (MaxInclusive && maxInt < input)
                    return false;
                if (!MaxInclusive && maxInt <= input)
                    return false;
            }
            return true;
        }
    }

    static class OdkDataExtractor
    {
        public static OdkRange GetRangeFromJsonString(string jsonRangeString)
        {
            var range = new OdkRange();
            if (jsonRangeString.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                return range;

            var jsonRangeObject = JObject.Parse(jsonRangeString);
            foreach (var rangeDetail in jsonRangeObject)
            {
                var detailValue = rangeDetail.Value.ToString();
                switch (rangeDetail.Key)
                {
                    case "min":
                        if (!string.IsNullOrWhiteSpace(detailValue))
                            range.Min = Convert.ToInt32(detailValue);
                        break;
                    case "max":
                        if (!string.IsNullOrWhiteSpace(detailValue))
                            range.Max = Convert.ToInt32(detailValue);
                        break;
                    case "minInclusive":
                        range.MinInclusive = Convert.ToBoolean(detailValue);
                        break;
                    case "maxInclusive":
                        range.MaxInclusive = Convert.ToBoolean(detailValue);
                        break;
                }
            }
            return range;
        }
    }
}
