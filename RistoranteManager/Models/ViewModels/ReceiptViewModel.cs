using System;
using System.Collections.Generic;

namespace RistoranteManager.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int Covers { get; set; }
        public DateTime Date { get; set; }
        public List<OrderItemViewModel> KitchenItems { get; set; }
        public List<OrderItemViewModel> PizzeriaItems { get; set; }
        public List<OrderItemViewModel> GlutenFreeItems { get; set; }
        public Dictionary<string, int> DoughSummary { get; set; }
        public decimal Total { get; set; }
    }
}