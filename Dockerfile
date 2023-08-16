#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["VulnerableApp4APISecurity.API/VulnerableApp4APISecurity.API.csproj", "VulnerableApp4APISecurity.API/"]
COPY ["VulnerableApp4APISecurity.Core/VulnerableApp4APISecurity.Core.csproj", "VulnerableApp4APISecurity.Core/"]
COPY ["VulnerableApp4APISecurity.Infrastructure/VulnerableApp4APISecurity.Infrastructure.csproj", "VulnerableApp4APISecurity.Infrastructure/"]
RUN dotnet restore "VulnerableApp4APISecurity.API/VulnerableApp4APISecurity.API.csproj"
COPY . .
WORKDIR "/src/VulnerableApp4APISecurity.API"
RUN dotnet build "VulnerableApp4APISecurity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VulnerableApp4APISecurity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VulnerableApp4APISecurity.API.dll"]
