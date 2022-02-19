using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace NightSwitcher.Extensions
{
    public static class Extensions
    {
        public static void ForceSetAppSettingValue<T>(this IConfiguration config, string key, T value)
        {
            var appSettingsJsonFilePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "appsettings.json");

            var json = System.IO.File.ReadAllText(appSettingsJsonFilePath);
            dynamic jsonObj = JsonSerializer.Deserialize<JsonNode>(json);

            jsonObj[key] = value;

            string output = JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });

            System.IO.File.WriteAllText(appSettingsJsonFilePath, output);
        }
    }
}
