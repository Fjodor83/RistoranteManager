using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RistoranteManager.Data;
using RistoranteManager.Models.ViewModels;

namespace RistoranteManager.Controllers
{
    public class CashRegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CashRegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CashRegister
        public async Task<IActionResult> Index()
        {
            var closedOrders = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Items)
                .ThenInclude(i => i.Extras)
                .Where(o => o.IsClosed)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var viewModel = new CashRegisterViewModel
            {
                ClosedOrders = closedOrders.Select(o => new ClosedOrderViewModel
                {
                    OrderId = o.Id,
                    TableNumber = o.Table.Number,
                    Covers = o.Table.Covers,
                    UseCount = o.Table.UseCount,
                    Date = o.CreatedAt,
                    Items = o.Items.Select(i => new OrderItemViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                        TotalPrice = i.TotalPrice,
                        Customizations = i.GetCustomizations()
                    }).ToList(),
                    Total = o.Total
                }).ToList(),
                TotalRevenue = closedOrders.Sum(o => o.Total)
            };

            return View(viewModel);
        }
    }
}