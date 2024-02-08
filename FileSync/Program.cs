using FileSync;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ISourceMonitor, SourceMonitor>();
builder.Services.AddTransient<IFileCheck,FileCheck>();
var host = builder.Build();
host.Run();
