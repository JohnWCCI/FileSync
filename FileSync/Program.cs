using FileSync;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ISourceMonitor, SourceMonitor>();
builder.Services.AddTransient<IFileCheck,FileCheck>();
builder.Services.AddSingleton<IFileSystemHealthCheck, FileSystemHealthCheck>();
var host = builder.Build();
host.Run();
