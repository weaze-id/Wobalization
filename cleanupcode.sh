#! /bin/bash

dotnet clean
dotnet restore
dotnet jb cleanupcode Wobalization.sln --verbosity=OFF --exclude=lib/**/*