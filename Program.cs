using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddLogging(c =>
{
    c.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.MapGet("Send", async () =>
{
    var traceparent = Activity.Current!.Id;
    app.Logger.LogInformation($"Send to Binding. Traceparent: {traceparent}");

    var bindingData = new
    {
        Operation = "post",
        Metadata = new
        {
            Path = "/Receive",
        },
    };

    var request = new HttpRequestMessage
    {
        RequestUri = new Uri("http://127.0.0.1:3500/v1.0/bindings/test"),
        Method = HttpMethod.Post,
        Content = new StringContent(JsonSerializer.Serialize(bindingData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), Encoding.UTF8, MediaTypeNames.Application.Json),
    };

    request.Headers.Add("Traceparent", traceparent);

    HttpClient client = new HttpClient();
    var response = await client.SendAsync(request);
    var body = await response.Content.ReadAsStringAsync();

    return $"Sent Traceparent: {traceparent}\nReceived {body}\n";
});

app.MapPost("Receive", ([FromHeader] string? traceparent, [FromHeader] string? tracestate) =>
{
    app.Logger.LogInformation($"Received. Traceparent: {traceparent}, Tracestate: {tracestate}");
    return $"Traceparent: {traceparent}, Tracestate: {tracestate}";
});

app.Run();
