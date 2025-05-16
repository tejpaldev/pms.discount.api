# Microservice Development Script for Windows
param (
    [Parameter(Position=0)]
    [string]$Command = "help"
)

$ErrorActionPreference = "Stop"

function Show-Help {
    Write-Host "Microservice Development Script"
    Write-Host "Usage: .\deployment\scripts\dev.ps1 [command]"
    Write-Host ""
    Write-Host "Commands:"
    Write-Host "  help        Show this help message"
    Write-Host "  up          Start the development environment"
    Write-Host "  down        Stop the development environment"
    Write-Host "  restart     Restart the development environment"
    Write-Host "  logs        View logs from all containers"
    Write-Host "  api-logs    View logs from the API container"
    Write-Host "  db-logs     View logs from the database container"
    Write-Host "  build       Rebuild the containers"
    Write-Host "  clean       Remove all containers, volumes, and images"
    Write-Host ""
}

function Start-Environment {
    Write-Host "Starting development environment..."
    docker-compose up -d
    Write-Host "Development environment started."
    Write-Host "API is available at: http://localhost:5002/swagger"
}

function Stop-Environment {
    Write-Host "Stopping development environment..."
    docker-compose down
    Write-Host "Development environment stopped."
}

function Restart-Environment {
    Write-Host "Restarting development environment..."
    docker-compose restart
    Write-Host "Development environment restarted."
}

function Show-Logs {
    param (
        [string]$Service = ""
    )
    
    if ($Service -eq "") {
        docker-compose logs -f
    } else {
        docker-compose logs -f $Service
    }
}

function Build-Environment {
    Write-Host "Building development environment..."
    docker-compose build --no-cache
    Write-Host "Development environment built."
}

function Clean-Environment {
    Write-Host "Cleaning development environment..."
    docker-compose down -v --rmi all
    Write-Host "Development environment cleaned."
}

# Main script
switch ($Command) {
    "help" { Show-Help }
    "up" { Start-Environment }
    "down" { Stop-Environment }
    "restart" { Restart-Environment }
    "logs" { Show-Logs }
    "api-logs" { Show-Logs -Service "api" }
    "db-logs" { Show-Logs -Service "postgres" }
    "build" { Build-Environment }
    "clean" { Clean-Environment }
    default {
        Write-Host "Unknown command: $Command"
        Show-Help
    }
}
