namespace wad_core_project.Models
{
    public class Products
    {
        public int ProductsId { get; set; }

        public string Name { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public ICollection<Transactions> Transactions { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
