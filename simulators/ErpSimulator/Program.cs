using Microsoft.EntityFrameworkCore;
using SoapCore;
using ErpSimulator.Application;
using ErpSimulator.Persistence;
using ErpSimulator.Soap.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("SqlServer") ?? throw new InvalidOperationException("Connection string 'SqlServer' was not found.");

builder.Services.AddDbContext<TigerDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSoapCore().AddScoped<InventoryQueryService>().AddScoped<TigerErpService>().AddScoped<SalesOrderCommandService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<TigerDbContext>();
    await TigerDbSeeder.SeedAsync(context);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

((IEndpointRouteBuilder)app).UseSoapEndpoint<TigerErpService>(options =>
{
    options.Path = "/soap/erp.svc";
    options.SoapSerializer = SoapSerializer.XmlSerializer;
});

app.Run();
