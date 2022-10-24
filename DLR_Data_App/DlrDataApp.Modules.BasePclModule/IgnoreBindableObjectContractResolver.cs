using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

using static DlrDataApp.Modules.Base.Shared.Helpers;

namespace DlrDataApp.Modules.Base.Shared
{

    class FormattedStringConverter : JsonConverter<FormattedString>
    {
        public static readonly FormattedStringConverter Instance = new FormattedStringConverter();

        public override FormattedString ReadJson(JsonReader reader, Type objectType, FormattedString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return StringWithAnnotationsToFormattedString((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, FormattedString value, JsonSerializer serializer)
        {
            writer.WriteValue(FormattedStringToAnnotatedString(value));
        }
    }
    /// <summary>
    /// Child of <see cref="DefaultContractResolver"/> which, when used, will prevent the serialization of elements where declaring type is a <see cref="BindableObject"/>.
    /// </summary>
    class IgnoreBindableObjectContractResolver : DefaultContractResolver
    {
        public static readonly IgnoreBindableObjectContractResolver Instance = new IgnoreBindableObjectContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType == typeof(BindableObject))
                property.ShouldSerialize = instance => false;
            else
                property.ShouldSerialize = instance => true;
            
            if (property.PropertyType == typeof(FormattedString))
                property.Converter = FormattedStringConverter.Instance;

            return property;
        }
    }
}
