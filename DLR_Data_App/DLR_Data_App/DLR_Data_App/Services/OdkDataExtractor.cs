using NCalc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace DLR_Data_App.Services
{
    /// <summary>
    /// Class representing an expression in odk expression format https://docs.opendatakit.org/form-logic/
    /// </summary>
    class OdkBooleanExpresion
    {
        private static readonly string[,] OdkReplacementStrings = new string[,]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="OdkBooleanExpresion"/> class, using the odkExpression
        /// </summary>
        /// <param name="odkExpression"><see cref="string"/> in odk format described in https://docs.opendatakit.org/form-logic/ </param>
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

        private Expression Expression;
        private object ExpressionLock = new object();


        /// <summary>
        /// Evaluates the expression using the given variables
        /// </summary>
        /// <param name="variables">Dictionary matching the variable name and its value</param>
        /// <returns>Result of expression when the given variables are used</returns>
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
                Expression.EvaluateParameter -= insertVariableValues;
                return result is bool b && b;
            }
        }
    }

    /// <summary>
    /// Struct representing a range with min and max and methods to check if a given number is in the range.
    /// </summary>
    struct OdkRange
    {
        public bool MinInclusive;
        public bool MaxInclusive;

        public float? Min;
        public float? Max;

        /// <summary>
        /// Initializes a new instance of the <see cref="OdkRange"/> struct, using the specified borders
        /// </summary>
        /// <param name="min">Minimum of the range</param>
        /// <param name="minInclusive">Determines if the minimum is part of the range</param>
        /// <param name="max">Maximum of the range</param>
        /// <param name="maxInclusive">Determines if the maximum is part of the range</param>
        public OdkRange(float min, bool minInclusive, float max, bool maxInclusive)
        {
            MinInclusive = minInclusive;
            MaxInclusive = maxInclusive;
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Checks if the given input is in the range repesented by this instance of <see cref="OdkRange"/>
        /// </summary>
        /// <param name="input">Input for which should be checked if it lies in the range</param>
        /// <returns><see cref="bool"/> indicating weather or not the input lies in the range</returns>
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

        /// <summary>
        /// Checks if the given input is in the range repesented by this instance of <see cref="OdkRange"/>
        /// </summary>
        /// <param name="input">Input for which should be checked if it lies in the range</param>
        /// <returns><see cref="bool"/> indicating weather or not the input lies in the range</returns>
        public bool IsValidIntegerInput(int input)
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

    /// <summary>
    /// Static clas providing methods to create instances of classes representing different odk structures
    /// </summary>
    static class OdkDataExtractor
    {
        /// <summary>
        /// Creates a <see cref="OdkRange"/> instance from a provided range string.
        /// </summary>
        /// <param name="jsonRangeString"><see cref="string"/> in json format, like produced from odk build in the range field</param>
        /// <returns><see cref="OdkRange"/> which represents a range</returns>
        public static OdkRange GetRangeFromJsonString(string jsonRangeString)
        {
            var range = new OdkRange();
            if (jsonRangeString == null || jsonRangeString.Equals("false", StringComparison.InvariantCultureIgnoreCase))
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

        /// <summary>
        /// Creates a <see cref="OdkBooleanExpresion"/> from a provided expression string.
        /// </summary>
        /// <param name="odkExpression"><see cref="string"/> representing an expression in format https://docs.opendatakit.org/form-logic/ </param>
        /// <returns><see cref="OdkBooleanExpresion"/> which represents the given expression</returns>
        public static OdkBooleanExpresion GetBooleanExpression(string odkExpression)
        {
            return new OdkBooleanExpresion(odkExpression);
        }

        /// <summary>
        /// Selects an english string from a JSON list.
        /// </summary>
        /// <param name="jsonList">JSON string, containing the wanted string in multiple languages</param>
        /// <param name="languageList">List of available languages</param>
        /// <returns>String in english language</returns>
        public static string GetEnglishStringFromJsonList(string jsonList, string languageList)
        {
            var currentLanguage = CultureInfo.GetCultureInfo("en-GB").EnglishName;
            string result = null;
            var temp = "0";

            try
            {
                // check which languages are available
                var languageObjects = JObject.Parse(languageList); // parse as object  
                foreach (var app in languageObjects)
                {
                    var key = app.Key;
                    var value = app.Value.ToString();

                    // if local language matches available language store key in result
                    if (!currentLanguage.Contains(value)) continue;
                    temp = key;
                    break;
                }

                // get string from json list in the correct language
                var translationObjects = JObject.Parse(jsonList); // parse as object  
                foreach (var app in languageObjects)
                {
                    var key = app.Key;
                    var value = app.Value.ToString();

                    // if key is found set matching string as result
                    if (key != temp) continue;
                    result = value;
                    break;
                }
                if (result == null)
                {
                    //just pick first available translation
                    var languagesEnumerator = languageObjects.GetEnumerator();
                    languagesEnumerator.MoveNext();
                    return languagesEnumerator.Current.Key;
                }
            }
            catch (Exception)
            {
                result = "Error in JSON file";
            }

            return result ?? "Unable to parse language from json";
        }

        public static string GetCurrentLanguageCodeFromJsonList(string languageList)
        {
            try
            {
                var currentLanguage = CultureInfo.CurrentUICulture.EnglishName;
                // check which languages are available
                var languageObjects = JObject.Parse(languageList); // parse as object 
                string languageCode = null;
                foreach (var app in languageObjects)
                {
                    var key = app.Key;
                    var value = app.Value.ToString();

                    // if local language matches available language store key in result
                    if (!currentLanguage.Contains(value)) continue;
                    languageCode = key;
                    break;
                }
                if (languageCode == null)
                {
                    //just pick first available translation
                    var languagesEnumerator = languageObjects.GetEnumerator();
                    languagesEnumerator.MoveNext();
                    return languagesEnumerator.Current.Key;
                }
                return languageCode;
            }
            catch (Exception)
            {
                return "Error in JSON file";
            }
        }

        /// <summary>
        /// Selects a string in the runtime systems language from a JSON list. 
        /// </summary>
        /// <param name="jsonList">JSON string, containing the wanted string in multiple languages</param>
        /// <param name="languageList">List of available languages</param>
        /// <returns>String in the runtime systems language</returns>
        public static string GetCurrentLanguageStringFromJsonList(string jsonList, string languageList)
        {
            // get current language
            var result = "Unable to parse language from json";
            var temp = "0";

            try
            {
                var languageCode = GetCurrentLanguageCodeFromJsonList(languageList);

                // get string from json list in the correct language
                var objects = JObject.Parse(jsonList); // parse as object  
                foreach (var app in objects)
                {
                    var key = app.Key;
                    var value = app.Value.ToString();

                    // if key is found set matching string as result
                    if (key != temp) continue;
                    result = value;
                    break;
                }
            }
            catch (Exception)
            {
                result = "Error in JSON file";
            }

            return result;
        }
    }
}
