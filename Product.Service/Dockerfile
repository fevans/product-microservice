FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["product-microservice/Product.Service/Product.Service.csproj", "Product.Service/"]
COPY ["local-nuget-packages/", "/local-nuget-packages/"]
COPY ["product-microservice/nuget.config", "./"]
RUN dotnet restore "Product.Service/Product.Service.csproj"

COPY ["product-microservice/Product.Service/", "Product.Service/"]

WORKDIR "/src/Product.Service"
RUN dotnet build "Product.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Product.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Service.dll"]
