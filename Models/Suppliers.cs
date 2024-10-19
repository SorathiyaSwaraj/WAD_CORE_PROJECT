using System.ComponentModel.DataAnnotations;

namespace wad_core_project.Models
{
    public class Suppliers
    {
        public int SuppliersId { get; set; }

        public string Name { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(10)]
        public string Contact { get; set; }

        public ICollection<Transactions> Transactions { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
