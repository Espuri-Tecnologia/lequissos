using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Reflection;

public class IgnoreEmptyEnumerablesResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
        {
            property.ShouldSerialize = instance =>
            {
                var enumerable = property.ValueProvider.GetValue(instance) as IEnumerable;
                return enumerable != null && enumerable.GetEnumerator().MoveNext();
            };
        }

        return property;
    }
}
