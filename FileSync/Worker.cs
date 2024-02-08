using Microsoft.Extensions.Logging;

namespace FileSync
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISourceMonitor _monitor = null!;
        public Worker(ILogger<Worker> logger, IConfiguration conf, ISourceMonitor monitor, IFileCheck fileCheck)
        {
            _logger = logger;
            ConfigFileSystem config = new ConfigFileSystem(conf);
            _logger.LogInformation(config.SourcePath);
            _logger.LogInformation(config.DestPath);
            _monitor = monitor;
            fileCheck.Process();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    }
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"This is a major problem");
            }
        }
    }
}
