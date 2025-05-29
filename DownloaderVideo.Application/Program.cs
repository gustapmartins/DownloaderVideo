using DownloaderVideo.Application.AppServices;
using DownloaderVideo.Domain;
using DownloaderVideo.Infra.CrossCutting.Ioc;
using DownloaderVideo.Infra.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

AppServiceDependencyInjection.AppServiceDependencyInjectionModule(builder.Services);

DependencyInjection.ConfigureService(builder.Services, builder.Configuration, xmlFilename);

RepositoryDependencyInjectionModule.RepositoryDependencyInjectionModuleModule(builder.Services);

ServiceDependencyInjection.ServiceDependencyInjectionModule(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();  // Habilita só no dev
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseRouting();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
