namespace Web_API_004
{
    public class WebscrapeItem
    {
        public string hrefItem { get; set; }
        public string textContent { get; set; }
        public int ranking { get; set; }


        public WebscrapeItem()
        {
        }
        public WebscrapeItem(string href, string text, int index)
        {
            this.hrefItem = href;
            this.textContent = text;
            this.ranking = index;
        }

    }
}
