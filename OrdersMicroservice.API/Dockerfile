#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrdersMicroservice.API/OrdersMicroService.API.csproj", "OrdersMicroservice.API/"]
COPY ["OrdersMicroservice.Core/OrdersMicroService.Core.csproj", "OrdersMicroservice.Core/"]
COPY ["OrdersMicroservice.Infrastructure/OrdersMicroService.Infrastructure.csproj", "OrdersMicroservice.Infrastructure/"]
RUN dotnet restore "./OrdersMicroservice.API/./OrdersMicroService.API.csproj"
COPY . .
WORKDIR "/src/OrdersMicroservice.API"
RUN dotnet build "./OrdersMicroService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrdersMicroService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV MONGO_HOST=localhost
ENV MONGO_PORT=27017
ENV MONGO_DATABASE=OrdersDatabase
ENV UsersMicroserviceHost=localhost
ENV UsersMicroservicePort=9090
ENV ProductsMicroserviceHost=localhost
ENV ProductsMicroservicePort=8080
ENTRYPOINT ["dotnet", "OrdersMicroService.API.dll"]