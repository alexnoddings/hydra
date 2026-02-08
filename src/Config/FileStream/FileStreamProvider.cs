namespace hydra.Config;

public class FileStreamProvider : IFileStreamProvider
{
    private readonly ILogger _logger;

    public FileStreamProvider(ILogger<FileStreamProvider> logger)
    {
        _logger = logger;
    }

    public Stream? OpenFileStream(string path)
    {
        _logger.LogInformation("Loading Hydra config from '{ConfigFilePath}'.", path);
        
        FileStream configFile;
        try
        {
            configFile = File.Open(path, FileMode.Open, FileAccess.Read);
        }
        catch (FileNotFoundException)
        {
            _logger.LogError("No '{ConfigFilePath}' Hydra config file found.", path);
            return null;
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Error opening Hydra config from '{ConfigFilePath}'.", path);
            throw;
        }

        return configFile;
    }
}
