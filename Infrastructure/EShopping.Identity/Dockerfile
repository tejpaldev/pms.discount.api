FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Infrastructure/EShopping.Identity/EShopping.Identity.csproj", "Infrastructure/EShopping.Identity/"]
RUN dotnet restore "Infrastructure/EShopping.Identity/EShopping.Identity.csproj"
COPY . .
WORKDIR "/src/Infrastructure/EShopping.Identity"
ARG TARGETARCH
RUN dotnet build "EShopping.Identity.csproj" -c Release -o /app/build /p:Platform=$TARGETARCH

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "EShopping.Identity.csproj" -c Release -o /app/publish /p:Platform=$TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShopping.Identity.dll"]
