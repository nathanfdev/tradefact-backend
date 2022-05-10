using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tradefact.Http.Utilities
{
    public static class HttpResponseExtensions
    {
        public static T ContentAsType<T>(this HttpResponseMessage response)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            return string.IsNullOrEmpty(data) ?
                            default(T) :
                            JsonSerializer.Deserialize<T>(data);
        }

        public static dynamic ContentAsDynamic(this HttpResponseMessage response)
        {
            var data = response.Content.ReadAsStringAsync().Result;
            JsonDocument jdoc = JsonDocument.Parse(data);
            return jdoc;
        }

        public static string ContentAsString(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
