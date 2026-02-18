using MailService.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAppDI(builder.Configuration);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MailService API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowSwagger");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
