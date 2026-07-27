# MQTT Lock Publish

## Overview

The server publishes bike lock commands to MQTT after a successful call to the lock update API.

Flow:
1. A client sends a lock or unlock request to the HTTP API.
2. The server updates the lock state in the database.
3. The server publishes an MQTT command for the target bike.

## HTTP Endpoint

Endpoint:
`POST /api/BikeLock/update`

Request body:

```json
{
  "bikeId": 1,
  "isLock": true
}
```

## MQTT Topic

Topic pattern:

```text
bikes/{bikeId}/lock
```

Example for bike `1`:

```text
bikes/1/lock
```

## MQTT Payload

Published payload:

```json
{
  "bikeId": 1,
  "isLock": true
}
```

Field meaning:
- `bikeId`: target bike identifier
- `isLock`: `true` locks the bike, `false` unlocks the bike

## MQTT Broker Address

The server reads broker settings from `BikeIotWebServer/appsettings.json`:

```json
"Mqtt": {
  "Host": "localhost",
  "Port": 1883,
  "Username": "bikeiot",
  "Password": "admin"
}
```

Current broker address:
- Host: `localhost`
- Port: `1883`

## Implementation Files

- `BikeIotWebServer/Mqtt/MqttPublisherService.cs`
- `BikeIotWebServer/Controllers/BikeLockController.cs`
- `BikeIotWebServer/Program.cs`

## Publish Behavior

- Creates an MQTT client
- Connects using the configured MQTT host, port, username, and password
- Publishes to `bikes/{bikeId}/lock`
- Uses QoS `AtLeastOnce`
- Disconnects after publishing

## Example

HTTP request body:

```json
{
  "bikeId": 5,
  "isLock": false
}
```

Published MQTT message:

Topic:

```text
bikes/5/lock
```

Payload:

```json
{
  "bikeId": 5,
  "isLock": false
}
```

## Bike-side Subscription

Each bike should subscribe to its own command topic.

Example for bike `5`:

```text
bikes/5/lock
```
