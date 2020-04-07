using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace zhouatnet.Utilities
{
    public static class Postman
    {
        private static readonly HttpClient _HttpClient;

        static Postman()
        {
            _HttpClient = new HttpClient();
        }

        public static async Task<JObject> Post(string url, object data, Action<JObject> success = null, Action<JObject> error = null)
        {
            return await Ajax(url, HttpMethod.Post, data, success, error);
        }

        public static async Task<JObject> Get(string url, object data, Action<JObject> success = null, Action<JObject> error = null)
        {
            return await Ajax(url, HttpMethod.Get, data, success, error);
        }

        public static async Task<JObject> Ajax(string url, HttpMethod method, object data, Action<JObject> success = null, Action<JObject> error = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);

            if (data != null)
            {
                string json = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(json);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            HttpResponseMessage response = await _HttpClient.SendAsync(request);
            JObject res;
            try
            {
                response.EnsureSuccessStatusCode();
                res = JObject.Parse(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                res = JObject.FromObject(new { msg = ex.ToString(), flag = false });
                error?.Invoke(res);
                return res;
            }

            success?.Invoke(res);
            return res;
        }

    }
}
