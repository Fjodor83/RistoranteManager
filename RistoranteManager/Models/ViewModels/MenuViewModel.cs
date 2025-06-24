using System.Collections.Generic;

namespace RistoranteManager.Models.ViewModels
{
    public class MenuViewModel
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int Covers { get; set; }
        public string Category { get; set; }
        public List<Product> Products { get; set; }
        public List<DoughType> DoughTypes { get; set; }
        public List<Extra> Extras { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}