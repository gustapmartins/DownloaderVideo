using DownloaderVideo.Domain.Entity;
using DownloaderVideo.Domain.Exceptions;
using DownloaderVideo.Infra.CrossCutting.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DownloaderVideo.Infra.CrossCutting.Ioc;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void ConfigureService(this IServiceCollection services, IConfiguration configuration, string xmlFileName)
    {
        services.Configure<DatabaseSettings>
            (configuration.GetSection("Database"));

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisConnection = configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(redisConnection);
        });

        services.AddSingleton<IDatabase>(provider =>
        {
            var connection = provider.GetRequiredService<IConnectionMultiplexer>();
            return connection.GetDatabase();
        });


        services.AddControllers(opts =>
        {
            opts.Filters.Add<ExceptionFilterGeneric>();
        });

        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0); // Exemplo de versão padrão
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        #region Config Swagger

        services.AddSwaggerGen(c =>
        {

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"Generate Template - {version}",
                Version = version,
                Description = "Aplicação responsavél por gerar um template generico",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Example Contact",
                    Url = new Uri("https://example.com/contact"),
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://example.com/license"),
                },
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    new List<string>()
                },
            });

            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });
        #endregion

        services.AddEndpointsApiExplorer();

        services.AddCors();

        services.AddMemoryCache();

        services.AddHttpContextAccessor();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddSingleton<DatabaseSettings>();

    }
}
