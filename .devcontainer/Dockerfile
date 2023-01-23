FROM mcr.microsoft.com/devcontainers/dotnet:0-6.0-bullseye

RUN wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | /bin/bash

USER vscode

RUN dapr init -s