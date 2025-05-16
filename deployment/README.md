# Microservice Deployment

This directory contains files and scripts for deploying the Microservice application.

## Docker Deployment

The application can be deployed using Docker and Docker Compose. The following files are provided:

- `docker/Dockerfile.api`: Dockerfile for building the API container
- `docker/nginx.conf`: Nginx configuration for the Client container (if applicable)
- `../docker-compose.yml`: Docker Compose file for orchestrating the containers

### Prerequisites

- Docker
- Docker Compose
- Required API keys and secrets

### Environment Variables

The following environment variables can be set to configure the application:

- `API_KEY`: API key for external services
- `API_SECRET`: API secret for external services
- `TELEGRAM_BOT_TOKEN`: Telegram bot token for notifications (if applicable)
- `TELEGRAM_CHAT_ID`: Telegram chat ID for notifications (if applicable)

### Development Environment

For local development, use the provided scripts:

**Windows:**
```powershell
# Show available commands
.\deployment\scripts\dev.ps1 help

# Start the development environment
.\deployment\scripts\dev.ps1 up

# View logs
.\deployment\scripts\dev.ps1 logs
```

**Linux/macOS:**
```bash
# Make the script executable
chmod +x deployment/scripts/dev.sh

# Show available commands
./deployment/scripts/dev.sh help

# Start the development environment
./deployment/scripts/dev.sh up

# View logs
./deployment/scripts/dev.sh logs
```

### Production Deployment

For production deployment, follow these steps:

1. Set up the required environment variables
2. Run the following commands:

```bash
# Build and start the containers
docker-compose up -d

# View logs
docker-compose logs -f
```

## Architecture

The application is deployed as containers:

1. **API Container**: Contains the ASP.NET Core API and background processing
2. **Client Container**: Contains the client application (if applicable)

The containers communicate over a Docker network.

## Database Migrations

Database migrations are automatically applied when the API container starts. No manual migration steps are required.

## Troubleshooting

### Container Logs

To view container logs:

```bash
# View all logs
docker-compose logs -f

# View API logs
docker-compose logs -f api
```

### Container Shell

To access a shell in a container:

```bash
# API container
docker exec -it microservice-api /bin/sh
```

### Rebuilding Containers

If you need to rebuild the containers:

```bash
docker-compose build --no-cache
docker-compose up -d
```
