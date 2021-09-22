using Moldtrax.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Moldtrax.Common
{
    public class GraphExplorer
    {
       

        private static Lazy<GraphExplorer> _instance = new Lazy<GraphExplorer>();
     

        public static GraphExplorer Instance
        {
            get
            {
                return _instance.Value;
            }
        }


        public async Task GetToken()
        {
            var _clientID = ConfigurationManager.AppSettings["ClientID"];
            var _clientSecret = ConfigurationManager.AppSettings["Secret"];
            var body = new Dictionary<string, string>
            {
                { "client_id","_clientID"},
                { "_clientSecret", _clientSecret},
                { "scope","https://graph.microsoft.com/.default" },
                {"grant_type","client_credentials" }
            };


            var content = new StringContent(JsonConvert.SerializeObject(body),Encoding.UTF8,"application/json");

            //var headers = new Dictionary<string, string>
            //{
            //    { "Content-Type","application/x-www-form-urlencoded"},
            //};

            HttpRequestInput input = new HttpRequestInput
            {
                BaseUrl = "https://graph.microsoft.com/beta/",
                Url = "me",
                //Headers = headers,
                Content = content
            };

            var result = await HttpRepository.Instance.Post(input);
        }


    }
}