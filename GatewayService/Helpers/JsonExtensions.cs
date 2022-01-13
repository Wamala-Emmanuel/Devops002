using System;
using Newtonsoft.Json.Linq;

namespace GatewayService.Helpers
{
    public static class JsonExtensions
    {
        public static bool IsNullOrEmpty(this JToken token)
        {
            return token == null ||
                   token.Type == JTokenType.Array && !token.HasValues ||
                   token.Type == JTokenType.Object && !token.HasValues ||
                   token.Type == JTokenType.String && token.ToString() == string.Empty ||
                   token.Type == JTokenType.Null;
        }

        public static T RequireValue<T>(this JToken token, string field) 
        {
            var ex = new ClientFriendlyException($"{field} is required");
            try
            {
                var data = token.SelectToken(field);
                if (data == null)
                    throw ex;
                return data.ToObject<T>();
            }
            catch (Exception)
            {
                throw ex;
            }
        }

        public static DateTime? GetDate(this JToken token, string field)
        {
            try
            {
                var data = token[field] ?? token[field.ToLowerCamelCase()] ?? token.SelectToken(field);
                return DateTime.Parse(data.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Guid? GetGuid(this JToken token, string field)
        {
            try
            {
                var data = token[field] ?? token[field.ToLowerCamelCase()] ?? token.SelectToken(field);
                return Guid.Parse(data.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}