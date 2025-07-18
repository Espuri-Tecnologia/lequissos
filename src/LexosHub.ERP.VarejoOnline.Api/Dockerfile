#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/LexosHub.ERP.VarejoOnline.Api/LexosHub.ERP.VarejoOnline.Api.csproj", "src/LexosHub.ERP.VarejoOnline.Api/"]
COPY ["src/LexosHub.ERP.VarejoOnline.Domain/LexosHub.ERP.VarejoOnline.Domain.csproj", "src/LexosHub.ERP.VarejoOnline.Domain/"]
COPY ["src/LexosHub.ERP.VarejoOnline.Infra.CrossCutting/LexosHub.ERP.VarejoOnline.Infra.CrossCutting.csproj", "src/LexosHub.ERP.VarejoOnline.Infra.CrossCutting/"]
COPY ["src/LexosHub.ERP.VarejoOnline.Infra.ErpApi/LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.csproj", "src/LexosHub.ERP.VarejoOnline.Infra.ErpApi/"]
# COPY ["src/LexosHub.ERP.VarejoOnline.Infra.SyncIn/LexosHub.ERP.VarejoOnline.Infra.SyncIn.csproj", "src/LexosHub.ERP.VarejoOnline.Infra.SyncIn/"]
# COPY ["src/LexosHub.ERP.VarejoOnline.Infra.SyncOut/LexosHub.ERP.VarejoOnline.Infra.SyncOut.csproj", "src/LexosHub.ERP.VarejoOnline.Infra.SyncOut/"]
# COPY ["src/LexosHub.ERP.VarejoOnline.Infra.Data.Migrations/LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.csproj", "src/LexosHub.ERP.VarejoOnline.Infra.Data.Migrations/"]
COPY ["src/LexosHub.ERP.VarejoOnline.Infra.Data/LexosHub.ERP.VarejoOnline.Infra.Data.csproj", "src/LexosHub.ERP.VarejoOnline.Infra.Data/"]
RUN dotnet restore "src/LexosHub.ERP.VarejoOnline.Api/LexosHub.ERP.VarejoOnline.Api.csproj"
COPY . .
WORKDIR "/src/src/LexosHub.ERP.VarejoOnline.Api"
RUN dotnet build "LexosHub.ERP.VarejoOnline.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LexosHub.ERP.VarejoOnline.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LexosHub.ERP.VarejoOnline.Api.dll"]