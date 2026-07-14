using Microsoft.AspNetCore.Mvc;
using CommunityResourceAssistant.Database;
using CommunityResourceAssistant.Services;

namespace CommunityResourceAssistant.Controllers
{
    public class AiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeminiService _geminiService;

        public AiController(
            ApplicationDbContext context,
            GeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ask(
            string personType,
            string situation,
            string urgency,
            string details)
        {
            ViewBag.PersonType = personType;
            ViewBag.Situation = situation;
            ViewBag.Urgency = urgency;
            ViewBag.Details = details;

            string fullText =
                ((situation ?? "") + " " + (details ?? "")).Trim();

            List<string> categories = new();
            List<string> reasons = new();

            if (string.IsNullOrWhiteSpace(fullText))
            {
                ViewBag.Categories = categories;
                ViewBag.Reasons = reasons;
                ViewBag.MatchingResources =
                    _context.Resources
                        .Where(resource => false)
                        .ToList();

                ViewBag.RecommendationSource = "None";
                ViewBag.GeminiDiagnostic =
                    "No client situation was entered.";

                return View("Index");
            }

            // Try Gemini first.
            List<GeminiMatch> geminiMatches =
                await _geminiService.MatchCategoriesAsync(fullText);

            // Temporary debugging information.
            ViewBag.GeminiDiagnostic =
                _geminiService.LastDiagnostic;

            if (geminiMatches.Count > 0)
            {
                categories = geminiMatches
                    .Where(match =>
                        !string.IsNullOrWhiteSpace(match.Category))
                    .Select(match => match.Category.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                reasons = geminiMatches
                    .Where(match =>
                        !string.IsNullOrWhiteSpace(match.Category))
                    .Select(match =>
                        string.IsNullOrWhiteSpace(match.Reason)
                            ? "Gemini identified this category based on the client's situation."
                            : match.Reason.Trim())
                    .ToList();

                ViewBag.RecommendationSource = "Gemini";
            }
            else
            {
                // Original keyword fallback.
                string lowerText = fullText.ToLowerInvariant();

                if (lowerText.Contains("food") ||
                    lowerText.Contains("groceries") ||
                    lowerText.Contains("meal") ||
                    lowerText.Contains("hungry"))
                {
                    categories.Add("Food");

                    reasons.Add(
                        "Food support was recommended because the intake mentions food, groceries, meals, or hunger.");
                }

                if (lowerText.Contains("rent") ||
                    lowerText.Contains("housing") ||
                    lowerText.Contains("shelter") ||
                    lowerText.Contains("eviction") ||
                    lowerText.Contains("homeless"))
                {
                    categories.Add("Housing");

                    reasons.Add(
                        "Housing support was recommended because the intake mentions rent, housing, shelter, eviction, or homelessness.");
                }

                if (lowerText.Contains("job") ||
                    lowerText.Contains("work") ||
                    lowerText.Contains("employment") ||
                    lowerText.Contains("resume") ||
                    lowerText.Contains("laid off"))
                {
                    categories.Add("Employment");

                    reasons.Add(
                        "Employment support was recommended because the intake mentions job loss, work, employment, or resume assistance.");
                }

                if (lowerText.Contains("doctor") ||
                    lowerText.Contains("health") ||
                    lowerText.Contains("clinic") ||
                    lowerText.Contains("medicine") ||
                    lowerText.Contains("medical"))
                {
                    categories.Add("Healthcare");

                    reasons.Add(
                        "Healthcare support was recommended because the intake mentions health, medical care, clinics, doctors, or medicine.");
                }

                if (lowerText.Contains("legal") ||
                    lowerText.Contains("lawyer") ||
                    lowerText.Contains("court") ||
                    lowerText.Contains("rights") ||
                    lowerText.Contains("attorney"))
                {
                    categories.Add("Legal Aid");

                    reasons.Add(
                        "Legal aid was recommended because the intake mentions legal help, lawyers, court, attorneys, or rights concerns.");
                }

                ViewBag.RecommendationSource =
                    categories.Count > 0
                        ? "Keyword fallback"
                        : "No recommendation";
            }

            var matchingResources =
                _context.Resources
                    .Where(resource =>
                        categories.Contains(resource.Category))
                    .ToList();

            ViewBag.Categories = categories;
            ViewBag.Reasons = reasons;
            ViewBag.MatchingResources = matchingResources;

            return View("Index");
        }
    }
}