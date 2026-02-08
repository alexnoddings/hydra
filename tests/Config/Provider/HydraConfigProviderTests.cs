using hydra.Config;

namespace hydra.tests.Config.Provider;

public class HydraConfigProviderTests
{
    [Test]
    public async Task Get_ReturnsCtorInstance()
    {
        // Arrange
        var config = HydraConfig.Empty;
        var provider = new HydraConfigProvider(config);
        
        // Act
        var providerConfig = await provider.GetConfigAsync();
        
        // Assert
        await Assert.That(providerConfig).IsSameReferenceAs(config);
    }
}
