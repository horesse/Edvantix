# Edvantix Project Justfile
# Cross-platform task runner for the Edvantix application
# Set shell for different platforms

set windows-shell := ["pwsh.exe", "-NoLogo", "-ExecutionPolicy", "Bypass", "-Command"]

# Variables

solution := "Edvantix.slnx"

# Default recipe when running just 'just'

default: run

# Restore NuGet packages and tools

restore:
    dotnet restore
    dotnet tool restore

# Build the solution

build: restore
    dotnet build {{ solution }}

# Run tests

test: build
    dotnet test {{ solution }}

# Format C# code

format-cs:
    dnx csharpier format .

# Format frontend code

format-fe:
    cd src/Clients && pnpm format

# Format Keycloakify

format-keycloakify:
    cd src/Aspire/Edvantix.AppHost/Container/keycloak/keycloakify && bun run format

# Format all code

format: format-cs format-fe format-keycloakify
    echo "All code formatted successfully!"

# Clean build artifacts

clean:
    dotnet clean {{ solution }}

# Setup pre-commit hooks

hook:
    git config core.hooksPath .husky
    echo "Pre-commit hook setup complete"

# First-time setup after cloning

prepare: restore hook
    echo "Setup complete! Run 'just run' to start the application."

# Run the application

run:
    aspire run

# Update Keycloakify bun packages

update-keycloakify:
    cd src/Aspire/Edvantix.AppHost/Container/keycloak/keycloakify && bun update --latest

# Update frontend packages

update-fe:
    cd src/Clients && pnpm update --recursive --latest

# Update all packages

update: update-keycloakify update-fe
    echo "All packages updated successfully!"