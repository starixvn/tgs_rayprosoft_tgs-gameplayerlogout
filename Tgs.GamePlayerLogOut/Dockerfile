#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GamePlayerLogOut/Tgs.GamePlayerLogOut.csproj", "Tgs.GamePlayerLogOut/"]
COPY ["Tgs.GamePlayerLogOut.Core/Tgs.GamePlayerLogOut.Core.csproj", "Tgs.GamePlayerLogOut.Core/"]
COPY ["Tgs.GamePlayerLogOut.Protos/Tgs.GamePlayerLogOut.Protos.csproj", "Tgs.GamePlayerLogOut.Protos/"]
RUN dotnet restore "./Tgs.GamePlayerLogOut/Tgs.GamePlayerLogOut.csproj"
COPY . .
WORKDIR "/src/Tgs.GamePlayerLogOut"
RUN dotnet build "./Tgs.GamePlayerLogOut.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GamePlayerLogOut.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GamePlayerLogOut.dll"]