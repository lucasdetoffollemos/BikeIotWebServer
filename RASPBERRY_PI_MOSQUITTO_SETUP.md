# Mosquitto Setup on Raspberry Pi

This document explains how to install and configure Mosquitto on a Raspberry Pi, and how to copy `mosquitto.conf` to the server.

## 1. Install Mosquitto

Update packages and install Mosquitto plus client tools:

```bash
sudo apt update
sudo apt install -y mosquitto mosquitto-clients
```

## 2. Enable Mosquitto on Boot

```bash
sudo systemctl enable mosquitto
sudo systemctl start mosquitto
```

Check service status:

```bash
sudo systemctl status mosquitto --no-pager
```

## 3. Create the Mosquitto Config File

Use this content in `mosquitto.conf`:

```conf
listener 1883
allow_anonymous true
```

Important:
- Do not put a space before `#` if you add comments.
- A line like ` # comment` may fail.
- Use `# comment` instead.

## 4. Copy `mosquitto.conf` to the Raspberry Pi

### Option A: Copy from another machine with `scp`

From your local machine:

```bash
scp mosquitto.conf pi@<raspberry-pi-ip>:/home/pi/
```

Then log into the Raspberry Pi:

```bash
ssh pi@<raspberry-pi-ip>
```

Copy the file into Mosquitto's config folder:

```bash
sudo cp /home/pi/mosquitto.conf /etc/mosquitto/conf.d/mosquitto.conf
```

### Option B: If the file is already on the Raspberry Pi

```bash
sudo cp /path/to/mosquitto.conf /etc/mosquitto/conf.d/mosquitto.conf
```

## 5. Restart Mosquitto

```bash
sudo systemctl restart mosquitto
```

Check status again:

```bash
sudo systemctl status mosquitto --no-pager
```

## 6. Troubleshooting

If Mosquitto fails to start, inspect the logs:

```bash
sudo journalctl -xeu mosquitto.service --no-pager
```

Test the config directly:

```bash
sudo mosquitto -c /etc/mosquitto/mosquitto.conf -v
```

Check which config files are being loaded:

```bash
sudo grep -R "listener\|allow_anonymous\|include_dir" /etc/mosquitto
```

## 7. Test Publish/Subscribe

Subscribe:

```bash
mosquitto_sub -h localhost -t "devices/+/telemetry" -v
```

Publish a test message:

```bash
mosquitto_pub -h localhost -t "devices/bike-001/telemetry" -m '{"velocidade":22.5,"latitude":-23.5505,"longitude":-46.6333,"timestamp":"2026-06-19T14:30:00Z"}'
```

## 8. MQTT Explorer Settings

Use these settings in MQTT Explorer:

- Host: Raspberry Pi IP address
- Port: `1883`
- Topic: `devices/bike-001/telemetry`

Example payload:

```json
{
  "velocidade": 22.5,
  "latitude": -23.5505,
  "longitude": -46.6333,
  "timestamp": "2026-06-19T14:30:00Z"
}
```

## 9. Notes

- `allow_anonymous true` is fine for local testing.
- For production, prefer username/password and TLS.
- If your ASP.NET app is not running on the Raspberry Pi, change the app to connect to the Raspberry Pi IP instead of `localhost`.
