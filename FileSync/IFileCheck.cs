namespace FileSync
{
    public interface IFileCheck
    {
        Task ProcessAsync(CancellationToken stoppingToken);
    }
}