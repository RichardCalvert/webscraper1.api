using Microsoft.AspNetCore.Mvc;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Web_API_004.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastTwoController : ControllerBase
    {

        private readonly ILogger<WeatherForecastTwoController> _logger;

        public WeatherForecastTwoController(ILogger<WeatherForecastTwoController> logger)
        {
            _logger = logger;
        }


        [HttpPost(Name = "PostWeatherForecastTwo")]
        //[HttpGet(Name = "GetWeatherForecastTwo")]
        public IEnumerable<WebscrapeItem> Post([FromBody] WebSearchInput webSearchInput)
        {

            HtmlDocument htmlDoc = new HtmlDocument();
            int resultQuantity = 100;
            //string searchterm = null;

            string searchTerm = !string.IsNullOrEmpty(webSearchInput.searchTerm) ? webSearchInput.searchTerm : "land registry search";

            //string url = "https://www.google.co.uk/search?num=100&q=land+registry+search";
            string searchTermString = searchTerm.Replace(" ", "+");


            string url = "https://www.google.co.uk/search?num=" + resultQuantity.ToString() +  "&q=" + searchTermString;


            Console.WriteLine("The url string is: " + url);

            string urlResponse = URLRequest(url);

            //Convert the Raw HTML into an HTML Object
            htmlDoc.LoadHtml(urlResponse);

            //Find all A tags in the document
            var anchorNodes = htmlDoc.DocumentNode.SelectNodes("//a").ToArray();

            var webscrapeItems = new List<WebscrapeItem>();


            for (var a = 0; a<anchorNodes.Length; a++)
            {
                int ranking = a;
                string textContent = anchorNodes[a].InnerText;
                string hrefValue = anchorNodes[a].GetAttributeValue("href", "");
                string trimmedHrefValue = hrefValue.Replace(@"/url?q=https://googleweblight.com/fp%3Fu%3D", "")
                .Replace(@"/url?q=http://googleweblight.com/fp%3Fu%3D", "")
                .Replace(@"https://", "")
                .Replace(@"http://", "");

                Regex regex = new Regex(@"(?:http(s)?://)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]");
                Match match = regex.Match(trimmedHrefValue);

                string urlValue = match.Value;

                if (
                    !String.IsNullOrEmpty(urlValue) &&
                    textContent != "NEWS" &&
                    textContent != "IMAGES" &&
                    textContent != "VIDEOS" &&
                    textContent != "Sign in" &&
                    textContent != "Settings" &&
                    textContent != "Privacy" &&
                    textContent != "Terms" &&
                    !textContent.Contains("&gt;")
                    )
                {

                    var webscrapeItem = new WebscrapeItem(urlValue, textContent, ranking);
                    webscrapeItems.Add(webscrapeItem);

                }

            }

            var filteredWebScrapeItems = new List<WebscrapeItem>();

            for (var a = 0; a<webscrapeItems.Count; a++)
            {
                if (webscrapeItems[a].hrefItem.Contains(webSearchInput.searchURL))
                {
                    webscrapeItems[a].ranking = a;
                    filteredWebScrapeItems.Add(webscrapeItems[a]);
                }
            }




                return Enumerable.Range(0, filteredWebScrapeItems.Count).Select(index => new WebscrapeItem
            {
                hrefItem = filteredWebScrapeItems[index].hrefItem,
                textContent = filteredWebScrapeItems[index].textContent,
                ranking = filteredWebScrapeItems[index].ranking
            })
                .ToArray();
        }



        //General Function to request data from a Server
        static string URLRequest(string url)
        {
            // Prepare the Request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // Set method to GET to retrieve data
            request.Method = "GET";
            request.Timeout = 6000; //60 second timeout
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";

            string responseContent = null;

            // Get the Response
            using (WebResponse response = request.GetResponse())
            {
                // Retrieve a handle to the Stream
                using (Stream stream = response.GetResponseStream())
                {
                    // Begin reading the Stream
                    using (StreamReader streamreader = new StreamReader(stream))
                    {
                        // Read the Response Stream to the end
                        responseContent = streamreader.ReadToEnd();
                    }
                }
            }

            return (responseContent);
        }


    }
}