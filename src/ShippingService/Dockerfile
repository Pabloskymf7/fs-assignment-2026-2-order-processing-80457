FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/ShippingService/ShippingService.csproj", "src/ShippingService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/ShippingService/ShippingService.csproj"
COPY . .
WORKDIR "/src/src/ShippingService"
RUN dotnet build "ShippingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShippingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShippingService.dll"]
