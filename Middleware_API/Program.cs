using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HTTP clients for each service
builder.Services.AddHttpClient("CRMApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:CRMApiUrl"] ?? "https://localhost:7069");
});

builder.Services.AddHttpClient("StoreTrackApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:StoreTrackApiUrl"] ?? "https://localhost:7197");
});

builder.Services.AddHttpClient("WarehouseFlowApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:WarehouseFlowApiUrl"] ?? "https://localhost:7227");
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
