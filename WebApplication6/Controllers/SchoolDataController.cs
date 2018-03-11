using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    [Produces("application/json")]
    [Route("api/SchoolData")]
    public class SchoolDataController : Controller
    {
        private const string baseUrl = "http://cbseaff.nic.in/cbse_aff/schdir_Report/AppViewdir.aspx?affno=";
        private const string urlWithAllData = baseUrl + "430020";
        private static readonly List<string> sources;

        static SchoolDataController()
        {
            sources = PrepareSourceUrls();
        }

        // GET: api/SchoolData
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(await DownloadHtmlAsync(urlWithAllData));

            CommaSeparator commaSeparator = new HeaderCommaSeparator();
            var headerData = commaSeparator.GetCommaSeparatedContent(htmlDocument);

            commaSeparator = new ValuesCommaSeparator();
            var allSchoolsData = new StringBuilder();
            var schoolsDetails = await DownloadAllSchoolsDetailsAsync();
            foreach (var schoolDetails in schoolsDetails)
            {
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(schoolDetails);
                var schoolData = commaSeparator.GetCommaSeparatedContent(htmlDocument);
                if (schoolData != null)
                {
                    allSchoolsData.Append(schoolData);
                }
            }

            var commaSeparatedContent = headerData.Append(allSchoolsData).ToString();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent(commaSeparatedContent);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "SchoolData.csv";
            return result;
        }

        private static List<string> PrepareSourceUrls()
        {
            var sources = new List<string>();
            for (int i = 430001; i < 430002; i++)
            {
                sources.Add(baseUrl + i);
            }

            return sources;
        }

        private async static Task<IEnumerable<string>> DownloadAllSchoolsDetailsAsync()
        {
            var schoolDetailTasks = new List<Task<string>>();
            foreach (var url in sources)
            {
                schoolDetailTasks.Add(DownloadHtmlAsync(url));
            }

            return await Task.WhenAll(schoolDetailTasks);
        }

        private async static Task<string> DownloadHtmlAsync(string url)
        {
            var httpClient = new HttpClient();

            var httpResponseMessage = await httpClient.GetAsync(url);
            httpResponseMessage.EnsureSuccessStatusCode();

            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}