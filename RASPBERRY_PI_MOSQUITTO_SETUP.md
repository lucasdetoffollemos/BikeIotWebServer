# Raspberry Pi Compose Deployment

This document explains how to run the full BikeIoT stack on a Raspberry Pi with Docker Compose.

Services in the stack:

- `postgres`
- `mosquitto`
- `api`

This setup replaces the old native Mosquitto service on the Raspberry Pi.

## 1. Remove the Old Native Mosquitto Service

Stop and disable the service:

```bash
sudo systemctl stop mosquitto
sudo systemctl disable mosquitto
```

Remove the package:

```bash
sudo apt remove --purge -y mosquitto mosquitto-clients
sudo apt autoremove -y
```

Verify removal:

```bash
systemctl status mosquitto --no-pager
dpkg -l | grep mosquitto
which mosquitto
```

## 2. Copy the Repository to the Raspberry Pi

Clone or copy this repository to the Raspberry Pi, then open the repository root.

Example:

```bash
git clone <repo-url>
cd BikeIotWebServer
```

## 3. Create the Mosquitto Password File

Create the MQTT user used by the app:

```bash
docker run --rm -it -v "$(pwd)/mosquitoConf:/mosquitto/config" eclipse-mosquitto:2 mosquitto_passwd -c /mosquitto/config/passwd bikeiot
```

If you need to update the password later:

```bash
docker run --rm -it -v "$(pwd)/mosquitoConf:/mosquitto/config" eclipse-mosquitto:2 mosquitto_passwd /mosquitto/config/passwd bikeiot
```

The password file is created on the Raspberry Pi and should not be committed to this repository.

## 4. Configure Environment Variables

Set the required values in your `.env` file.

MQTT settings:

```env
MQTT_HOST=mosquitto
MQTT_PORT=1883
MQTT_USERNAME=bikeiot
MQTT_PASSWORD=<same password used in mosquitoConf/passwd>
```

The Compose file already connects the API container to the `mosquitto` service name on the internal Docker network.

## 5. Start the Full Stack

From the repository root:

```bash
docker compose up -d
```

Check container status:

```bash
docker compose ps
```

## 6. Verify the Services

Check logs:

```bash
docker compose logs mosquitto
docker compose logs api
docker compose logs postgres
```

If you changed configuration and want to recreate containers:

```bash
docker compose up -d --force-recreate
```

## 7. Test MQTT Authentication

Subscribe from the Raspberry Pi or another machine:

```bash
docker run --rm -it eclipse-mosquitto:2 mosquitto_sub -h <raspberry-pi-ip> -p 1883 -u bikeiot -P "<password>" -t "devices/+/telemetry" -v
```

Publish a test message:

```bash
docker run --rm eclipse-mosquitto:2 mosquitto_pub -h <raspberry-pi-ip> -p 1883 -u bikeiot -P "<password>" -t "devices/bike-001/telemetry" -m '{"velocidade":22.5,"latitude":-23.5505,"longitude":-46.6333,"timestamp":"2026-06-19T14:30:00Z"}'
```

## 8. External Device Settings

Use these settings in devices or MQTT Explorer:

- Host: Raspberry Pi IP address
- Port: `1883`
- Username: `bikeiot`
- Password: the password set with `mosquitto_passwd`

The API container uses the internal Docker hostname `mosquitto`, but devices outside Docker must use the Raspberry Pi IP or DNS name.

## 9. Notes

- `mosquitoConf/mosquitto.conf` is mounted into the container at `/mosquitto/config/mosquitto.conf`.
- The password file is mounted into the container at `/mosquitto/config/passwd`.
- Mosquitto data and logs are stored in Docker volumes managed by Compose.
- If the API fails to connect to MQTT, verify `MQTT_USERNAME` and `MQTT_PASSWORD` in `.env` match the generated Mosquitto password file.
