# Raspberry Pi Deployment Configuration

This document describes the environment configuration used to run `BikeIotWebServer` on a Raspberry Pi with Docker Compose, PostgreSQL, and an external MQTT broker.

## Project Location on Raspberry Pi

Project folder:

```bash
/home/judilu/opt/bikeiot/BikeIotWebServer
```

The `.env` file must be in the same folder as `docker-compose.yml`.

## Environment File

File:

```bash
.env
```

Content:

```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

API_PORT=8080
POSTGRES_PORT=5432

POSTGRES_HOST=postgres
POSTGRES_DB=bikesdb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

MQTT_HOST=192.168.218.119
MQTT_PORT=1883
MQTT_USERNAME=bikeiot
MQTT_PASSWORD=

CONNECTION_STRING=Host=postgres;Port=5432;Database=bikesdb;Username=postgres;Password=postgres
```

## What Each Variable Does

- `ASPNETCORE_ENVIRONMENT=Production`
  - Runs the API in production mode.
- `ASPNETCORE_URLS=http://+:8080`
  - Makes the ASP.NET API listen on port `8080` inside the container.
- `API_PORT=8080`
  - Exposes the API on port `8080` on the Raspberry Pi.
- `POSTGRES_PORT=5432`
  - Exposes PostgreSQL on port `5432`.
- `POSTGRES_HOST=postgres`
  - Uses the Docker Compose service name for the PostgreSQL container.
- `POSTGRES_DB=bikesdb`
  - Sets the PostgreSQL database name.
- `POSTGRES_USER=postgres`
  - Sets the PostgreSQL username.
- `POSTGRES_PASSWORD=postgres`
  - Sets the PostgreSQL password.
- `MQTT_HOST=192.168.218.119`
  - Points the API container to the MQTT broker IP.
- `MQTT_PORT=1883`
  - MQTT broker port.
- `MQTT_USERNAME=bikeiot`
  - MQTT username used by the API to authenticate with Mosquitto.
- `MQTT_PASSWORD=`
  - MQTT password used by the API to authenticate with Mosquitto. Set the real value on the Raspberry Pi and do not commit it.
- `CONNECTION_STRING=Host=postgres;Port=5432;Database=bikesdb;Username=postgres;Password=postgres`
  - Connection string used by the ASP.NET application to connect to PostgreSQL.

## Notes

- `POSTGRES_HOST=postgres` works because the API and PostgreSQL run in the same Docker Compose network.
- `MQTT_HOST` uses a fixed IP because the MQTT broker is outside this Compose stack.
- `MQTT_USERNAME` and `MQTT_PASSWORD` must match the Mosquitto user created on the Raspberry Pi.
- The `.env` file is loaded automatically by Docker Compose when it is in the same directory as `docker-compose.yml`.

## How To Edit the .env File on Raspberry Pi

Open the file:

```bash
nano .env
```

Save and exit:

1. Press `Ctrl+O`
2. Press `Enter`
3. Press `Ctrl+X`

## Start the Application

From the project directory:

```bash
docker compose up --build -d
```

## Check Running Containers

```bash
docker compose ps
```

## Check Logs

```bash
docker compose logs -f api
docker compose logs -f postgres
```

## Access the API

From another machine on the same network:

```bash
http://<raspberry-pi-ip>:8080
```
