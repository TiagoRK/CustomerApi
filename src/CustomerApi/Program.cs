using CustomerApi.Extensions;
using CustomerApi.Infraestructure.External.IOC;
using CustomerApi.Infrastructure.IOC;
using CustomerApi.Web.Configurations;
using CustomerApi.Web.Filters;
using CustomerApi.Web.Middlewares;
using Serilog;
using Serilog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGlobalization(builder.Configuration);

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");

builder.AddLoggerConfigs();

var appLogger = new SerilogLoggerFactory(logger)
    .CreateLogger<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLocalizationConfigs();
builder.Services.AddSwaggerGen(options =>
{
  options.OperationFilter<IdempotencySwaggerOperationFilter>();
});

builder.Services.AddInfrastructureServices(builder.Configuration, appLogger);
builder.Services.AddExternalInfrastructureServices(builder.Configuration, appLogger);
builder.Services.AddServices(builder.Configuration, appLogger);

builder.Services.AddHealthChecks();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseLocalizationConfigs();

app.UseAuthorization();

app.UseExceptionHandler();
app.UseMiddleware<IdempotencyMiddleware>();

app.MapHealthChecks("/health-check");

app.MapControllers();

app.Run();

public partial class Program { }
