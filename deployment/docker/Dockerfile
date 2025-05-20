# Stage 1: Build the application
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Set up NuGet configuration to avoid Windows paths
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NUGET_HTTP_CACHE_PATH=/root/.local/share/NuGet/v3-cache
ENV NUGET_PLUGINS_CACHE_PATH=/root/.local/share/NuGet/plugins-cache

# Set GitHub credentials for NuGet
ARG GITHUB_USERNAME
ARG GITHUB_TOKEN

# Copy NuGet.config first
COPY NuGet.config .

# Update GitHub credentials in NuGet.config if provided
RUN if [ -n "$GITHUB_USERNAME" ] && [ -n "$GITHUB_TOKEN" ]; then \
    sed -i "s/%GITHUB_USERNAME%/$GITHUB_USERNAME/g" NuGet.config && \
    sed -i "s/%GITHUB_TOKEN%/$GITHUB_TOKEN/g" NuGet.config; \
    else \
    echo "WARNING: GitHub credentials not provided. GitHub packages may not be accessible."; \
    fi

# Display the NuGet.config for debugging (remove sensitive info)
RUN cat NuGet.config | grep -v ClearTextPassword
# Copy the entire solution
COPY . .

# First restore dependencies explicitly to better handle GitHub package authentication
RUN dotnet restore "Discount.API/Discount.API.csproj"

# Then build and publish the application
RUN dotnet publish "Discount.API/Discount.API.csproj" \
    -c Release \
    --no-restore \
    -o /app/publish

# Stage 2: Create the runtime image
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user to run the application
RUN useradd -ms /bin/bash -d /app appuser && \
    chown -R appuser:appuser /app
USER appuser

# Copy the published application
COPY --from=build --chown=appuser:appuser /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

# Create directory for logs
RUN mkdir -p logs && \
    chmod 755 logs

# Expose port 80
EXPOSE 5002

# Set the entry point
ENTRYPOINT ["dotnet", "Discount.API.dll"]
