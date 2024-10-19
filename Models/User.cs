using System.ComponentModel.DataAnnotations;

namespace wad_core_project.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        // Navigation properties
        public ICollection<Products> Products { get; set; }
        public ICollection<Transactions> Transactions { get; set; }
        public ICollection<Suppliers> Suppliers { get; set; }
    }
}
