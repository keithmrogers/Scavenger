#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Scavenger.Api/Scavenger.Api.csproj", "Scavenger.Api/"]
COPY ["Scavenger.Core/Scavenger.Core.csproj", "Scavenger.Core/"]
COPY ["Scavenger.Interfaces/Scavenger.Interfaces.csproj", "Scavenger.Interfaces/"]
RUN dotnet restore "./Scavenger.Api/./Scavenger.Api.csproj"
COPY . .
WORKDIR "/src/Scavenger.Api"
RUN dotnet build "./Scavenger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Scavenger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scavenger.Api.dll"]