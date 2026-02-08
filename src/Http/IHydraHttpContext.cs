namespace hydra.Http;

public interface IHydraHttpContext
{
    public string? Host { get; }
    public string Path { get; }
}
