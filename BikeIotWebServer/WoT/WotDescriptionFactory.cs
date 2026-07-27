using System.Text.Json;
using System.Text.Json.Nodes;

namespace BikeIotWebServer.WoT
{
    public static class WotDescriptionFactory
    {
        public static string BuildThingDescription(string httpBaseUrl, string mqttHost, int mqttPort)
        {
            var td = new JsonObject
            {
                ["@context"] = new JsonArray
                {
                    "https://www.w3.org/2022/wot/td/v1.1",
                    "https://www.w3.org/2019/wot/hypermedia",
                    new JsonObject
                    {
                        ["bikeId"] = "https://example.org/bikeId",
                        ["velocidade"] = "https://example.org/velocidade",
                        ["isLock"] = "https://example.org/isLock",
                        ["timestamp"] = "http://schema.org/DateTime",
                        ["latitude"] = "http://www.w3.org/2003/01/geo/wgs84_pos#lat",
                        ["longitude"] = "http://www.w3.org/2003/01/geo/wgs84_pos#long"
                    }
                },
                ["id"] = "urn:bikeiot:gateway",
                ["title"] = "Bike IoT Gateway",
                ["@type"] = "Thing",
                ["description"] = "Gateway Thing for receiving bike telemetry and managing bike lock state over HTTP and MQTT.",
                ["version"] = new JsonObject
                {
                    ["instance"] = "1.0.0"
                },
                ["securityDefinitions"] = new JsonObject
                {
                    ["apiKey_sc"] = new JsonObject
                    {
                        ["scheme"] = "apikey",
                        ["in"] = "header",
                        ["name"] = "X-Api-Key"
                    },
                    ["nosec_sc"] = new JsonObject
                    {
                        ["scheme"] = "nosec"
                    }
                },
                ["security"] = new JsonArray("nosec_sc"),
                ["properties"] = new JsonObject
                {
                    ["health"] = new JsonObject
                    {
                        ["title"] = "Gateway Health",
                        ["description"] = "Simple API health status.",
                        ["type"] = "boolean",
                        ["readOnly"] = true,
                        ["security"] = new JsonArray("nosec_sc"),
                        ["forms"] = new JsonArray
                        {
                            CreateForm($"{httpBaseUrl}/api/Bike/status", "GET", "application/json", "readproperty")
                        }
                    },
                    ["telemetryHistory"] = new JsonObject
                    {
                        ["title"] = "Telemetry History",
                        ["description"] = "Historical telemetry records stored by the gateway. This is not the latest live state per bike.",
                        ["type"] = "array",
                        ["readOnly"] = true,
                        ["security"] = new JsonArray("apiKey_sc"),
                        ["items"] = CreateRef("#/schemaDefinitions/BikeTelemetry"),
                        ["uriVariables"] = new JsonObject
                        {
                            ["bikeId"] = new JsonObject { ["type"] = "integer" },
                            ["from"] = new JsonObject { ["type"] = "string", ["format"] = "date-time" },
                            ["to"] = new JsonObject { ["type"] = "string", ["format"] = "date-time" },
                            ["limit"] = new JsonObject { ["type"] = "integer", ["minimum"] = 1, ["maximum"] = 500 },
                            ["offset"] = new JsonObject { ["type"] = "integer", ["minimum"] = 0 },
                            ["order"] = new JsonObject { ["type"] = "string", ["enum"] = new JsonArray("asc", "desc") }
                        },
                        ["forms"] = new JsonArray
                        {
                            CreateForm($"{httpBaseUrl}/api/Bike", "GET", "application/json", "readproperty")
                        }
                    }
                },
                ["actions"] = new JsonObject
                {
                    ["submitTelemetry"] = new JsonObject
                    {
                        ["title"] = "Submit Telemetry",
                        ["description"] = "Submit telemetry for a bike to be persisted by the gateway. Over MQTT, the device identifier is carried in the topic while the bike identifier is carried in the payload.",
                        ["security"] = new JsonArray("apiKey_sc"),
                        ["input"] = CreateRef("#/schemaDefinitions/BikeTelemetry"),
                        ["output"] = new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["received"] = new JsonObject { ["type"] = "boolean" },
                                ["bikeId"] = new JsonObject { ["type"] = "integer" },
                                ["velocidade"] = new JsonObject { ["type"] = "number" },
                                ["posicao"] = CreateRef("#/schemaDefinitions/Position"),
                                ["timestamp"] = new JsonObject
                                {
                                    ["type"] = "string",
                                    ["format"] = "date-time"
                                }
                            },
                            ["required"] = new JsonArray("received", "bikeId", "velocidade", "posicao", "timestamp")
                        },
                        ["uriVariables"] = new JsonObject
                        {
                            ["deviceId"] = new JsonObject
                            {
                                ["type"] = "string",
                                ["description"] = "MQTT device identifier used in the telemetry topic.",
                                ["pattern"] = "^[^/]+$"
                            }
                        },
                        ["forms"] = new JsonArray
                        {
                            CreateForm($"{httpBaseUrl}/api/Bike", "POST", "application/json", "invokeaction"),
                            CreateForm($"mqtt://{mqttHost}:{mqttPort}/devices/{{deviceId}}/telemetry", null, "application/json", "invokeaction")
                        }
                    },
                    ["getLockState"] = new JsonObject
                    {
                        ["title"] = "Get Lock State",
                        ["description"] = "Read the stored lock state for a specific bike.",
                        ["security"] = new JsonArray("apiKey_sc"),
                        ["input"] = new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["bikeId"] = new JsonObject { ["type"] = "integer" }
                            },
                            ["required"] = new JsonArray("bikeId")
                        },
                        ["output"] = CreateRef("#/schemaDefinitions/BikeLockState"),
                        ["uriVariables"] = new JsonObject
                        {
                            ["bikeId"] = new JsonObject { ["type"] = "integer" }
                        },
                        ["forms"] = new JsonArray
                        {
                            CreateForm($"{httpBaseUrl}/api/BikeLock/{{bikeId}}", "GET", "application/json", "invokeaction")
                        }
                    },
                    ["setLockState"] = new JsonObject
                    {
                        ["title"] = "Set Lock State",
                        ["description"] = "Update the stored lock state for a bike. A successful update also causes the gateway to publish an MQTT lock command for that bike.",
                        ["security"] = new JsonArray("apiKey_sc"),
                        ["input"] = CreateRef("#/schemaDefinitions/BikeLockCommand"),
                        ["output"] = new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["id"] = new JsonObject
                                {
                                    ["type"] = "string",
                                    ["description"] = "Server-generated lock state record identifier."
                                },
                                ["bikeId"] = new JsonObject { ["type"] = "integer" },
                                ["isLock"] = new JsonObject { ["type"] = "boolean" }
                            },
                            ["required"] = new JsonArray("id", "bikeId", "isLock")
                        },
                        ["forms"] = new JsonArray
                        {
                            CreateForm($"{httpBaseUrl}/api/BikeLock/update", "POST", "application/json", "invokeaction")
                        }
                    }
                },
                ["events"] = new JsonObject
                {
                    ["lockCommandPublished"] = new JsonObject
                    {
                        ["title"] = "Lock Command Published",
                        ["description"] = "MQTT command emitted by the gateway after a successful lock update.",
                        ["security"] = new JsonArray("apiKey_sc"),
                        ["data"] = CreateRef("#/schemaDefinitions/BikeLockCommand"),
                        ["uriVariables"] = new JsonObject
                        {
                            ["bikeId"] = new JsonObject { ["type"] = "integer" }
                        },
                        ["forms"] = new JsonArray
                        {
                            CreateForm($"mqtt://{mqttHost}:{mqttPort}/bikes/{{bikeId}}/lock", null, "application/json", "subscribeevent")
                        }
                    }
                },
                ["schemaDefinitions"] = new JsonObject
                {
                    ["BikeTelemetry"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["bikeId"] = new JsonObject { ["type"] = "integer" },
                            ["velocidade"] = new JsonObject { ["type"] = "number" },
                            ["latitude"] = new JsonObject { ["type"] = "number" },
                            ["longitude"] = new JsonObject { ["type"] = "number" },
                            ["timestamp"] = new JsonObject
                            {
                                ["type"] = "string",
                                ["format"] = "date-time"
                            }
                        },
                        ["required"] = new JsonArray("bikeId", "velocidade", "latitude", "longitude", "timestamp")
                    },
                    ["Position"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["latitude"] = new JsonObject { ["type"] = "number" },
                            ["longitude"] = new JsonObject { ["type"] = "number" }
                        },
                        ["required"] = new JsonArray("latitude", "longitude")
                    },
                    ["BikeLockState"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["bikeId"] = new JsonObject { ["type"] = "integer" },
                            ["isLock"] = new JsonObject { ["type"] = "boolean" }
                        },
                        ["required"] = new JsonArray("bikeId", "isLock")
                    },
                    ["BikeLockCommand"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["bikeId"] = new JsonObject { ["type"] = "integer" },
                            ["isLock"] = new JsonObject { ["type"] = "boolean" }
                        },
                        ["required"] = new JsonArray("bikeId", "isLock")
                    }
                }
            };

            return td.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }

        public static string BuildDiscoveryDocument(string httpBaseUrl)
        {
            var discovery = new JsonObject
            {
                ["things"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["id"] = "urn:bikeiot:gateway",
                        ["title"] = "Bike IoT Gateway",
                        ["description"] = "Gateway Thing Description entry point.",
                        ["href"] = $"{httpBaseUrl}/td"
                    }
                }
            };

            return discovery.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }

        private static JsonObject CreateRef(string value)
        {
            return new JsonObject
            {
                ["$ref"] = value
            };
        }

        private static JsonObject CreateForm(string href, string? methodName, string contentType, string operation)
        {
            var form = new JsonObject
            {
                ["href"] = href,
                ["contentType"] = contentType,
                ["op"] = new JsonArray(operation)
            };

            if (!string.IsNullOrWhiteSpace(methodName))
            {
                form["htv:methodName"] = methodName;
            }

            return form;
        }
    }
}
