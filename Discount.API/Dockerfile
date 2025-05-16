FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Discount.API/Discount.API.csproj", "Discount.API/"]
COPY ["Discount.Application/Discount.Application.csproj", "Discount.Application/"]
COPY ["Discount.Core/Discount.Core.csproj", "Discount.Core/"]
COPY ["Discount.Infrastructure/Discount.Infrastructure.csproj", "Discount.Infrastructure/"]
RUN dotnet restore "Discount.API/Discount.API.csproj"
COPY . .
WORKDIR "/src/Discount.API"
ARG TARGETARCH
RUN dotnet build "Discount.API.csproj" -c Release -o /app/build /p:Platform=$TARGETARCH

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Discount.API.csproj" -c Release -o /app/publish /p:Platform=$TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discount.API.dll"]
