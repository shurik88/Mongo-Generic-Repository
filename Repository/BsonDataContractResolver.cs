using System;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Repository
{
    public class BsonDataContractResolver : DefaultContractResolver
    {
        public static readonly BsonDataContractResolver Instance = new BsonDataContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (Attribute.IsDefined(member, typeof(BsonElementAttribute)))
            {
                var attr = member.GetCustomAttribute<BsonElementAttribute>();
                property.PropertyName = attr.ElementName;
            }
            else if (Attribute.IsDefined(member, typeof(BsonIdAttribute)))
            {
                property.PropertyName = "_id";
            }

            return property;
        }
    }
}
