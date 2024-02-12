using Microsoft.Extensions.Logging;

namespace FileSync
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration conf;
        private  ISourceMonitor _monitor = null!;
        private readonly IFileCheck fileCheck;
        private readonly IFileSystemHealthCheck health;


        public Worker(ILogger<Worker> logger, IConfiguration conf, ISourceMonitor monitor, IFileCheck fileCheck, IFileSystemHealthCheck health)
        {
            _logger = logger;
            this.conf = conf;
            ConfigFileSystem config = new ConfigFileSystem(conf);
            _logger.LogInformation(config.SourcePath);
            _logger.LogInformation(config.DestPath);
            _monitor = monitor;
            this.fileCheck = fileCheck;
            this.health = health;


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
               await fileCheck.ProcessAsync(stoppingToken);
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    }
                    bool result = await this.health.IsHealthAsync(stoppingToken);
                    if (!result)
                    {
                        await fileCheck.ProcessAsync(stoppingToken);
                        _monitor.Startup();

                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"This is a major problem");
            }
        }
    }
}
