FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/OrderManagement.API/OrderManagement.API.csproj", "src/OrderManagement.API/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/OrderManagement.API/OrderManagement.API.csproj"
COPY . .
WORKDIR "/src/src/OrderManagement.API"
RUN dotnet build "OrderManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderManagement.API.dll"]
