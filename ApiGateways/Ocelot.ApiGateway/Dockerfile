#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiGateways/Ocelot.ApiGateway/Ocelot.ApiGateway.csproj", "ApiGateways/Ocelot.ApiGateway/"]
COPY ["Infrastructure/Common.Logging/Common.Logging.csproj", "Infrastructure/Common.Logging/"]
COPY ["Services/Discount/Discount.API/Discount.API.csproj", "Services/Discount/Discount.API/"]
COPY ["Services/Discount/Discount.Application/Discount.Application.csproj", "Services/Discount/Discount.Application/"]
COPY ["Services/Discount/Discount.Core/Discount.Core.csproj", "Services/Discount/Discount.Core/"]
COPY ["Services/Discount/Discount.Infrastructure/Discount.Infrastructure.csproj", "Services/Discount/Discount.Infrastructure/"]

RUN dotnet restore "ApiGateways/Ocelot.ApiGateway/Ocelot.ApiGateway.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Ocelot.ApiGateway"
ARG TARGETARCH
RUN dotnet build "Ocelot.ApiGateway.csproj" -c Release -o /app/build /p:Platform=$TARGETARCH

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Ocelot.ApiGateway.csproj" -c Release -o /app/publish /p:Platform=$TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ocelot.ApiGateway.dll"]