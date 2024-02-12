using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileSync
{
    public class FileCheck : IFileCheck
    {
        private readonly string? sourcePath;
        private readonly string? destPath;
        private readonly ILogger<FileCheck> logger;
        public FileCheck(ILogger<FileCheck> logger, IConfiguration configuration)
        {
            this.logger = logger;
            ConfigFileSystem config = new ConfigFileSystem(configuration);
            this.sourcePath = config.SourcePath;
            this.destPath = config.DestPath; 
        }
        public async Task ProcessAsync(CancellationToken stoppingToken)
        {
            List<FileModel> files = new List<FileModel>();
            foreach(string file in Directory.GetFiles(this.sourcePath))
            {
                if (File.Exists(file) && !stoppingToken.IsCancellationRequested)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    FileModel model = new FileModel();
                    model.Name = fileInfo.Name;
                    model.LastDate = fileInfo.LastWriteTime;
                    model.Size = fileInfo.Length;
                    files.Add(model);
                    await Task.Delay(200);
                }
            }
           await CheckFilesAsync(files, stoppingToken);
           await Task.Delay(1000);
        }

        private async Task CheckFilesAsync(List<FileModel> sourceFiles, CancellationToken stoppingToken)
        {
            try
            {
                DeleteDestFiles();
                foreach (FileModel file in sourceFiles)
                {
                    Path.Combine(this.destPath, file.Name);
                    string destFile = Path.Combine(this.destPath, file.Name);
                    string sourceFile = Path.Combine(this.sourcePath, file.Name);
                    if (File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, destFile, true);
                    }
                    await Task.Delay(200);
                }

            }
            catch(Exception ex) 
            {
                logger.LogError(ex, ex.Message);
            }
            
        }

            private void DeleteDestFiles()
        {
            List<FileModel> files = new List<FileModel>();
            foreach (string file in Directory.GetFiles(this.destPath))
            {
                if (File.Exists(file))
                {
                   File.Delete(file);
                }
            }
           
        }

    }
}
