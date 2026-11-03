using Gaia.Api.Extensions;
using Gaia.Api.ExceptionHandling;
using Microsoft.AspNetCore.HttpLogging;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOperationEngine();
builder.Services.AddGaiaPersistence(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestMethod |
        HttpLoggingFields.RequestPath |
        HttpLoggingFields.ResponseStatusCode |
        HttpLoggingFields.RequestBody |
        HttpLoggingFields.ResponseBody;

    options.RequestBodyLogLimit = 4096;
    options.ResponseBodyLogLimit = 4096;

    options.MediaTypeOptions.AddText("application/json");
    options.MediaTypeOptions.AddText("application/problem+json");

    options.CombineLogs = true;
});

var app = builder.Build();

app.UseHttpLogging();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
