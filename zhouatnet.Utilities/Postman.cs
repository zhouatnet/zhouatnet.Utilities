using System;
using System.Collections.Generic;
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


        public static async Task<HttpResponseMessage> PostForm(string url, Dictionary<string, string> formData = null, Action<HttpResponseMessage> success = null, Action<Exception> error = null, string token = null)
        {
            return await Ajax(url, HttpMethod.Post, formData, success, error, token);
        }

        public static async Task<HttpResponseMessage> GetByForm(string url, Dictionary<string, string> formData = null, Action<HttpResponseMessage> success = null, Action<Exception> error = null, string token = null)
        {
            return await Ajax(url, HttpMethod.Get, formData, success, error, token);
        }


        public static async Task<HttpResponseMessage> Post(string url, object data = null, Action<HttpResponseMessage> success = null, Action<Exception> error = null, string token = null)
        {
            return await Ajax(url, HttpMethod.Post, data, success, error, token);
        }

        public static async Task<HttpResponseMessage> Get(string url, object data = null, Action<HttpResponseMessage> success = null, Action<Exception> error = null, string token = null)
        {
            return await Ajax(url, HttpMethod.Get, data, success, error, token);
        }

        public static async Task<HttpResponseMessage> Ajax(string url, HttpMethod method, object data = null, Action<HttpResponseMessage> success = null, Action<Exception> error = null, string token = null)
        {
            HttpResponseMessage response = await SendAsync(url, method, data, token);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
                return response;
            }

            success?.Invoke(response);
            return response;
        }

        public static async Task<T> GetByForm<T>(string url, Dictionary<string, string> formData = null, Action<T> success = null, Action<Exception> error = null, string token = null)
        {
            return await Ajax<T>(url, HttpMethod.Get, formData, success, error, token);
        }

        public static async Task<T> PostForm<T>(string url, Dictionary<string, string> formData = null, Action<T> success = null, Action<Exception> error = null)
        {
            return await Ajax<T>(url, HttpMethod.Post, formData, success, error);
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
                IEnumerable<KeyValuePair<string, string>> formData = data as IEnumerable<KeyValuePair<string, string>>;
                if (formData != null)
                {
                    request.Content = new FormUrlEncodedContent(formData);
                }
                else
                {
                    string jsonStr = JsonConvert.SerializeObject(data);
                    request.Content = new StringContent(jsonStr);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
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
