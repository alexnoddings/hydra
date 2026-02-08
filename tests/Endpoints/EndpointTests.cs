using hydra.Endpoints;
using hydra.tests.Config;
using Microsoft.AspNetCore.Http.HttpResults;

namespace hydra.tests;

public class GetEndpointTests
{
    private const string Component = "component";
    private const string Path = "path"; 
    private const string Redirect = "https://redirect:8080/path";
    
    private static TestConfigProvider TestConfigProviderInstance => TestConfigProvider.Create(Component, Path, Redirect);
    
    [Test]
    public async Task Get_NoComponent_ReturnsBadRequest()
    {
        // Arrange
        var httpContext = new TestHttpContext(null, Path);
        var configProvider = TestConfigProviderInstance;
        
        // Act
        var result = await GetEndpoint.ExecuteAsync(configProvider, httpContext);
        
        // Assert
        await Assert.That(result.Result).IsTypeOf<BadRequest>();
    }
    
    [Test]
    public async Task Get_NotConfiguredComponent_ReturnsNotFound()
    {
        // Arrange
        var httpContext = new TestHttpContext("not-configured", Path);
        var configProvider = TestConfigProviderInstance;
        
        // Act
        var result = await GetEndpoint.ExecuteAsync(configProvider, httpContext);
        
        // Assert
        await Assert.That(result.Result).IsTypeOf<NotFound>();
    }
    
    [Test]
    public async Task Get_NotConfiguredPathSlug_ReturnsNotFound()
    {
        // Arrange
        var httpContext = new TestHttpContext(Component, "not-configured");
        var configProvider = TestConfigProviderInstance;
        
        // Act
        var result = await GetEndpoint.ExecuteAsync(configProvider, httpContext);
        
        // Assert
        await Assert.That(result.Result).IsTypeOf<NotFound>();
    }
    
    [Test]
    public async Task Get_Configured_ReturnsRedirect()
    {
        // Arrange
        var httpContext = new TestHttpContext(Component, Path);
        var configProvider = TestConfigProviderInstance;
        
        // Act
        var result = await GetEndpoint.ExecuteAsync(configProvider, httpContext);
        
        // Assert
        var redirect = await Assert.That(result.Result).IsTypeOf<RedirectHttpResult>();
        await Assert.That(redirect!.Url).IsEqualTo(Redirect);
    }
}
