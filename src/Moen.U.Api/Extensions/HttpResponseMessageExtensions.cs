using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Moen.U.Api.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> ContentToAsync<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content?.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(data))
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    return JsonConvert.DeserializeObject<T>(data, settings);
                }
            }

            return default(T);
        }
        public static async Task<string> ContentToString(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return await response.Content?.ReadAsStringAsync();
            else
                return null;
        }
    }
}