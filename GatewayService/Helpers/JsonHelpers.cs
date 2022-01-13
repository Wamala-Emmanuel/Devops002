using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace GatewayService.Helpers
{
    public class JsonHelpers
    {
        public static JsonSerializer GetSerializer()
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false
                    }
                }
            };
            serializer.Converters.Add(new StringEnumConverter());
            return serializer;
        }

        public static JsonSerializerSettings GetSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false,
                        ProcessDictionaryKeys = true
                    }
                }
            };
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }

 

        public static string SanitizeJsonData(string myJsonInput)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = true
                    }
                }
            };
            var interimObject = JsonConvert.DeserializeObject<ExpandoObject>(myJsonInput);
            var myJsonOutput = JsonConvert.SerializeObject(interimObject, jsonSerializerSettings);
            return myJsonOutput;
        }

        public static dynamic ConvertFromString(string data)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(data);
        }


        public static Dictionary<string, object> FlattenJson(JObject jObject)
        {
            return jObject.Descendants()
                .Where(j => !j.Children().Any())
                .Aggregate(
                    new Dictionary<string, object>(),
                    (props, jToken) =>
                    {
                        props.Add(jToken.Path, jToken.ToObject<object>());
                        return props;
                    });
        }
    }
}