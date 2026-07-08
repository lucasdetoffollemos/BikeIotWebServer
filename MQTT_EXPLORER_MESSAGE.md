# MQTT Explorer Message Example

Use this topic in MQTT Explorer:

```text
devices/bike-001/telemetry
```

This matches the subscriber topic filter:

```text
devices/+/telemetry
```

Use this example JSON payload:

```json
{
  "bikeId": 123,
  "velocidade": 20.5,
  "latitude": -23.55,
  "longitude": -46.63,
  "timestamp": "2026-07-06T12:00:00Z"
}
```

You can also publish plain text for a basic connectivity test:

```text
hello from mqtt explorer
```
