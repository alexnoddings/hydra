using hydra.Http;

namespace hydra.tests;

public class TestHttpContext : IHydraHttpContext
{
    public string? Host { get; }
    public string Path { get; }

    public TestHttpContext(string? hostComponent, string pathSlug)
    {
        Host = hostComponent;
        Path = pathSlug;
    }
}
