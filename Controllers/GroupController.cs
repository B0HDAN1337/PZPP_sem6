using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;

namespace Scraper.Controllers
{
    public class GroupController : Controller
    {
        public IActionResult Groups()
        {
            return View();
        }

        public IActionResult RedirectToPlan(string fullHrefValue)
        {
            TempData["ScrapResult"] = fullHrefValue;

            return RedirectToAction("Scrap", "Scrapper");
        }

        public async Task<IActionResult> GroupScrap(string menuQuery)
        {

            var httpSubmit = new HttpClient();
            var allGroups = httpSubmit.BaseAddress = new Uri("https://plany.ubb.edu.pl/right_menu_result_plan.php");
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("search", "plan"),
                new KeyValuePair<string, string>("groups", "1")
            });

            var result = await httpSubmit.PostAsync(allGroups, content);

            if (result == null)
            {
                Console.WriteLine("Post request bad");
            }

            string resultContent = await result.Content.ReadAsStringAsync();


            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(resultContent);

            var planOfStudium = htmlDocument.DocumentNode.Descendants("div")
            .Where(node => node.GetAttributeValue("id", "")
            .Contains("result_link")).ToList();

            List<string> htmlContentList = new List<string>();

            foreach (var planOfStudiumResults in planOfStudium)
            {
                var Groups = planOfStudiumResults.InnerText.Trim();

                Console.WriteLine(Groups);

                var idGroup = planOfStudiumResults.SelectSingleNode("a");
                string hrefValue = idGroup.GetAttributeValue("href", "target");

                Console.WriteLine(hrefValue);

                string fullHrefValue = "https://plany.ubb.edu.pl/" + hrefValue + "&winW=1904&winH=941&loadBG=000000";

                string htmlContent = $"<tr><td><a href = '{Url.Action("RedirectToPlan", "Group", new { fullHrefValue })}' target ='_blank'>{Groups}</a></td></tr>";
                htmlContentList.Add(htmlContent);
            }

            string allHtmlContent = string.Join("", htmlContentList);

            ViewData["HtmlContent"] = allHtmlContent;


            return View("Groups");
        }
    }
}

