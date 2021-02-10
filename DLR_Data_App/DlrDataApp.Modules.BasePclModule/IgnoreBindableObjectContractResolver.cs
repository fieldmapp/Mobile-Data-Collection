using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.SharedModule
{
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

            return property;
        }
    }
}
