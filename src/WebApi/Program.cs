using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add OpenTelemetry exporter
builder.Services.AddOpenTelemetry().WithTracing(b => 
    b.SetResourceBuilder(
        ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
        .AddAspNetCoreInstrumentation(o => o.Filter = c =>
        {
            if (c.Request.Path.HasValue)
            {
                return !c.Request.Path.Value.StartsWith("/swagger");
            }
            return true;
        })
        .AddOtlpExporter(o =>o.Endpoint = new Uri("http://localhost:4317")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();