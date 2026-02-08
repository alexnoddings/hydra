namespace hydra.Config;

public class HydraConfigProvider : IHydraConfigProvider
{
    private readonly HydraConfig _config;

    public HydraConfigProvider(HydraConfig config)
    {
        _config = config;
    }
    
    public ValueTask<HydraConfig> GetConfigAsync() => new(_config);
}
