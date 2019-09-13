using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
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

            return property;
        }
    }
}
