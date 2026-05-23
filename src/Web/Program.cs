using DailyTracker.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//aspire сервисы
//builder.AddServiceDefaults();

//сервис для безопасного хранения и получения секретов
//builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseCors(static builder =>
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());
}
else
{
    var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(',').Select(x => x.Trim()).ToArray() ?? Array.Empty<string>();

    app.UseCors(options => options
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
}

app.UseFileServer();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/scalar"));

//точки доступа для aspire-сервисов
//app.MapDefaultEndpoints();
app.MapEndpoints(typeof(Program).Assembly);


app.Run();
