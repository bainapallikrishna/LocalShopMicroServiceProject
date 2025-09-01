FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LocalShop.Gateway/LocalShop.Gateway.csproj", "LocalShop.Gateway/"]
COPY ["LocalShop.Shared/LocalShop.Shared.csproj", "LocalShop.Shared/"]
COPY ["LocalShop.Shared.Infrastructure/LocalShop.Shared.Infrastructure.csproj", "LocalShop.Shared.Infrastructure/"]
RUN dotnet restore "LocalShop.Gateway/LocalShop.Gateway.csproj"
COPY . .
WORKDIR "/src/LocalShop.Gateway"
RUN dotnet build "LocalShop.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LocalShop.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LocalShop.Gateway.dll"]
