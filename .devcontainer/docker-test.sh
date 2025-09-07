#!/bin/bash
# Test script to verify Docker-outside-of-Docker setup

echo "🔍 Testing Docker availability..."

# Check if docker command exists
if ! command -v docker &> /dev/null; then
    echo "❌ Docker command not found"
    exit 1
fi

echo "✅ Docker command available"

# Check if docker daemon is accessible
if ! docker info &> /dev/null; then
    echo "❌ Cannot connect to Docker daemon"
    echo "ℹ️  Make sure:"
    echo "   1. Docker Desktop is running on the host"
    echo "   2. Dev container has Docker-outside-of-Docker feature enabled"
    echo "   3. Docker socket is properly mounted"
    exit 1
fi

echo "✅ Docker daemon accessible"

# Test basic docker functionality
echo "🧪 Testing basic Docker functionality..."
if docker run --rm hello-world > /dev/null 2>&1; then
    echo "✅ Docker is working correctly!"
    echo "🐳 Ready to build Docker images"
else
    echo "❌ Docker test failed"
    exit 1
fi

echo ""
echo ""


echo "🔍 Testing docker compose availability..."

# Check if docker-compose command exists
if ! command -v docker-compose &> /dev/null; then
    echo "❌ docker-compose command not found"
    exit 1
fi

# Test basic docker-compose functionality
echo "🧪 Testing basic docker-compose functionality..."
if docker-compose config > /dev/null 2>&1; then
    echo "✅ Docker Compose is working correctly!"
else
    echo "❌ Docker Compose test failed"
    exit 1
fi