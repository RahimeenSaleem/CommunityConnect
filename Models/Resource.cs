
using System.ComponentModel.DataAnnotations;

namespace CommunityResourceAssistant.Models
{
    public class Resource
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string ContactInfo { get; set; }

        public string City { get; set; }

        public string Hours { get; set; }

        public string Website { get; set; }

        public string Languages { get; set; }

        public bool IsVerified { get; set; }

        public string Status { get; set; }

        public string Availability { get; set; }

        public string LastUpdated { get; set; }
    }
}