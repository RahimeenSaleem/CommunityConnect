using Microsoft.AspNetCore.Mvc;
using CommunityResourceAssistant.Database;

namespace CommunityResourceAssistant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var resources = _context.Resources.ToList();

            ViewBag.TotalResources = resources.Count;
            ViewBag.CitiesCovered = resources.Select(r => r.City).Distinct().Count();
            ViewBag.SupportAreas = resources.Select(r => r.Category).Distinct().Count();

            int verifiedCount = resources.Count(r => r.IsVerified);

            ViewBag.VerifiedPercentage = resources.Count == 0
                ? 0
                : (verifiedCount * 100) / resources.Count;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}