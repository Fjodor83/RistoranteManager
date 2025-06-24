using System.Collections.Generic;

namespace RistoranteManager.Models.ViewModels
{
    public class CashRegisterViewModel
    {
        public List<ClosedOrderViewModel> ClosedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}