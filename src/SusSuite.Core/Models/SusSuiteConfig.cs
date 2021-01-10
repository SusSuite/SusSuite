using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SusSuite.Core.Models
{
    public class SusSuiteConfig
    {
        public string ServerName { get; set; }
        public string ServerColor { get; set; }
    }

    public class SusSuiteConfigPropertyConverter : JsonConverter<SusSuiteConfig>
    {
        public override SusSuiteConfig Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            // Don't pass in options when recursively calling Deserialize.
            var susSuiteConfig = JsonSerializer.Deserialize<SusSuiteConfig>(ref reader);

            var rx = new Regex(@"\[[0-9a-fA-F]{8}\]");

            if (!rx.IsMatch(susSuiteConfig.ServerColor)) throw new JsonException("ServerColor is not in proper format.");

            return susSuiteConfig;
        }

        public override void Write(Utf8JsonWriter writer, SusSuiteConfig susSuiteConfig, JsonSerializerOptions options)
        {
            susSuiteConfig.ServerName = "SusSuiteServer";
            susSuiteConfig.ServerColor = "[ffaabbff]";
            // Don't pass in options when recursively calling Serialize.
            JsonSerializer.Serialize(writer, susSuiteConfig);
        }
    }
}
