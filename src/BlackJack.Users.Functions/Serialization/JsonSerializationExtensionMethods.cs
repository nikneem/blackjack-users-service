using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlackJack.Users.Functions.Serialization
{
    internal static class JsonSerializationExtensionMethods
    {
        private static readonly JsonSerializerSettings settings;

        internal static string Serialize(this object input)
        {
            return JsonConvert.SerializeObject(input, settings);
        }

        static JsonSerializationExtensionMethods()
        {
            settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,

            };
        }
    }
}
