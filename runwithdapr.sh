#!/bin/bash

dapr run --app-id http-test --components-path ./components/ --config ./config/config.yaml --app-port 5000 -H 3500 -- dotnet run --project .