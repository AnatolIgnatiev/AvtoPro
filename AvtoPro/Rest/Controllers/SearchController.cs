using AvtoPro.Rest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvtoPro.Rest.Controllers
{
    public class SearchController
    {
        private readonly HttpClient client;

        public SearchController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://avto.pro");
        }

        public SearchResult Search(string query,int timeout, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "api/v1/search/query");
            request.Content = new StringContent($"{{\"Query\":\"{query}\",\"RegionId\":1}}", Encoding.UTF8, "application/json");
            var resultMessage = client.SendAsync(request, cancellationToken);
            HttpResponseMessage result;
            if (resultMessage.Wait(timeout, cancellationToken))
            {
                result = resultMessage.Result;
            }
            else
            {
                throw new TaskCanceledException();
            }
            
            return result.IsSuccessStatusCode ? JsonConvert.DeserializeObject<SearchResult>(result.Content.ReadAsStringAsync().Result) : null;
        }
    }
}
