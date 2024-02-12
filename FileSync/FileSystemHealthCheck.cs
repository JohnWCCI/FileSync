using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
namespace FileSync
{
    public class FileSystemHealthCheck : IFileSystemHealthCheck
    {
        private readonly ILogger<FileSystemHealthCheck> logger;
        private readonly IConfiguration configuration;
        private readonly ConfigFileSystem fileSystem;
        public FileSystemHealthCheck(ILogger<FileSystemHealthCheck> logger, IConfiguration configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            fileSystem = new ConfigFileSystem(configuration);
        }

        public async Task<bool> IsHealthAsync(CancellationToken stoppingToken)
        {
            bool result = false;
            StreamWriter fileHandle;
            string temFile = Path.GetTempFileName();
            temFile = Path.GetFileName(temFile);
            string sourceFile = Path.Combine(fileSystem.SourcePath, temFile);
            string destFile = Path.Combine(fileSystem.DestPath, temFile);
            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {   
                    fileHandle = File.CreateText(sourceFile);
                    await fileHandle.WriteAsync("Testing HealthCheck");
                    fileHandle.Close();
                    await Task.Delay(5000, stoppingToken);
                    if(!File.Exists(destFile))
                    {
                        throw new Exception("FileWatch restart");
                    }
                   
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            finally
            {
                if (File.Exists(sourceFile))
                {
                    File.Delete(sourceFile);
                    await Task.Delay(1000, stoppingToken);
                }
                if (File.Exists(destFile))
                {
                    Exception ex = new Exception($"{destFile} still exist restarting watch");
                    logger.LogError(ex, ex.Message);
                    result = false;
                    await Task.Delay(20, stoppingToken);
                }

            }
            return result;
        }
    }
}
