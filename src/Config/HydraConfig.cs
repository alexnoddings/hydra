using System.Collections.Immutable;

namespace hydra.Config;

public class HydraConfig
{
    private readonly ImmutableDictionary<string, HydraComponent> _components;

    public static HydraConfig Empty { get; } = new(new Dictionary<string, HydraComponent>());
    
    public HydraConfig(IDictionary<string, HydraComponent> components)
    {
        _components = components.ToImmutableDictionary();
    }

    public HydraComponent? this[string component] => CollectionExtensions.GetValueOrDefault(_components, component);
}

public class HydraComponent
{
    private readonly ImmutableDictionary<string, string> _paths;

    public HydraComponent(IDictionary<string, string> paths)
    {
        _paths = paths.ToImmutableDictionary();
    }

    public string? this[string path] => CollectionExtensions.GetValueOrDefault(_paths, path);
}
