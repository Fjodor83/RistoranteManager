using System.Collections.Generic;

namespace RistoranteManager.Models.ViewModels
{
    public class OrderViewModel
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int Covers { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
        public decimal Total { get; set; }
    }
}