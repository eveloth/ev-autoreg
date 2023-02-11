FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/EvAutoreg.Autoregistrar/EvAutoreg.Autoregistrar.csproj", "src/EvAutoreg.Autoregistrar/"]
COPY ["src/EvAutoreg.Data/EvAutoreg.Data.csproj", "src/EvAutoreg.Data/"]
COPY ["src/EvAutoreg.Extensions/EvAutoreg.Extensions.csproj", "src/EvAutoreg.Extensions/"]
RUN dotnet restore "src/EvAutoreg.Autoregistrar/EvAutoreg.Autoregistrar.csproj"
COPY . .
WORKDIR "/src/src/EvAutoreg.Autoregistrar"
RUN dotnet build "EvAutoreg.Autoregistrar.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EvAutoreg.Autoregistrar.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN apt update && apt -y install gss-ntlmssp
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EvAutoreg.Autoregistrar.dll"]
