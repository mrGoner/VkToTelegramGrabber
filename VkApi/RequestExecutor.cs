using System.Threading.Tasks;
using RestSharp;

namespace VkApi
{
    public class RequestExecutor
    {
        public readonly RestClient m_restClient;

        public RequestExecutor(string _baseUrl)
        {
            m_restClient = new RestClient(_baseUrl);
        }

        public string Execute(string _request)
        {
            var request = new RestRequest(_request, Method.GET);

            var response = m_restClient.Execute(request);

            return response.Content;
        }

        public Task<string> ExecuteAsync(string _request)
        {
            return Task.Factory.StartNew(() => { return Execute(_request); });
        }
    }
}