using hydra.Config;

namespace hydra.tests.Config;

public class TestConfigProvider : IHydraConfigProvider
{
    private readonly HydraConfig _config;

    public TestConfigProvider(HydraConfig config)
    {
        _config = config;
    }
    
    public ValueTask<HydraConfig> GetConfigAsync() => new(_config);

    public static TestConfigProvider Create(string component, string path, string redirect)
    {
        var componentDict = new Dictionary<string, string>
        {
            { path, redirect }
        };
        var configComponent = new HydraComponent(componentDict);

        var configDict = new Dictionary<string, HydraComponent>()
        {
            { component, configComponent }
        };
        var config = new HydraConfig(configDict);
        
        return new TestConfigProvider(config);
    }
}
