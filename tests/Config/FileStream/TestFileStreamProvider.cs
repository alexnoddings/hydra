using System.Text;
using hydra.Config;

namespace hydra.tests.Config;

public class TestFileStreamProvider : IFileStreamProvider
{
    private readonly string? _json;
    
    public TestFileStreamProvider(string? json)
    {
        _json = json;
    }

    public Stream? OpenFileStream(string path)
    {
        if (_json is null)
            return null;

        var bytes = Encoding.UTF8.GetBytes(_json);
        return new MemoryStream(bytes);
    }
}
