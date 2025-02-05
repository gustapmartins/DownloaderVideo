# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["DownloaderVideo.Application/DownloaderVideo.Application.csproj", "DownloaderVideo.Application/"]
COPY ["DownloaderVideo.CrossCutting/DownloaderVideo.Infra.CrossCutting.csproj", "DownloaderVideo.Infra.CrossCutting/"]
COPY ["DownloaderVideo.Domain/DownloaderVideo.Domain.csproj", "DownloaderVideo.Domain/"]
COPY ["DownloaderVideo.Data/DownloaderVideo.Infra.Data.csproj", "DownloaderVideo.Infra.Data/"]
COPY ["DownloaderVideo.Test/DownloaderVideo.Test.csproj", "DownloaderVideo.Test/"]

# Restaura as dependências do projeto
RUN dotnet restore "DownloaderVideo.Application/DownloaderVideo.Application.csproj"

COPY . .
WORKDIR "/src/DownloaderVideo.Application"
RUN dotnet build "DownloaderVideo.Application.csproj" -c Release -o /app/build 

# Esta fase é usada na produção ou quando executada no VS no modo normal
FROM build AS final
WORKDIR /app
COPY --from=build /app/build .
EXPOSE 80

ENTRYPOINT ["dotnet", "DownloaderVideo.Application.dll", "--urls", "http://+:443;http://*:80"]