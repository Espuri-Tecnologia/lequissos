#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/LexosHub.ERP.VarejOnline.Api/LexosHub.ERP.VarejOnline.Api.csproj", "src/LexosHub.ERP.VarejOnline.Api/"]
COPY ["src/LexosHub.ERP.VarejOnline.Domain/LexosHub.ERP.VarejOnline.Domain.csproj", "src/LexosHub.ERP.VarejOnline.Domain/"]
COPY ["src/LexosHub.ERP.VarejOnline.Infra.CrossCutting/LexosHub.ERP.VarejOnline.Infra.CrossCutting.csproj", "src/LexosHub.ERP.VarejOnline.Infra.CrossCutting/"]
COPY ["src/LexosHub.ERP.VarejOnline.Infra.ErpApi/LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.csproj", "src/LexosHub.ERP.VarejOnline.Infra.ErpApi/"]
# COPY ["src/LexosHub.ERP.VarejOnline.Infra.SyncIn/LexosHub.ERP.VarejOnline.Infra.SyncIn.csproj", "src/LexosHub.ERP.VarejOnline.Infra.SyncIn/"]
# COPY ["src/LexosHub.ERP.VarejOnline.Infra.SyncOut/LexosHub.ERP.VarejOnline.Infra.SyncOut.csproj", "src/LexosHub.ERP.VarejOnline.Infra.SyncOut/"]
# COPY ["src/LexosHub.ERP.VarejOnline.Infra.Data.Migrations/LexosHub.ERP.VarejOnline.Infra.Data.Migrations.csproj", "src/LexosHub.ERP.VarejOnline.Infra.Data.Migrations/"]
COPY ["src/LexosHub.ERP.VarejOnline.Infra.Data/LexosHub.ERP.VarejOnline.Infra.Data.csproj", "src/LexosHub.ERP.VarejOnline.Infra.Data/"]
RUN dotnet restore "src/LexosHub.ERP.VarejOnline.Api/LexosHub.ERP.VarejOnline.Api.csproj"
COPY . .
WORKDIR "/src/src/LexosHub.ERP.VarejOnline.Api"
RUN dotnet build "LexosHub.ERP.VarejOnline.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LexosHub.ERP.VarejOnline.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LexosHub.ERP.VarejOnline.Api.dll"]