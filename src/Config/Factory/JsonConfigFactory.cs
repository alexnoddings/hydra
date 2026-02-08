using System.Text.Json;

namespace hydra.Config;

public class JsonConfigFactory : IHydraConfigFactory
{
    private readonly ILogger _logger;
    private readonly IFileStreamProvider _fileStreamProvider;

    public JsonConfigFactory(
        ILogger<JsonConfigFactory> logger,
        IFileStreamProvider fileStreamProvider
    )
    {
        _fileStreamProvider = fileStreamProvider;
        _logger = logger;
    }

    public async ValueTask<HydraConfig> CreateConfigAsync()
    {
        var configFileStream = _fileStreamProvider.OpenFileStream("./hydra.json");
        if (configFileStream is null)
            return HydraConfig.Empty;

        Dictionary<string, Dictionary<string, string>>? fileConfig;
        try
        {
            fileConfig = await JsonSerializer.DeserializeAsync(
                configFileStream,
                HydraJsonSerializerContext.Default.DictionaryStringDictionaryStringString
            );
            if (fileConfig is null)
            {
                _logger.LogWarning("Could not deserialise config.");
                return HydraConfig.Empty;
            }
        }
        finally
        {
            await configFileStream.DisposeAsync();
        }

        var configDict = new Dictionary<string, HydraComponent>();
        foreach (var component in fileConfig.Keys)
        {
            var value = fileConfig[component];
            configDict[component] = new HydraComponent(value);
        }

        return new HydraConfig(configDict);
    }
}
