using hydra.Config;
using hydra.Endpoints;
using hydra.Http;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, HydraJsonSerializerContext.Default);
});

var configProvider = await Helpers.CreateConfigProviderAsync();
builder.Services.AddSingleton(configProvider);
builder.Services.AddScoped<IHydraHttpContext, HydraHttpContext>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapGetEndpoint();

await app.RunAsync();

file static class Helpers
{
    public static async Task<IHydraConfigProvider> CreateConfigProviderAsync()
    {
        var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
        
        var fileStreamProviderLogger = loggerFactory.CreateLogger<FileStreamProvider>();
        var fileStreamProvider = new FileStreamProvider(fileStreamProviderLogger);
        
        var configFactoryLogger = loggerFactory.CreateLogger<JsonConfigFactory>();
        var configFactory = new JsonConfigFactory(configFactoryLogger, fileStreamProvider);
        
        var config = await configFactory.CreateConfigAsync();
        var configProvider = new HydraConfigProvider(config);
        return configProvider;
    }
}