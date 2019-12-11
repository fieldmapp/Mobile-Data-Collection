using NCalc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DLR_Data_App.Services
{
    class OdkBooleanExpresion
    {
        public static readonly string[,] OdkReplacementStrings = new string[,]
        {
            { "mod", "%" },
            { "div", "/" },
            { "true()", "true" },
            { "false()", "false" },
            { "sin(", "Sin(" },
            { "cos(", "Cos(" },
            { "tan(", "Tan(" },
            { "asin(", "Asin(" },
            { "acos(", "Acos(" },
            { "atan(", "Atan(" },
            { "abs(", "Abs(" },
            { "log(","Log(" },
            { "log10(", "Log10(" },
            { "sqrt(", "Sqrt(" },
            { "round(", "Round(" },
            { "int(", "Truncate(" },
            { "pow(", "Pow(" },
            { "exp(", "Exp(" },
            { "exp10(", "Pow(10," },
            { "${", string.Empty },
            { "}", string.Empty },
        };
        public OdkBooleanExpresion(string odkExpression)
        {
            int replacementStringCount = OdkReplacementStrings.GetLength(0);
            for (int i = 0; i < replacementStringCount; i++)
            {
                odkExpression = odkExpression.Replace(OdkReplacementStrings[i, 0], OdkReplacementStrings[i, 1]);
            }
            Expression = new Expression(odkExpression);
            Expression.EvaluateFunction += Expression_EvaluateFunction;
        }

        private void Expression_EvaluateFunction(string name, FunctionArgs args)
        {
            if (name == "boolean-from-string")
            {
                var childValue = args.Parameters[0].Evaluate().ToString();
                args.Result = childValue == "true" || childValue == "1";
            }
            else if (name == "random")
            {
                args.Result = App.RandomProvider.Next();
            }
            else if (name == "pi")
            {
                args.Result = Math.PI;
            }
        }

        Expression Expression;
        object ExpressionLock = new object();

        public bool Evaluate(Dictionary<string,string> variables)
        {
            void insertVariableValues(string name, ParameterArgs args)
            {
                if (variables.TryGetValue(name, out var variableValue))
                {
                    args.Result = variableValue;
                }
            }
            lock (ExpressionLock)
            {
                Expression.EvaluateParameter += insertVariableValues;
                var result = Expression.Evaluate();
                return result is bool b && b;
            }
        }
    }

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

        public bool IsValidDecimalInput(float input)
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

        public bool IsValidIntegerInput(float input)
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

        public static OdkBooleanExpresion GetBooleanExpression(string odkExpression)
        {
            return new OdkBooleanExpresion(odkExpression);
        }
    }
}
