# Local Mosquitto Docker Setup

This document shows how to recreate the local Mosquitto broker with username/password authentication on Windows.

## Files Used

- `mosquitoConf/mosquitto.conf`
- `mosquitoConf/passwd`

Current config:

```conf
listener 1883
allow_anonymous false
password_file /etc/mosquitto/passwd
```

## 1. Remove the Old Container

```bat
docker rm -f mosquitto
```

If the container does not exist, Docker will print an error and you can ignore it.

## 2. Create or Update the Password File

Run this from `cmd.exe` in the repository root:

```bat
docker run --rm -it -v "%cd%\mosquitoConf:/mosquitto/config" eclipse-mosquitto:latest mosquitto_passwd -c /mosquitto/config/passwd bikeiot
```

This command prompts for the password for user `bikeiot` and stores only the hash in `mosquitoConf/passwd`.

If the user already exists and you want to change the password:

```bat
docker run --rm -it -v "%cd%\mosquitoConf:/mosquitto/config" eclipse-mosquitto:latest mosquitto_passwd /mosquitto/config/passwd bikeiot
```

## 3. Start the Broker Container

Run this from `cmd.exe` in the repository root:

```bat
docker run -d --name mosquitto -p 1883:1883 -v "%cd%\mosquitoConf\mosquitto.conf:/mosquitto/config/mosquitto.conf" -v "%cd%\mosquitoConf\passwd:/etc/mosquitto/passwd" eclipse-mosquitto:latest
```

## 4. Check the Logs

```bat
docker logs mosquitto
```

## 5. Test Publish

Replace `<password>` with the password you entered for `bikeiot`:

```bat
docker run --rm eclipse-mosquitto:latest mosquitto_pub -h host.docker.internal -p 1883 -u bikeiot -P "<password>" -t "devices/test-device/telemetry" -m "{\"temperature\":24.9}"
```

## 6. Test Subscribe

```bat
docker run --rm -it eclipse-mosquitto:latest mosquitto_sub -h host.docker.internal -p 1883 -u bikeiot -P "<password>" -t "devices/+/telemetry" -v
```

## 7. App Settings

Use the same credentials in the app environment:

```env
MQTT_HOST=host.docker.internal
MQTT_PORT=1883
MQTT_USERNAME=bikeiot
MQTT_PASSWORD=<password>
```

If the app runs directly on the host instead of inside Docker, `MQTT_HOST=localhost` also works.
