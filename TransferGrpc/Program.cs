using TransferGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.

// Add services to the container.
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(9090); // Puerto para gRPC HTTP
    
});

var app = builder.Build();


app.MapGrpcService<MediaService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
