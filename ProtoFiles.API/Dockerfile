FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProtoFiles.API/ProtoFiles.API.csproj", "ProtoFiles.API/"]
COPY ["ProtoFiles.Lib/ProtoFiles.Lib.csproj", "ProtoFiles.Lib/"]
RUN dotnet restore "ProtoFiles.API/ProtoFiles.API.csproj"
COPY . .
WORKDIR "/src/ProtoFiles.API"
RUN dotnet build "ProtoFiles.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProtoFiles.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProtoFiles.API.dll"]
