FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/PaymentService/PaymentService.csproj", "src/PaymentService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/PaymentService/PaymentService.csproj"
COPY . .
WORKDIR "/src/src/PaymentService"
RUN dotnet build "PaymentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]
