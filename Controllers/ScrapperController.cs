using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;


namespace Scraper.Controllers
{
    public class ScrapperController : Controller
    {


        public IActionResult Scrap()
        {

            return View();
        }

        public async Task<IActionResult> HTTPScrap(string scrapQuery)
        {
            ViewBag.ScrapResult = scrapQuery;

            if (string.IsNullOrEmpty(scrapQuery))
            {
                return BadRequest("Where the url???");
            }

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(scrapQuery);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            //Search lectures from html
            var lectures = htmlDocument.DocumentNode.Descendants("div")
            .Where(node => node.GetAttributeValue("style", "")
            .Contains("background-color: #4ec400")).ToList();


            //Searh labaratorium from html
            var labaratorium = htmlDocument.DocumentNode.Descendants("div")
           .Where(node => node.GetAttributeValue("cw", "").Contains("100")).ToList();

            var labaratorium1 = htmlDocument.DocumentNode.Descendants("div")
           .Where(node => node.GetAttributeValue("mtp", "").Contains("2")).ToList();

            var english = htmlDocument.DocumentNode.Descendants("div")
            .Where(node => node.GetAttributeValue("mtp", "").Contains("1")).ToList();

            //Save the names in lectures
            HashSet<string> displayedSubjects = new HashSet<string>();
            List<string> displayedTutors_lab = new List<string>();
            List<string> resultsFrontend = new List<string>();

            foreach (var div in lectures)
            {

                //Extract name of Tutor 
                var a = div.SelectSingleNode("a");
                string hrefvalue = a.GetAttributeValue("href", string.Empty);

                var TutorID = "https://plany.ubb.edu.pl/" + hrefvalue + "&winW=1904&winH=941&loadBG=000000";
                var httpClient1 = new HttpClient();
                var html1 = await httpClient1.GetStringAsync(TutorID);
                var htmlDocument1 = new HtmlDocument();
                htmlDocument1.LoadHtml(html1);

                //Search for tutor name
                var TutorName = htmlDocument1.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "")
               .Contains("title")).ToList();

                //Trim to lecture name
                string innertext = div.InnerText.Trim().ToString();
                string textBeforeComma = innertext.Split(',')[0];

                //Search Form of studing
                string[] words = innertext.Split();
                var textFormStudium = words.FirstOrDefault(start => start.StartsWith("St") || start.StartsWith("NZ"));


                
                //resultsFrontend.Add($"{textBeforeComma}\t{textFormStudium}\t");
                Console.Write(textBeforeComma + '\t' + textFormStudium + '\t');

                displayedSubjects.Add(textBeforeComma);




                //select full name of tutor
                foreach (var divs_a1 in TutorName)
                {
                    string onlyTutorName = divs_a1.InnerText.Trim().ToString();

                    int startIndex = onlyTutorName.IndexOf("-");
                    int endIndex = onlyTutorName.IndexOf(",");

                    if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
                    {
                        
                        string result = onlyTutorName.Substring(startIndex + 2, endIndex - startIndex - 2);
                        resultsFrontend.Add($"{textBeforeComma}\t{textFormStudium}\t{result}");
                        Console.WriteLine(result);
                        

                    }
                }

            }

            Console.WriteLine("--------------------------------------");        

            return View("Scrap");
        }
    }
}