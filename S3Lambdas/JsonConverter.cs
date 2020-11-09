using System.Text.Json;

namespace S3Lambdas
{
    public static class JsonConverter
    {
        public static string Serialize<T>(T objectToSerialize)
        {
            return JsonSerializer.Serialize<T>(objectToSerialize, new JsonSerializerOptions { WriteIndented = true });
        }

        public static T Deserialize<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static string SerializeWithCamelCase<T>(T objectToSerialize)
        {
            return JsonSerializer.Serialize<T>(objectToSerialize, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
        }
    }
}
