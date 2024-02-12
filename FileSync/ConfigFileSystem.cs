using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// "FileSystem": {
    /// "SourceFolder": "C:\\SourceTest",
    /// "DestFolder": "C:\\DestTest"
    /// </remarks>
    public class ConfigFileSystem
    {
        private readonly string? _SourcePath;
        private readonly string? _DestPath;
        public ConfigFileSystem(IConfiguration config) {
            IConfigurationSection section = config.GetSection("FileSystem");
            _DestPath = section.GetValue<string>("DestFolder");
            _SourcePath = section.GetValue<string>("SourceFolder");
        }

        public string? SourcePath => _SourcePath;

        public string? DestPath => _DestPath;
    }
}
