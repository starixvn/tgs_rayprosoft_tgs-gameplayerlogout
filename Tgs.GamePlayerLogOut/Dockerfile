#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GamePlayerLogOut/Tgs.GamePlayerLogOut.csproj", "Tgs.GamePlayerLogOut/"]
COPY ["Tgs.GamePlayerLogOut.Common/Tgs.GamePlayerLogOut.Common.csproj", "Tgs.GamePlayerLogOut.Common/"]
COPY ["Tgs.GamePlayerLogOut.Core/Tgs.GamePlayerLogOut.Core.csproj", "Tgs.GamePlayerLogOut.Core/"]
RUN dotnet restore "Tgs.GamePlayerLogOut/Tgs.GamePlayerLogOut.csproj"
COPY . .
WORKDIR "/src/Tgs.GamePlayerLogOut"
RUN dotnet build "Tgs.GamePlayerLogOut.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GamePlayerLogOut.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GamePlayerLogOut.dll"]