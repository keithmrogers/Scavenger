#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 11111
EXPOSE 30000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Scavenger.Api/Scavenger.Api.csproj", "Scavenger.Api/"]
COPY ["Scavenger.Server.Grains/Scavenger.Server.Grains.csproj", "Scavenger.Server.Grains/"]
COPY ["Scavenger.Server.GrainInterfaces/Scavenger.Server.GrainInterfaces.csproj", "Scavenger.Server.GrainInterfaces/"]
COPY ["Scavenger.Server.Domain/Scavenger.Server.Domain.csproj", "Scavenger.Server.Domain/"]
RUN dotnet restore "./Scavenger.Api/./Scavenger.Api.csproj"
COPY . .
WORKDIR "/src/Scavenger.Api"
RUN dotnet build "./Scavenger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Scavenger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["./Scavenger.Api"]