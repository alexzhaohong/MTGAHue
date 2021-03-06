﻿using Newtonsoft.Json;

namespace MagicLights.Configuration.Models
{
    [JsonObject]
    public class KeyValueConfiguration
    {
        public string? Key { get; set; }

        public string? Value { get; set; }
    }
}
