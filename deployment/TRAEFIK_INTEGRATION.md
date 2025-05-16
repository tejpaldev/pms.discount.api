# Microservice Deployment with Traefik and Let's Encrypt

This guide explains how the current infrastructure is set up with Traefik as a reverse proxy that automatically handles HTTPS certificates through Let's Encrypt.

## Current Architecture

The current setup uses:
- Traefik (deployed as a separate Docker container) for routing and SSL termination
- Let's Encrypt for automatic SSL certificate generation and renewal
- Docker Compose for service definition
- GitHub Actions for CI/CD

## How Traefik Integration Works

Traefik automatically:
1. Discovers Docker containers based on their labels
2. Obtains and renews Let's Encrypt certificates
3. Routes traffic based on hostnames
4. Handles HTTPS traffic with proper SSL termination

## Adding a New Microservice

When adding a new microservice to your infrastructure, follow these steps to ensure it's properly integrated with Traefik:

### 1. Docker Compose Configuration

Create a docker-compose.yml for your new microservice with the following key components:

```yml
services:
  your-service-name:
    image: your-service-image
    container_name: your-service-name
    restart: unless-stopped
    environment:
      - YOUR_ENV_VARIABLE=value
    volumes:
      - your-service-data:/app/data
    networks:
      - pms_network  # Important: Connect to the same network as Traefik
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.your-service-name.rule=Host(`your-service.yourdomain.com`)"
      - "traefik.http.routers.your-service-name.entrypoints=websecure"
      - "traefik.http.routers.your-service-name.tls.certresolver=letsencrypt"
      - "traefik.http.services.your-service-name.loadbalancer.server.port=YOUR_CONTAINER_PORT"

networks:
  pms_network:
    external: true

volumes:
  your-service-data:
    driver: local
```

### 2. GitHub Actions Workflow

Create a GitHub Actions workflow similar to the one for the Discount API:

```yml
name: Build and Deploy Your Service

on:
  push:
    branches: [main]

jobs:
  # Include jobs for building and testing
  
  # Prepare deployment files
  prepare-deployment:
    name: Prepare Deployment Files
    runs-on: ubuntu-latest
    
    steps:
      - name: Create docker-compose.yml for deployment
        run: |
          cat > docker-compose.yml << 'EOL'
          services:
            your-service:
              image: ${YOUR_SERVICE_IMAGE}
              container_name: your-service-name
              restart: unless-stopped
              environment:
                - YOUR_ENV_VARIABLE=${YOUR_ENV_VARIABLE}
              volumes:
                - your-service-data:/app/data
              networks:
                - pms_network
              labels:
                - "traefik.enable=true"
                - "traefik.http.routers.your-service-name.rule=Host(\`your-service.yourdomain.com\`)"
                - "traefik.http.routers.your-service-name.entrypoints=websecure"
                - "traefik.http.routers.your-service-name.tls.certresolver=letsencrypt"
                - "traefik.http.services.your-service-name.loadbalancer.server.port=YOUR_CONTAINER_PORT"
          
          networks:
            pms_network:
              external: true
          
          volumes:
            your-service-data:
              driver: local
          EOL

      # Include steps for deployment script and deployment to server
```

### 3. Deploying the Service

When deploying your new service:

1. Make sure your service is pushed to a container registry
2. Ensure it has the proper Traefik labels
3. Connect it to the `pms_network` (which should be marked as `external: true`)
4. Deploy it to your server using SSH

## Important Traefik Label Explanations

- `traefik.enable=true` - Tells Traefik to process this container
- `traefik.http.routers.your-service-name.rule=Host(\`your-service.yourdomain.com\`)` - Routes traffic based on the hostname
- `traefik.http.routers.your-service-name.entrypoints=websecure` - Uses the HTTPS entrypoint
- `traefik.http.routers.your-service-name.tls.certresolver=letsencrypt` - Uses Let's Encrypt to get SSL certificates
- `traefik.http.services.your-service-name.loadbalancer.server.port=YOUR_CONTAINER_PORT` - Specifies which port your service is listening on inside the container

## Troubleshooting

If your service isn't accessible via HTTPS:

1. Check that it's connected to the `pms_network`
2. Verify all Traefik labels are correctly set
3. Make sure your DNS is properly configured to point to your server
4. Check Traefik logs for certificate issuance problems

## DNS Configuration

For each new microservice, make sure to add a DNS A record:
- Name: `your-service` (subdomain)
- Value: Your server's IP address
- TTL: 3600 (or as appropriate)

Your DNS configuration should point all your service subdomains to the same server IP where Traefik is running.
