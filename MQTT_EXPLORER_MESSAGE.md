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
  "velocidade": 22.5,
  "latitude": -23.5505,
  "longitude": -46.6333,
  "timestamp": "2026-06-19T14:30:00Z"
}
```

You can also publish plain text for a basic connectivity test:

```text
hello from mqtt explorer
```
