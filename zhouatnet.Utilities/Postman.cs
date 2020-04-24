using System;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public static async Task<JToken> Post(string url, object data = null, Action<JToken> success = null, Action<Exception> error = null, string token = null)
        {
            return await Ajax(url, HttpMethod.Post, data, success, error, token);
        }

        public static async Task<JToken> Get(string url, object data = null, Action<JToken> success = null, Action<Exception> error = null, string token = null)
        {
            return await Ajax(url, HttpMethod.Get, data, success, error, token);
        }

        public static async Task<JToken> Ajax(string url, HttpMethod method, object data = null, Action<JToken> success = null, Action<Exception> error = null, string token = null)
        {
            HttpResponseMessage response = await SendAsync(url, method, data, token);
            JToken res;
            try
            {
                response.EnsureSuccessStatusCode();
                res = JToken.Parse(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                res = null;
                error?.Invoke(ex);
                return res;
            }

            success?.Invoke(res);
            return res;
        }

        public static async Task<T> Get<T>(string url, object data = null, Action<T> success = null, Action<Exception> error = null, string token = null)
        {
            return await Ajax<T>(url, HttpMethod.Get, data, success, error, token);
        }

        public static async Task<T> Post<T>(string url, object data = null, Action<T> success = null, Action<Exception> error = null)
        {
            return await Ajax<T>(url, HttpMethod.Post, data, success, error);
        }

        public static async Task<T> Ajax<T>(string url, HttpMethod method, object data = null, Action<T> success = null, Action<Exception> error = null, string token = null)
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
                error?.Invoke(ex);
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

        /// <summary>
        /// 向授权服务器获取AccessToken
        /// </summary>
        /// <param name="authenticationServerUrl">授权服务器地址</param>
        /// <param name="clientId">客户端Id</param>
        /// <param name="clientSecret">客户端密钥</param>
        /// <param name="apiScope">apiScope</param>
        /// <returns>返回包含token的响应结果，需要另写出错处理逻辑</returns>
        public static async Task<TokenResponse> GetToken(string authenticationServerUrl, string clientId, string clientSecret, string apiScope)
        {
            // discover endpoints from metadata
            var disco = await _HttpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = authenticationServerUrl,
                Policy = new DiscoveryPolicy()
                {
                    RequireHttps = false
                }

            });
            if (disco.IsError)
            {
                return new TokenResponse(new Exception(disco.Error));
            }

            // request token
            TokenResponse tokenResponse = await _HttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = apiScope
            });

            return tokenResponse;
        }


    }
}
