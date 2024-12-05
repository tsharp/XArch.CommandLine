using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace XArch.CommandLine
{
    internal static class Serialization
    {
        public static string SerializeToYaml<T>(this T obj)
            where T : class, new()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance) // Optional: Use snake case
                .Build();

            return serializer.Serialize(obj, obj!.GetType());
        }

        public static T DeserializeFromYaml<T>(this string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance) // Optional: Use snake case
                .IgnoreUnmatchedProperties() // Ignore unknown properties
                .Build();

            return deserializer.Deserialize<T>(yaml);
        }
    }
}
