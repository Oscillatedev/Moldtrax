using Moldtrax.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Moldtrax.Common
{
    public sealed  class HttpRepository :  IDisposable
    {
        private HttpClient _client;

        private static Lazy<HttpRepository> _instance = new Lazy<HttpRepository>();
       

        public static HttpRepository Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        

       
        public Task<HttpResponseMessage> Delete()
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> Get(HttpRequestInput input)
        {
            _client = GetClient();
            _client.BaseAddress = new Uri(input.BaseUrl);
            if (input.Headers != null && input.Headers.Count > 0)
            {
                foreach (var header in input.Headers)
                {
                    if (header.Key.ToLower() == "authorization")
                    {
                        var split = header.Value.Split(' ');
                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(split[0], split[1]);
                    }
                    else
                    {
                        _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            }

            var result = await _client.GetAsync(input.Url);
            //_client.Dispose();
            return result;
            //return JsonConvert.DeserializeObject<T>(result);            
        }

        public async Task<HttpResponseMessage> Post(HttpRequestInput input)
        {
            try
            {
                _client = GetClient();
                _client.BaseAddress = new Uri(input.BaseUrl);
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                //.Accept
                //.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (input.Headers != null && input.Headers.Count > 0)
                {
                    foreach (var header in input.Headers)
                    {
                        if (header.Key.ToLower() == "authorization")
                        {
                            var split = header.Value.Split(' ');
                            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(split[0], split[1]);
                        }
                        else
                        {
                            _client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                }
                var response = await _client.PostAsync(input.Url, input.Content);
                //_client.Dispose();
                return response;
            }
            catch(Exception ex)
            {
                return null;
            }
        }



        public async Task<HttpResponseMessage> Put(HttpRequestInput input)
        {
            _client = GetClient();
            _client.BaseAddress = new Uri(input.BaseUrl);
            _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            //.Accept
            //.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (input.Headers != null && input.Headers.Count > 0)
            {
                foreach (var header in input.Headers)
                {
                    if (header.Key.ToLower() == "authorization")
                    {
                        var split = header.Value.Split(' ');
                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(split[0], split[1]);
                    }
                    else
                    {
                        _client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }
            var response = await _client.PutAsync(input.Url, input.Content);
            //_client.Dispose();
            return response;
        }

        private HttpClient GetClient()
        {
            _client = new HttpClient();
            return _client;
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}