using Microsoft.AspNetCore.Mvc;
using System.Net;
using HtmlAgilityPack;

namespace Web_API_004.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebscrapeController : ControllerBase
    {

        private readonly ILogger<WebscrapeController> _logger;

        public WebscrapeController(ILogger<WebscrapeController> logger)
        {
            _logger = logger;
        }


        [HttpGet(Name = "GetWebscrape")]
        public IEnumerable<Webscrape> Get()
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            int resultQuantity = 10;
            string searchterm = null;

            string searchTerm = !string.IsNullOrEmpty(searchterm) ? searchterm : "land registry search";

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


            foreach (var anchorNode in anchorNodes)
            {
                string attributeValue = anchorNode.InnerText;
                string hrefValue = anchorNode.GetAttributeValue("href", "");

                if (attributeValue != "NEWS" &&
                    attributeValue != "IMAGES" &&
                    attributeValue != "VIDEOS" &&
                    attributeValue != "Next&nbsp;&gt;" &&
                    hrefValue.Contains(searchTermString) || attributeValue.Contains(searchTerm)
                    )
                {
                    var webscrapeItem = new WebscrapeItem(hrefValue, attributeValue);
                    webscrapeItems.Add(webscrapeItem);

                    Console.WriteLine(String.Format("{0} - {1}", anchorNode.InnerText, anchorNode.GetAttributeValue("href", "")));
                }

            }



            return Enumerable.Range(1, resultQuantity).Select(index => new Webscrape
            {
                hrefItem = webscrapeItems[index].hrefItem,
                textContent = webscrapeItems[index].textContent
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