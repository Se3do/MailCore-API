using MailCore.API;
using MailCore.API.Hubs;
using MailCore.API.Middleware;
using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using MailCore.Infrastructure.Data.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(o => o.AddServerHeader = false);

builder.Services.AddAppDI(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MailCore API v1");
    });
}

if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("SeedOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MailCoreDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbSeeder.SeedAsync(context, passwordHasher);
}

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MailCoreDbContext>();
    await context.Database.MigrateAsync();
}

app.UseHttpLogging();
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseCors("AllowedOrigins");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<MailHub>("/hubs/mail");

app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
