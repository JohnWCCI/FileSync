
namespace FileSync
{
    public interface IFileSystemHealthCheck
    {
        Task<bool> IsHealthAsync(CancellationToken stoppingToken);
    }
}