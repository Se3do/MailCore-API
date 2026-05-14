using MailCore.API;
using MailCore.API.Hubs;
using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using MailCore.Infrastructure.Data.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Enforce no server header
builder.WebHost.UseKestrel(o => o.AddServerHeader = false);

builder.Services.AddAppDI(builder.Configuration);

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

if (app.Configuration.GetValue<bool>("SeedOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MailCoreDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbSeeder.SeedAsync(context, passwordHasher);
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors("AllowedOrigins");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<MailHub>("/hubs/mail");

app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
