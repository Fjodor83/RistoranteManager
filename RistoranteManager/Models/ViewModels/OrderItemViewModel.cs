using System.Collections.Generic;

namespace RistoranteManager.Models.ViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public List<string> Customizations { get; set; }
    }
}