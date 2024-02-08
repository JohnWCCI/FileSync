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
        public void Process()
        {
            List<FileModel> files = new List<FileModel>();
            foreach(string file in Directory.GetFiles(this.sourcePath))
            {
                if (File.Exists(file))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    FileModel model = new FileModel();
                    model.Name = fileInfo.Name;
                    model.LastDate = fileInfo.LastWriteTime;
                    model.Size = fileInfo.Length;
                    files.Add(model);
                }
            }
        }

        private void CheckFiles(List<FileModel> files)
        {
            
        }

    }
}
