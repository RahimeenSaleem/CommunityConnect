using Microsoft.AspNetCore.Mvc;
using CommunityResourceAssistant.Models;
using CommunityResourceAssistant.Database;

namespace CommunityResourceAssistant.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResourceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchTerm)
        {
            var resources = _context.Resources.ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                resources = resources
                    .Where(r =>
                        r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        r.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        r.City.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(resources);
        }

        public IActionResult Details(int id)
        {
            var resource = _context.Resources
                .FirstOrDefault(r => r.Id == id);

            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Resource resource)
        {
            if (ModelState.IsValid)
            {
                _context.Resources.Add(resource);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(resource);
        }

        public IActionResult Edit(int id)
        {
            var resource = _context.Resources
                .FirstOrDefault(r => r.Id == id);

            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        [HttpPost]
        public IActionResult Edit(Resource updatedResource)
        {
            var resource = _context.Resources
                .FirstOrDefault(r => r.Id == updatedResource.Id);

            if (resource == null)
            {
                return NotFound();
            }

            resource.Name = updatedResource.Name;
            resource.Category = updatedResource.Category;
            resource.Description = updatedResource.Description;
            resource.ContactInfo = updatedResource.ContactInfo;
            resource.City = updatedResource.City;
            resource.Hours = updatedResource.Hours;
            resource.Website = updatedResource.Website;
            resource.Languages = updatedResource.Languages;
            resource.IsVerified = updatedResource.IsVerified;
            resource.Status = updatedResource.Status;
            resource.Availability = updatedResource.Availability;
            resource.LastUpdated = updatedResource.LastUpdated;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var resource = _context.Resources
                .FirstOrDefault(r => r.Id == id);

            if (resource == null)
            {
                return NotFound();
            }

            _context.Resources.Remove(resource);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}