# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

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

# 🟢 NOVO BLOCO: Publicar corretamente
FROM build AS publish
RUN dotnet publish "DownloaderVideo.Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 🟢 Copiar os arquivos publicados
FROM final AS runtime
COPY --from=publish /app/publish .

# Copiar cookies.txt do seu contexto de build (docker-assets) para dentro do container
COPY docker-assets/Cookies.txt /app/Cookies.txt

ENTRYPOINT ["dotnet", "DownloaderVideo.Application.dll"]