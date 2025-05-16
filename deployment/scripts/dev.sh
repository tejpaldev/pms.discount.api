#!/bin/bash
# Microservice Development Script for Linux/macOS

set -e

function show_help {
    echo "Microservice Development Script"
    echo "Usage: ./deployment/scripts/dev.sh [command]"
    echo ""
    echo "Commands:"
    echo "  help        Show this help message"
    echo "  up          Start the development environment"
    echo "  down        Stop the development environment"
    echo "  restart     Restart the development environment"
    echo "  logs        View logs from all containers"
    echo "  api-logs    View logs from the API container"
    echo "  db-logs     View logs from the database container"
    echo "  build       Rebuild the containers"
    echo "  clean       Remove all containers, volumes, and images"
    echo ""
}

function start_environment {
    echo "Starting development environment..."
    docker-compose up -d
    echo "Development environment started."
    echo "API is available at: http://localhost:5000/swagger"
}

function stop_environment {
    echo "Stopping development environment..."
    docker-compose down
    echo "Development environment stopped."
}

function restart_environment {
    echo "Restarting development environment..."
    docker-compose restart
    echo "Development environment restarted."
}

function show_logs {
    if [ -z "$1" ]; then
        docker-compose logs -f
    else
        docker-compose logs -f "$1"
    fi
}

function build_environment {
    echo "Building development environment..."
    docker-compose build --no-cache
    echo "Development environment built."
}

function clean_environment {
    echo "Cleaning development environment..."
    docker-compose down -v --rmi all
    echo "Development environment cleaned."
}

# Main script
COMMAND=${1:-help}

case $COMMAND in
    help)
        show_help
        ;;
    up)
        start_environment
        ;;
    down)
        stop_environment
        ;;
    restart)
        restart_environment
        ;;
    logs)
        show_logs
        ;;
    api-logs)
        show_logs "api"
        ;;
    db-logs)
        show_logs "postgres"
        ;;
    build)
        build_environment
        ;;
    clean)
        clean_environment
        ;;
    *)
        echo "Unknown command: $COMMAND"
        show_help
        exit 1
        ;;
esac
