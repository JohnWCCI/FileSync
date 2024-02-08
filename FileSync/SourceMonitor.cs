using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSync
{
    internal class SourceMonitor : ISourceMonitor
    {
        private readonly ILogger<SourceMonitor> logger;
        private readonly string? sourcePath;
        private readonly string? destPath;
        private readonly FileSystemWatcher watcher;
        public SourceMonitor(ILogger<SourceMonitor> logger, IConfiguration configuration) {
            this.logger = logger;
            ConfigFileSystem config = new ConfigFileSystem(configuration);
            this.sourcePath = config.SourcePath;
            this.destPath = config.DestPath;
            watcher = new FileSystemWatcher(this.sourcePath);
            watcher.IncludeSubdirectories = true;
            watcher.Changed += Watcher_Changed;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += Watcher_Renamed;
            watcher.Created += Watcher_Created; 
            watcher.EnableRaisingEvents = true;

        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string newPath = Path.Combine(this.destPath, e.Name); 
            logger.LogInformation($"Copy file {e.FullPath} to {newPath}");
            File.Copy(e.FullPath, newPath);
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string newPath = Path.Combine(this.destPath, e.Name);
            string OldPath = Path.Combine(this.destPath, e.OldName);
            logger.LogInformation($"Rename file {OldPath} to {newPath}");
            File.Move(OldPath,newPath);
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                string path = Path.Combine(this.destPath, e.Name);
                logger.LogInformation($"Deleteing file {path}");
                File.Delete(path);       
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Unable to delete file {e.FullPath}");
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string newPath = Path.Combine(this.destPath, e.Name);
            logger.LogInformation($"Change file {e.FullPath} to {newPath}");
            File.Copy(e.FullPath, newPath,true);
        }
    }
}
