# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

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

# Final image with ffmpeg and yt-dlp installed in virtual env
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y \
    python3 python3-pip ffmpeg curl

RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /usr/local/bin/yt-dlp \
 && chmod +x /usr/local/bin/yt-dlp

COPY --from=build /app/build .

# 🟢 Aqui está o segredo: use a variável de ambiente PORT
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE 8080
ENTRYPOINT ["dotnet", "DownloaderVideo.Application.dll"]