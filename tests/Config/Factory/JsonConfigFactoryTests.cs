using hydra.Config;
using Microsoft.Extensions.Logging.Abstractions;

namespace hydra.tests.Config;

public class JsonConfigFactoryTests
{
    private const string Json = 
        """
        {
          "a": {
            "path1": "https://localhost:8888/path1",
            "path/2": "https://localhost:8888/path2"
          },
          "b": {
            "other": "https://localhost:9999/other"
          }
        }
        """;

    private static JsonConfigFactory CreateConfigFactory()
    {
        var fileStreamProvider = new TestFileStreamProvider(Json);
        var logger = NullLogger<JsonConfigFactory>.Instance;
        return new JsonConfigFactory(logger, fileStreamProvider);
    }
    
    [Test]
    public async Task Get_ComponentNotConfigured_ReturnsNullConfig()
    {
        // Arrange
        var factory = CreateConfigFactory();
        
        // Act
        var config = await factory.CreateConfigAsync();
        
        // Assert
        var component = config["not-configured"];
        await Assert.That(component).IsNull();
    }
    
    [Test]
    public async Task Get_ComponentConfigured_PathNotConfigured_ReturnsNullPath()
    {
        // Arrange
        var factory = CreateConfigFactory();
        
        // Act
        var config = await factory.CreateConfigAsync();
        
        // Assert
        var component = config["a"];
        await Assert.That(component).IsNotNull();

        var path = component["not-configured"];
        await Assert.That(path).IsNull();
    }
    
    [Test]
    public async Task Get_ComponentConfigured_PathConfigured_ReturnsRedirect()
    {
        // Arrange
        var factory = CreateConfigFactory();
        
        // Act
        var config = await factory.CreateConfigAsync();
        
        // Assert
        var component = config["a"];
        await Assert.That(component).IsNotNull();

        var redirect = component["path1"];
        await Assert.That(redirect).IsEqualTo("https://localhost:8888/path1");
    }
    
    [Test]
    public async Task Get_SegmentedPath_ReturnsRedirect()
    {
        // Arrange
        var factory = CreateConfigFactory();
        
        // Act
        var config = await factory.CreateConfigAsync();
        
        // Assert
        var component = config["a"];
        await Assert.That(component).IsNotNull();

        var redirect = component["path/2"];
        await Assert.That(redirect).IsEqualTo("https://localhost:8888/path2");
    }
}
