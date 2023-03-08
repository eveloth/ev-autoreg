FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0.103-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY ["src/EvAutoreg.Api/EvAutoreg.Api.csproj", "src/EvAutoreg.Api/"]
COPY ["src/EvAutoreg.Data/EvAutoreg.Data.csproj", "src/EvAutoreg.Data/"]
COPY ["src/EvAutoreg.Extensions/EvAutoreg.Extensions.csproj", "src/EvAutoreg.Extensions/"]
RUN dotnet restore "src/EvAutoreg.Api/EvAutoreg.Api.csproj"
COPY . .
WORKDIR "/src/src/EvAutoreg.Api"
RUN dotnet build "EvAutoreg.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EvAutoreg.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EvAutoreg.Api.dll"]
