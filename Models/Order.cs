using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class Order
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        public DeliveryStatus Status { get; set; }

        public virtual List<Product>? Products { get; set; }

        // Navigation property to the User entity
        public virtual User? User { get; set; }

        // Foreign key to the User entity
        [ForeignKey("User")]
        public int? UserID { get; set; }

        // Additional properties for user information during checkout
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"\d{2}/\d{2}", ErrorMessage = "Expiry date must be in MM/YY format")]
        public string ExpiryDate { get; set; }

        [Required]
        [StringLength(3, ErrorMessage = "CVV must be 3 digits", MinimumLength = 3)]
        public string CVV { get; set; }
    }

    public enum DeliveryStatus
    {
        Pending,
        Dispatched,
        Delivered
    }
}
