using System.ComponentModel.DataAnnotations;

namespace CandidateAPI.Models
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string PreferredCallTime { get; set; }

        [Url]
        public string LinkedInProfile { get; set; }

        [Url]
        public string GitHubProfile { get; set; }

        [Required]
        public string Comments { get; set; }
    }
}