using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlimDX.DirectInput;

namespace ETS2_Local_Radio_server
{
    public static class Favourites
    {
        public static Dictionary<string, IEnumerable<string>> storage = new Dictionary<string, IEnumerable<string>>();

        public static void Set(string key, string value)
        {
            if (storage.ContainsKey(key))
            {
                storage[key] = storage[key].Append(value);
            }
            else
            {
                storage.Add(key, new string[] { value });
            }
        }

        public static string Get(string key)
        {
            if (key == "")
            {
                return JsonConvert.SerializeObject(storage, Formatting.Indented);
            }
            if (storage.ContainsKey(key))
            {
                return JsonConvert.SerializeObject(storage[key]);
            }
            else
            {
                return null;
            }
        }

        public static void Load()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\favourites.json"))
            {
                System.IO.StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "\\favourites.json");
                string json = reader.ReadToEnd();
                reader.Close();
                JObject parsedJson = JObject.Parse(json);
                foreach (var property in parsedJson.Properties())
                {
                    if (property.Name != "_info")
                    {
                        if (property.Value.Type == JTokenType.Array)
                        {
                            storage = JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(json);
                            return;
                        }
                        break;
                    }
                }

                // Convert v1 format
                Dictionary<string, string> oldStorage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (oldStorage != null)
                {
                    storage = new Dictionary<string, IEnumerable<string>>();
                    foreach (var station in oldStorage)
                    {
                        storage[station.Key] = new string[] { station.Value };
                    }
                    Save();
                }
            }
            if (storage == null)
            {
                storage = new Dictionary<string, IEnumerable<string>>();
                storage.Add("_info", new string[] { "File to store favourite station on country - station name array basis" });
            }
        }

        public static void Save()
        {
            System.IO.StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\favourites.json");
            writer.Write(JsonConvert.SerializeObject(storage, Formatting.Indented));
            writer.Close();
        }
    }
}
