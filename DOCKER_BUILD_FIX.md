# Docker Build Fix for ARM64

This document explains the changes made to fix the Docker build issues for ARM64 architecture.

## Issues Fixed

1. **ARM64 Build Error**: Fixed the error `Could not open '/lib/ld-linux-aarch64.so.1': No such file or directory`
2. **Nullable Warning**: Fixed the warning about non-nullable properties in the Coupon class

## Changes Made

### 1. Updated Dockerfile.Discount.API

#### Changed the base images to support multi-platform builds:

```dockerfile
# Before
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
# After
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
```

```dockerfile
# Before
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine-arm64v8 AS final
# After
FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
```

#### Simplified the build and publish commands:

```dockerfile
# Before
RUN dotnet restore "Discount.API/Discount.API.csproj" \
    --runtime linux-musl-arm64 \
    -p:Platform=ARM64

RUN dotnet publish "Discount.API/Discount.API.csproj" \
    -c Release \
    --runtime linux-musl-arm64 \
    -p:Platform=ARM64 \
    --self-contained false \
    --no-restore \
    -o /app/publish

# After
RUN dotnet restore "Discount.API/Discount.API.csproj"

RUN dotnet publish "Discount.API/Discount.API.csproj" \
    -c Release \
    --no-restore \
    -o /app/publish
```

#### Updated user creation for non-Alpine images:

```dockerfile
# Before (Alpine-specific)
RUN apk add --no-cache libaio libnsl libc6-compat
RUN adduser --disabled-password --home /app --gecos "" appuser && \
    chown -R appuser:appuser /app

# After (Works on Debian/Ubuntu-based images)
RUN useradd -ms /bin/bash -d /app appuser && \
    chown -R appuser:appuser /app
```

### 2. Updated GitHub Actions Workflow

#### Removed platform restriction from Docker Buildx setup:

```yaml
# Before
- name: Set up Docker Buildx
  uses: docker/setup-buildx-action@v3
  with:
    platforms: linux/arm64

# After
- name: Set up Docker Buildx
  uses: docker/setup-buildx-action@v3
```

#### Added multi-platform support to the build-and-push step:

```yaml
# Before
platforms: linux/arm64

# After
platforms: linux/amd64,linux/arm64
```

### 3. Fixed Nullable Warning in Coupon.cs

```csharp
// Before
public string ProductName { get; set; }
public string Description { get; set; }

// After
public required string ProductName { get; set; }
public required string Description { get; set; }
```

## Benefits of These Changes

1. **Better Cross-Platform Support**: The Dockerfile now properly handles building for multiple architectures
2. **Simplified Build Process**: Removed unnecessary platform-specific flags
3. **Improved Code Quality**: Fixed nullable warnings in the C# code
4. **More Flexible Deployment**: The image can now be built for both AMD64 and ARM64 architectures

## Testing

To test these changes locally:

1. Build the Docker image:
   ```bash
   docker buildx build --platform linux/amd64,linux/arm64 -t discount-api -f deployment/docker/Dockerfile.Discount.API .
   ```

2. Run the container:
   ```bash
   docker run -p 5002:5002 discount-api
   ```

## Notes

- The `required` keyword in C# 11 ensures that properties must be initialized during object creation
- The `--platform=$BUILDPLATFORM` and `--platform=$TARGETPLATFORM` flags allow Docker to handle cross-platform builds automatically
- Removing platform-specific dependencies makes the Dockerfile more portable
