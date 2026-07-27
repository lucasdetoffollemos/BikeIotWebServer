# Polling Demo

## Purpose

Demonstrate WoT polling by repeatedly reading the `telemetryHistory` property from the gateway Thing Description.

## Start the API

Run the gateway API first.

Example:

```powershell
dotnet run --project BikeIotWebServer/BikeIotWebServer.csproj
```

Default local URL:

```text
http://localhost:5242
```

## Start the Polling Consumer

Run the WoT consumer in polling mode:

```powershell
dotnet run --project BikeIotWotConsumer/BikeIotWotConsumer.csproj -- "http://localhost:5242" poll 1 5 change-this-api-key
```

Arguments:

1. base URL
2. `poll`
3. `bikeId`
4. interval in seconds
5. API key

This polls:

```text
GET /api/Bike?bikeId=1&limit=1&order=desc
```

through the `telemetryHistory` WoT property discovered from `/td`.

## Send New Telemetry During Polling

In another terminal, send telemetry for the same bike:

```powershell
curl -X POST "http://localhost:5242/api/Bike" ^
  -H "Content-Type: application/json" ^
  -H "X-Api-Key: change-this-api-key" ^
  -d "{\"bikeId\":1,\"velocidade\":20.5,\"latitude\":-23.55,\"longitude\":-46.63,\"timestamp\":\"2026-07-27T16:00:00Z\"}"
```

## Expected Output

The consumer prints one line per poll.

Examples:

```text
Poll 1 | 2026-07-27T16:10:00.0000000+00:00 | no telemetry yet
Poll 2 | 2026-07-27T16:10:05.0000000+00:00 | changed | bikeId=1 | velocidade=20.5 | lat=-23.55 | lon=-46.63 | ts=2026-07-27T16:00:00.0000000
Poll 3 | 2026-07-27T16:10:10.0000000+00:00 | no change | bikeId=1 | velocidade=20.5 | lat=-23.55 | lon=-46.63 | ts=2026-07-27T16:00:00.0000000
```

## What This Demonstrates

1. Thing discovery via `/.well-known/wot`
2. TD retrieval via `/td`
3. WoT property consumption
4. Repeated polling over time
5. Detection of state changes between polls
