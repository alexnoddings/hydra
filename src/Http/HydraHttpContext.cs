namespace hydra.Http;

public class HydraHttpContext : IHydraHttpContext
{
    private readonly IHttpContextAccessor _accessor;
    
    public HydraHttpContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private HttpContext HttpContextOrThrow => 
        _accessor.HttpContext 
        ?? throw new InvalidOperationException("No active HttpContext.");

    public string? Host
    {
        get
        {
            var httpContext = HttpContextOrThrow;
            var host = httpContext.Request.Host;
            if (!host.HasValue)
                return null;

            var component = GetComponentFromHost(host.Host); 
            return component;
        }
    }

    public string Path
    {
        get
        {
            var httpContext = HttpContextOrThrow;
            string path = httpContext.Request.Path;
            if (path.StartsWith('/'))
                return path[1..];

            return path;
        }
    }
    
    private static string GetComponentFromHost(string host)
    {
        for (var i = 0; i < host.Length; i++)
        {
            if (host[i] == '.')
                return host[..i];
        }

        return host;
    }
}
