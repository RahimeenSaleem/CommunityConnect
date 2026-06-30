using Microsoft.AspNetCore.Mvc;
using CommunityResourceAssistant.Database;

namespace CommunityResourceAssistant.Controllers
{
    public class AiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ask(string personType, string situation, string urgency, string details)
        {
            ViewBag.PersonType = personType;
            ViewBag.Situation = situation;
            ViewBag.Urgency = urgency;
            ViewBag.Details = details;

            List<string> categories = new List<string>();
            List<string> reasons = new List<string>();

            string fullText = ((situation ?? "") + " " + (details ?? "")).ToLower();

            if (fullText.Contains("food") ||
                fullText.Contains("groceries") ||
                fullText.Contains("meal") ||
                fullText.Contains("hungry"))
            {
                categories.Add("Food");
                reasons.Add("Food support was recommended because the intake mentions food, groceries, meals, or hunger.");
            }

            if (fullText.Contains("rent") ||
                fullText.Contains("housing") ||
                fullText.Contains("shelter") ||
                fullText.Contains("eviction"))
            {
                categories.Add("Housing");
                reasons.Add("Housing support was recommended because the intake mentions rent, housing, shelter, or eviction concerns.");
            }

            if (fullText.Contains("job") ||
                fullText.Contains("work") ||
                fullText.Contains("employment") ||
                fullText.Contains("resume"))
            {
                categories.Add("Employment");
                reasons.Add("Employment support was recommended because the intake mentions job loss, work, employment, or resume help.");
            }

            if (fullText.Contains("doctor") ||
                fullText.Contains("health") ||
                fullText.Contains("clinic") ||
                fullText.Contains("medicine"))
            {
                categories.Add("Healthcare");
                reasons.Add("Healthcare support was recommended because the intake mentions health, medical care, clinics, doctors, or medicine.");
            }

            if (fullText.Contains("legal") ||
                fullText.Contains("lawyer") ||
                fullText.Contains("court") ||
                fullText.Contains("rights"))
            {
                categories.Add("Legal Aid");
                reasons.Add("Legal aid was recommended because the intake mentions legal help, lawyers, court, or rights concerns.");
            }

            var matchingResources = _context.Resources
                .Where(r => categories.Contains(r.Category))
                .ToList();

            ViewBag.Categories = categories;
            ViewBag.Reasons = reasons;
            ViewBag.MatchingResources = matchingResources;

            return View("Index");
        }
    }
}