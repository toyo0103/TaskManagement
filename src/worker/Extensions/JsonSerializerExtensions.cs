using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskManagement.Worker.Extensions
{
    public static class JsonSerializerExtensions
    {
        public static async Task<(bool result, T data)> TryDeserializeAsync<T>(string jsonString)
        {
            try
            {
                var data = await JsonSerializer.DeserializeAsync<T>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)));
                return (true, data);
            }
            catch (Exception)
            {
                return (false, default(T));
            }
        }
    }
}
