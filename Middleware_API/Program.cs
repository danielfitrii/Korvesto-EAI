using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure HTTP Clients for other APIs
builder.Services.AddHttpClient("POSApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:POSApiUrl"] ?? "https://localhost:7001");
});

builder.Services.AddHttpClient("WarehouseApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:WarehouseApiUrl"] ?? "https://localhost:7002");
});

builder.Services.AddHttpClient("CRMApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:CRMApiUrl"] ?? "https://localhost:7003");
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
