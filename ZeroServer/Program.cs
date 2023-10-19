using NetMQ;
using NetMQ.Sockets;
using ZeroServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<SocketService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/runserver", () =>
{
    using (var server = new ResponseSocket())
    {
        server.Bind("tcp://*:5555");
        while (true)
        {
            var message = server.ReceiveFrameString();
            Console.WriteLine("Received {0}", message);
            // processing the request
            Thread.Sleep(100);
            Console.WriteLine("Sending World");
            server.SendFrame("World");
        }
    }
})
.WithName("RunServer")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
