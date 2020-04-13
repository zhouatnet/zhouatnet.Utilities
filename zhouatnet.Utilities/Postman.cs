using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
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

        public static async Task<JObject> Post(string url, object data = null, Action<JObject> success = null, Action<JObject> error = null, string token = null)
        {
            return await Ajax(url, HttpMethod.Post, data, success, error, token);
        }

        public static async Task<JObject> Get(string url, object data = null, Action<JObject> success = null, Action<JObject> error = null, string token = null)
        {
            return await Ajax(url, HttpMethod.Get, data, success, error, token);
        }

        public static async Task<JObject> Ajax(string url, HttpMethod method, object data = null, Action<JObject> success = null, Action<JObject> error = null, string token = null)
        {
            HttpResponseMessage response = await SendAsync(url, method, data, token);
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

        public static async Task<T> Get<T>(string url, HttpMethod method, object data = null, Action<T> success = null, Action<T> error = null, string token = null)
        {
            return await Ajax<T>(url, HttpMethod.Get, data, success, error, token);
        }

        public static async Task<T> Post<T>(string url, HttpMethod method, object data = null, Action<T> success = null, Action<T> error = null)
        {
            return await Ajax<T>(url, HttpMethod.Post, data, success, error);
        }

        public static async Task<T> Ajax<T>(string url, HttpMethod method, object data = null, Action<T> success = null, Action<T> error = null, string token = null)
        {
            HttpResponseMessage response = await SendAsync(url, method, data, token);
            T res;
            try
            {
                response.EnsureSuccessStatusCode();
                res = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                res = default;
                error?.Invoke(res);
                return res;
            }

            success?.Invoke(res);
            return res;
        }

        private static async Task<HttpResponseMessage> SendAsync(string url, HttpMethod method, object data = null, string token = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);

            if (data != null)
            {
                string json = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(json);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                _HttpClient.SetBearerToken(token);
            }

            HttpResponseMessage response = await _HttpClient.SendAsync(request);
            return response;
        }
    }
}
