using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductsService.IntegrationTests.Helpers;

public static class JsonOptions
{
    /// <summary>
    /// HttpClient's ReadFromJsonAsync defaults don't know the Api serializes
    /// enums as strings (configured in Program.cs), so tests need the same
    /// converter to deserialize responses.
    /// </summary>
    public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}
