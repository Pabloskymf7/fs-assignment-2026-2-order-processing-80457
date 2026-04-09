FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/InventoryService/InventoryService.csproj", "src/InventoryService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/InventoryService/InventoryService.csproj"
COPY . .
WORKDIR "/src/src/InventoryService"
RUN dotnet build "InventoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]
