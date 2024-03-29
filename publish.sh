#!/bin/bash

# Blazor WebAssembly project path
blazorProjectPath="Cmd/Wobalization.Wasm"

# ASP.NET Core project path
aspnetProjectPath="Cmd/Wobalization"

# Publish Blazor WebAssembly project
dotnet publish "$blazorProjectPath" --configuration Release

# Check if publish succeeded
if [ $? -ne 0 ]; then
  echo "Blazor WebAssembly project publish failed."
  exit 1
fi

echo "Blazor WebAssembly project publish successful."

# Publish ASP.NET Core project
dotnet publish "$aspnetProjectPath" --configuration Release

echo "ASP.NET Core project publish successful."