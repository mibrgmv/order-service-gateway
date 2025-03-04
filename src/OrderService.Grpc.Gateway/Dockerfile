FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./*.props ./
COPY ["src/OrderService.Grpc.Gateway/OrderService.Grpc.Gateway.csproj", "src/OrderService.Grpc.Gateway/"]
RUN dotnet restore "src/OrderService.Grpc.Gateway/OrderService.Grpc.Gateway.csproj"
COPY . .
WORKDIR "/src/src/OrderService.Grpc.Gateway"
RUN dotnet build "OrderService.Grpc.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.Grpc.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Grpc.Gateway.dll"]
