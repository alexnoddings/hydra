namespace hydra.Config;

public interface IHydraConfigProvider
{
    public ValueTask<HydraConfig> GetConfigAsync();
}
