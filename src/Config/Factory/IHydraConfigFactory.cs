namespace hydra.Config;

public interface IHydraConfigFactory
{
    public ValueTask<HydraConfig> CreateConfigAsync();
}
