#!/usr/bin/env bash

#nuget install -OutputDirectory nuget

msbuild /p:Configuration=Release MMALSharp.sln
#msbuild /p:Configuration=Release ./src/MMALSharp/MMALSharp.csproj