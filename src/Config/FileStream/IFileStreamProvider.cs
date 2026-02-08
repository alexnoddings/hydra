namespace hydra.Config;

public interface IFileStreamProvider
{
    public Stream? OpenFileStream(string path);
}
