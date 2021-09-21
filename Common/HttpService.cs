using Moldtrax.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Moldtrax.Common
{
    public class HttpService
    {
        private static Lazy<HttpService> _instance = new Lazy<HttpService>();
        private HttpService() { }

        public static HttpService Instance
        {
            get
            {
               
                return _instance.Value;
            }
        }


        public async Task<T> SendRequest<T>(HttpRequestInput input)
        {
            try
            {
                HttpError errorResponse = new HttpError();
                string content = string.Empty;
                var response = await HttpRepository.Instance.Get(input);
                content = await response.Content.ReadAsStringAsync();
                errorResponse = JsonConvert.DeserializeObject<HttpError>(content);
                ThrowException(errorResponse);
                // Console.WriteLine($"Content : {content}");
                var dObject = JsonConvert.DeserializeObject<T>(content);
                return dObject;
            }
            catch (Exception ex)
            {
                throw;
            }

        }



        public void ThrowException(HttpError error)
        {
            if (error != null && error.HttpCode != null)
            {
                throw new Exception(error.Message);
            }
        }

        public async Task<HttpResponseMessage> Get(HttpRequestInput input)
        {
            return await HttpRepository.Instance.Get(input);
            //var content = await response.Content.ReadAsStringAsync();
            //var dObject = JsonConvert.DeserializeObject<T>(content);
            //return dObject;
        }
        public async Task<T> PostRequest<T>(HttpRequestInput input)
        {
            HttpError errorResponse = new HttpError();
            string content = string.Empty;


            var response = await HttpRepository.Instance.Post(input);


            content = await response.Content.ReadAsStringAsync();

            errorResponse = JsonConvert.DeserializeObject<HttpError>(content);

            //if (errorResponse != null && errorResponse.Message != null && errorResponse.Message.ToLower() == "token has expired")
            //{
            //    retryAfter = null;
            //    _logger.LogInformation($"content : {content}");
            //    if (input != null && input.Headers != null && input.Headers.ContainsKey("authorization"))
            //    {
            //        //_logger.LogInformation("token has expired already, so renewing it.");                      
            //        tokenRenew = true;
            //    }
            //}

            ThrowException(errorResponse);
            var dObject = JsonConvert.DeserializeObject<T>(content);
            return dObject;
        }

        public async Task<T> PutRequest<T>(HttpRequestInput input, bool isXML = false)
        {
            HttpError errorResponse = new HttpError();
            string content = string.Empty;

            var response = await HttpRepository.Instance.Put(input);

            content = await response.Content.ReadAsStringAsync();

            if (isXML)
            {
                return (default(T));
            }

            errorResponse = JsonConvert.DeserializeObject<HttpError>(content);


            //if (errorResponse != null && errorResponse.Message != null && errorResponse.Message.ToLower() == "token has expired")
            //{
            //    retryAfter = null;
            //    _logger.LogInformation($"content : {content}");
            //    if (input != null && input.Headers != null && input.Headers.ContainsKey("authorization"))
            //    {
            //        //_logger.LogInformation("token has expired already, so renewing it.");                      
            //        tokenRenew = true;
            //    }
            //}


            ThrowException(errorResponse);
            var dObject = JsonConvert.DeserializeObject<T>(content);
            return dObject;
        }

        public async Task<HttpResponseMessage> Post(HttpRequestInput input)
        {
            return await HttpRepository.Instance.Post(input);

        }
    }
}


