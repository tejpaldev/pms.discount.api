# Sample Microservice Integration Template

This template provides a starting point for adding new microservices to your infrastructure with proper Traefik integration.

## Docker Compose Template

```yml
version: '3.8'

services:
  # Your new microservice
  your-service-name:
    image: your-image:tag
    container_name: your-service-name
    restart: unless-stopped
    environment:
      - ENVIRONMENT_VARIABLE=value
    volumes:
      - your-service-data:/path/in/container
    networks:
      - pms_network
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.your-service-name.rule=Host(`your-service.yourdomain.com`)"
      - "traefik.http.routers.your-service-name.entrypoints=websecure"
      - "traefik.http.routers.your-service-name.tls.certresolver=letsencrypt"
      - "traefik.http.services.your-service-name.loadbalancer.server.port=80"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:80/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 10s

networks:
  pms_network:
    external: true

volumes:
  your-service-data:
    driver: local
```

## GitHub Actions Workflow Template

```yml
name: Deploy Your Microservice

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v3
      
      - name: Build and push Docker image
        # Build and push your image to a registry
        
      - name: Deploy to server
        uses: appleboy/ssh-action@master
        with:
          host: ${{ secrets.HOST }}
          username: ${{ secrets.USERNAME }}
          key: ${{ secrets.SSH_KEY }}
          script: |
            cd ~/pms-stack
            
            # Create or update your service definition
            cat > your-service.yml << EOF
            version: '3.8'
            
            services:
              your-service-name:
                image: your-image:latest
                restart: always
                networks:
                  - pms_network
                labels:
                  - "traefik.enable=true"
                  - "traefik.http.routers.your-service-name.rule=Host(\`your-service.yourdomain.com\`)"
                  - "traefik.http.routers.your-service-name.entrypoints=websecure"
                  - "traefik.http.routers.your-service-name.tls.certresolver=letsencrypt"
                  - "traefik.http.services.your-service-name.loadbalancer.server.port=80"
            
            networks:
              pms_network:
                external: true
            EOF
            
            # Apply the changes
            docker-compose -f your-service.yml up -d
```

## Deployment Script Template

```bash
#!/bin/bash
set -e

# Set environment variables for docker-compose
export SERVICE_IMAGE="your-registry/your-image:tag"
export YOUR_ENV_VARIABLE="your-value"

# Log in to Container Registry (if needed)
echo "${REGISTRY_TOKEN}" | docker login your-registry -u ${REGISTRY_USER} --password-stdin

# Pull the latest images
docker-compose pull

# Start the containers
docker-compose up -d

# Clean up old images
docker image prune -f
```

## Checklist for New Microservice Deployment

- [ ] Create Docker image for your microservice
- [ ] Push image to container registry
- [ ] Create docker-compose.yml with proper Traefik labels
- [ ] Connect to the pms_network (external network)
- [ ] Set up GitHub Actions workflow for CI/CD
- [ ] Configure DNS records for your new service subdomain
- [ ] Deploy and verify HTTPS is working correctly
