# dapr-http-binding-testapp-csharp

Simple ASP.NET core application to test tracing with an HTTP Binding. 

Current versions of the Dapr HTTP Binding to not pass traceparent and tracestate to the HTTP endpoint.

I'm using this app to verify code changes to the HTTP Binding to support trace header propogation.

The devcontainer includes a self-hosted instance of Dapr. If you do not use the devcontainer, you will need to [install your own copy of the Dapr cli](https://docs.dapr.io/getting-started/install-dapr-cli/) and run `dapr init` (with the -s for self-hosting). 

To run the app with dapr, execute `./runwithdapr.sh`.

Or run the command directly: 
```bash
dapr run --app-id http-test --components-path ./components/ --config ./config/config.yaml --app-port 5000 -H 3500 -- dotnet run --project .
``` 

In a different terminal, send a request to the app using `curl`:

```bash
curl http://localhost:5000/Send
```

This will generate a response similar to: 

```
Sent Traceparent: 00-39fa28e206037ae76349dd0154aec9db-f446d05a9b6fa499-00
Received Traceparent: 00-39fa28e206037ae76349dd0154aec9db-07e891d91fb093ee-01, Tracestate: 
```

We want to see the service return the return the same `traceid` but a different `spanid` (representing the span from `daprd`).  Note that in current versions of Dpar (~v1.9) the HTTP Binding does not propogate these headers. You'll need a build that includes the propogation logic (likely v1.10).

Traceparent format: `version - traceid - spanid - traceflags`

## What it does

```
[ GET /send ] --> [ DAPR Binding ] --> [ POST /receive ]
```

The /send endpoint creates a request and posts it to the daprd sidecar which is configured with an HTTP Binding. The request includes a `Traceparent` header assigned the value of `System.Diagnostics.Activity.Current.Id` 

The HTTP binding points back at localhost and the request we send uses the /receive endpoint. If working correctly, Dapr will for include a new `Traceparent` that includes the `traceid` from our request and a new `spanid`. 
