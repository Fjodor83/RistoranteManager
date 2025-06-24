using System.ComponentModel.DataAnnotations;

namespace RistoranteManager.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; } // antipasti, pasta, pizza, dessert
        [Required]
        public string Type { get; set; } // kitchen, pizzeria
        public bool IsCustomizable { get; set; }
    }
}