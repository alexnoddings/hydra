using System.Text.Json;
using System.Text.Json.Serialization;

namespace hydra.Config;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(Dictionary<string, Dictionary<string, string>>))]
internal partial class HydraJsonSerializerContext : JsonSerializerContext;
