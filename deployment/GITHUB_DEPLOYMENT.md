# GitHub Deployment Guide

This document provides detailed information about the GitHub Actions workflow setup for the Microservice application.

## Overview

The GitHub Actions workflow automates the following tasks:

1. Building and testing the application
2. Building Docker images for the API components
3. Pushing the Docker images to GitHub Container Registry (ghcr.io)
4. Deploying the application to an on-premises server or cloud provider

## Workflow Structure

The GitHub Actions workflow is organized into multiple jobs for better organization and efficiency:

1. **Build and Test**: Builds and tests the .NET solution
2. **Setup Docker**: Sets up Docker and container registry authentication
3. **Build API Image**: Builds and pushes the API Docker image to GitHub Container Registry
4. **Prepare Deployment**: Creates deployment files for the application
5. **Deploy to Server**: Deploys the application to the target server

This modular approach allows for better maintainability and parallel execution of independent tasks.

## Required GitHub Secrets

The following secrets need to be configured in your GitHub repository:

### Docker Registry Credentials

- `GIT_PAT`: A GitHub Personal Access Token with `read:packages` scope to pull images from GitHub Container Registry

This token is needed for the deployment server to authenticate with GitHub Container Registry and pull the Docker images.

### Deployment Credentials

- `SSH_PRIVATE_KEY`: SSH private key for connecting to the deployment server
- `SSH_USER`: SSH username for the deployment server
- `SERVER_HOST`: Hostname or IP address of the deployment server

### Application Secrets

- `API_KEY`: API key for external services
- `API_SECRET`: API secret for external services
- `TELEGRAM_BOT_TOKEN`: Telegram bot token for notifications (if applicable)
- `TELEGRAM_CHAT_ID`: Telegram chat ID for notifications (if applicable)

## Setting Up GitHub Secrets

To set up GitHub secrets:

1. Go to your GitHub repository
2. Click on "Settings"
3. Click on "Secrets and variables" in the left sidebar
4. Click on "Actions"
5. Click on "New repository secret"
6. Enter the name and value for each secret:
   - Name: `GIT_PAT`, Value: your GitHub Personal Access Token
   - Name: `SSH_PRIVATE_KEY`, Value: your SSH private key
   - Name: `SSH_USER`, Value: your SSH username
   - Name: `SERVER_HOST`, Value: your server hostname or IP
   - Name: `API_KEY`, Value: your API key
   - Name: `API_SECRET`, Value: your API secret
   - Name: `TELEGRAM_BOT_TOKEN`, Value: your Telegram bot token (if applicable)
   - Name: `TELEGRAM_CHAT_ID`, Value: your Telegram chat ID (if applicable)
7. Click "Add secret" for each secret

## Deployment Process

The deployment process works as follows:

1. The workflow builds the application and creates Docker images
2. The images are tagged with the Git SHA and pushed to GitHub Container Registry
3. A deployment script is created and copied to the deployment server
4. The script pulls the latest images and starts the containers using Docker Compose
5. The application is deployed and made available at the configured endpoints

## Troubleshooting

### Workflow Failures

If the workflow fails, check the following:

1. Verify that the `GIT_PAT` secret is correctly set in your GitHub repository
2. Ensure the PAT has the `read:packages` scope
3. Check that the PAT is not expired (GitHub PATs can expire)
4. Verify that the repository owner has access to the packages
5. Try manually logging in to ghcr.io on the deployment server:
   ```bash
   echo "YOUR_PAT" | docker login ghcr.io -u YOUR_GITHUB_USERNAME --password-stdin
   ```

### Deployment Issues

If the deployment fails, you can check the logs on the deployment server:

```bash
# Check the deployment script logs
cat ~/microservice/deploy.log

# Check Docker Compose logs
cd ~/microservice
docker-compose logs
```

## Manual Deployment

If you need to deploy manually, you can run the following commands on the deployment server:

```bash
cd ~/microservice
export API_IMAGE="ghcr.io/your-username/microservice/api:latest"
export API_KEY="your-api-key"
export API_SECRET="your-api-secret"
./deploy.sh
```

## Security Considerations

- Always use secrets for sensitive information
- Limit access to your GitHub repository and deployment server
- Regularly rotate your API keys and tokens
- Use HTTPS for all communications
- Consider using a VPN for accessing your deployment server
