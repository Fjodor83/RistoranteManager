using System;
using System.Collections.Generic;

namespace RistoranteManager.Models.ViewModels
{
    public class ClosedOrderViewModel
    {
        public int OrderId { get; set; }
        public int TableNumber { get; set; }
        public int Covers { get; set; }
        public int UseCount { get; set; }
        public DateTime Date { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
        public decimal Total { get; set; }
    }
}