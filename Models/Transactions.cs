using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace wad_core_project.Models
{
    public class Transactions
    {

        [Key]
        public int TransactionId { get; set; }

        // Foreign Key to Products
        [Required]
        [ForeignKey("Products")]
        public int ProductsId { get; set; }
        public Products Products { get; set; }

        // Foreign Key to Suppliers
        [Required]
        [ForeignKey("Suppliers")]
        public int SuppliersId { get; set; }
        public Suppliers Suppliers { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; }  // 'IN' for stock added, 'OUT' for sold

        [Required]
        public int QuantityChanged { get; set; }     // Number of items added or removed

        [Required]
        public DateTime TransactionDate { get; set; } // Date of the transaction

        // Foreign Key
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
