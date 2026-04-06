using System.Text.Json;

namespace Group1Flight.Models
{
    public static class SessionExtensions
    {
        // Allows you to save any object as JSON in the session
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Allows you to pull that JSON back out as a real C# object
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}