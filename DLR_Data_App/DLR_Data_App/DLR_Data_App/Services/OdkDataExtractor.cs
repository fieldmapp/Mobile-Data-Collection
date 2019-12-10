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
        public OdkBooleanExpresion(string odkExpression, List<string> fieldNames)
        {
            //make expression compatible with ncalc, found mostly by trial and error
            odkExpression = odkExpression.Replace("mod", "%");
            odkExpression = odkExpression.Replace("div", "/");
            odkExpression = odkExpression.Replace("true()", "true");
            odkExpression = odkExpression.Replace("false()", "false");
            odkExpression = odkExpression.Replace("sin(", "Sin(");
            odkExpression = odkExpression.Replace("cos(", "Cos(");
            foreach (var fieldName in fieldNames)
            {
                if (fieldName != Database.MakeValidSqlName(fieldName))
                    throw new ArgumentException($"Odk field name {fieldName} is not safe");

                odkExpression = odkExpression.Replace("${" + fieldName + "}", fieldName);
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
        }

        Expression Expression;
        object ExpressionLock = new object();

        bool Evaluate(Dictionary<string,string> variables)
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

        public static OdkBooleanExpresion GetBooleanExpression(string odkExpression, List<string> fieldNames)
        {
            return new OdkBooleanExpresion(odkExpression, fieldNames);
        }
    }
}
