using System.Net;

namespace StatisticService.WEB.Models
{
    public class HandleErrorInfo
    {
        public HttpStatusCode StatusCode { get; set; }

        public string StackTrace { get; set; }
    }
}