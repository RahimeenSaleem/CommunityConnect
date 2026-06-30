using CommunityResourceAssistant.Models;

namespace CommunityResourceAssistant.Database
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Resources.Any())
            {
                return;
            }

            context.Resources.AddRange(
                new Resource
                {
                    Name = "Chicago Food Pantry",
                    Category = "Food",
                    Description = "Provides emergency groceries and weekly food assistance for individuals and families.",
                    ContactInfo = "(312) 555-0101",
                    City = "Chicago",
                    Hours = "Mon - Fri | 9:00 AM - 5:00 PM",
                    Website = "www.chicagofoodpantry.org",
                    Languages = "English, Spanish",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Walk-ins Welcome",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Hope Community Kitchen",
                    Category = "Food",
                    Description = "Serves hot meals daily and distributes food boxes.",
                    ContactInfo = "(312) 555-0102",
                    City = "Aurora",
                    Hours = "Daily | 11:00 AM - 7:00 PM",
                    Website = "www.hopekitchen.org",
                    Languages = "English",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Walk-ins Welcome",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Illinois Housing Support",
                    Category = "Housing",
                    Description = "Rental assistance, eviction prevention, and temporary housing referrals.",
                    ContactInfo = "(312) 555-0201",
                    City = "Chicago",
                    Hours = "Mon - Fri | 8:30 AM - 4:30 PM",
                    Website = "www.ilhousinghelp.org",
                    Languages = "English, Spanish",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Appointment Required",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Safe Shelter Network",
                    Category = "Housing",
                    Description = "Emergency shelter and transitional housing services.",
                    ContactInfo = "(312) 555-0202",
                    City = "Joliet",
                    Hours = "24 Hours",
                    Website = "www.safeshelter.org",
                    Languages = "English",
                    IsVerified = true,
                    Status = "Open 24 Hours",
                    Availability = "Emergency Services Available",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Career Development Center",
                    Category = "Employment",
                    Description = "Resume assistance, interview preparation, and career coaching.",
                    ContactInfo = "(312) 555-0301",
                    City = "Naperville",
                    Hours = "Mon - Fri | 9:00 AM - 5:00 PM",
                    Website = "www.careercenter.org",
                    Languages = "English",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Appointment Required",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Workforce Illinois",
                    Category = "Employment",
                    Description = "Employment programs, workforce training, and job placement.",
                    ContactInfo = "(312) 555-0302",
                    City = "Chicago",
                    Hours = "Mon - Sat | 8:00 AM - 6:00 PM",
                    Website = "www.workforceillinois.org",
                    Languages = "English, Spanish",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Walk-ins Welcome",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Community Health Clinic",
                    Category = "Healthcare",
                    Description = "Affordable primary care, vaccinations, and wellness services.",
                    ContactInfo = "(312) 555-0401",
                    City = "Elgin",
                    Hours = "Mon - Fri | 8:00 AM - 5:00 PM",
                    Website = "www.communityhealth.org",
                    Languages = "English, Spanish",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Appointment Required",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Family Wellness Center",
                    Category = "Healthcare",
                    Description = "Mental health counseling and family wellness programs.",
                    ContactInfo = "(312) 555-0402",
                    City = "Chicago",
                    Hours = "Mon - Fri | 10:00 AM - 6:00 PM",
                    Website = "www.familywellness.org",
                    Languages = "English, Arabic",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Appointment Required",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Legal Aid Illinois",
                    Category = "Legal Aid",
                    Description = "Free legal advice for housing, employment, and family matters.",
                    ContactInfo = "(312) 555-0501",
                    City = "Chicago",
                    Hours = "Mon - Fri | 9:00 AM - 5:00 PM",
                    Website = "www.legalaidillinois.org",
                    Languages = "English, Spanish",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Appointment Required",
                    LastUpdated = "June 2026"
                },
                new Resource
                {
                    Name = "Community Justice Project",
                    Category = "Legal Aid",
                    Description = "Legal support for civil matters and community advocacy.",
                    ContactInfo = "(312) 555-0502",
                    City = "Aurora",
                    Hours = "Mon - Fri | 8:30 AM - 4:30 PM",
                    Website = "www.communityjustice.org",
                    Languages = "English",
                    IsVerified = true,
                    Status = "Open Today",
                    Availability = "Appointment Required",
                    LastUpdated = "June 2026"
                }
            );

            context.SaveChanges();
        }
    }
}