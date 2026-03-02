using FluentValidation;
using MailCore.Application;
using MailCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

namespace MailCore.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationDI()
                .AddInfrastructureDI(configuration);

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddEndpointsApiExplorer();

            services.AddHealthChecks();

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("auth", o =>
                {
                    o.Window = TimeSpan.FromMinutes(1);
                    o.PermitLimit = 10;
                    o.QueueLimit = 0;
                    o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var jwt = configuration.GetSection("Jwt");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt["Secret"]!)),

                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MailCore API",
                    Version = "v1"
                });
            
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });
            
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", policy =>
                {
                    var origins = configuration
                        .GetSection("Cors:AllowedOrigins")
                        .Get<string[]>() ?? [];

                    if (origins.Length > 0)
                        policy.WithOrigins(origins)
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    else
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
