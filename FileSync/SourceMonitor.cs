using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSync
{
    /// <summary>
    /// Source file monitor class
    /// </summary>
    internal class SourceMonitor : ISourceMonitor
    {
        private readonly ILogger<SourceMonitor> logger;
        private readonly string? sourcePath;
        private readonly string? destPath;
        private FileSystemWatcher watcher;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public SourceMonitor(ILogger<SourceMonitor> logger, IConfiguration configuration)
        {
            try
            {
                this.logger = logger;
                ConfigFileSystem config = new ConfigFileSystem(configuration);
                this.sourcePath = config.SourcePath;
                this.destPath = config.DestPath;
                if (this.destPath is null || sourcePath is null)
                {
                    throw new ArgumentException("Paths are null, check appsetting.json");
                }
                Startup();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

        }

      /// <summary>
      /// Startup for 
      /// </summary>
        public void Startup()
        {
            if (watcher is not null)
            {
                watcher.Dispose();
            }
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
            try
            {
                string newPath = Path.Combine(this.destPath, e.Name);
                logger.LogInformation($"Copy file {e.FullPath} to {newPath}");
                File.Copy(e.FullPath, newPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unable to delete file {e.FullPath}");
            }
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            try
            {
                string newPath = Path.Combine(this.destPath, e.Name);
                string OldPath = Path.Combine(this.destPath, e.OldName);
                logger.LogInformation($"Rename file {OldPath} to {newPath}");
                File.Move(OldPath, newPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unable to delete file {e.FullPath}");
            }
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                string path = Path.Combine(this.destPath, e.Name);
                logger.LogInformation($"Deleteing file {path}");
                File.Delete(path);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unable to delete file {e.FullPath}");
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                string newPath = Path.Combine(this.destPath, e.Name);
                logger.LogInformation($"Change file {e.FullPath} to {newPath}");
                File.Copy(e.FullPath, newPath, true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unable to delete file {e.FullPath}");
            }
        }
    }
}
