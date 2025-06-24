using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RistoranteManager.Data;
using RistoranteManager.Models;
using RistoranteManager.Models.ViewModels;

namespace RistoranteManager.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders/Menu/5 (tableId)
        public async Task<IActionResult> Menu(int? tableId, string category = "antipasti")
        {
            if (tableId == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .Include(t => t.Orders)
                .ThenInclude(o => o.Items)
                .ThenInclude(i => i.Extras)
                .FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null)
            {
                return NotFound();
            }

            var activeOrder = table.Orders.FirstOrDefault(o => !o.IsClosed);
            if (activeOrder == null)
            {
                return RedirectToAction("SetCovers", "Tables", new { id = tableId });
            }

            // Get products for the selected category
            var products = await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();

            // Get dough types and extras for pizza customization
            var doughTypes = await _context.DoughTypes.ToListAsync();
            var extras = await _context.Extras.ToListAsync();

            // Create view model
            var viewModel = new MenuViewModel
            {
                TableId = table.Id,
                TableNumber = table.Number,
                Covers = table.Covers,
                Category = category,
                Products = products,
                DoughTypes = doughTypes,
                Extras = extras,
                OrderItems = activeOrder.Items.Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    TotalPrice = i.TotalPrice,
                    Category = i.Product?.Category,
                    Type = i.ProductType,
                    Customizations = i.GetCustomizations()
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Orders/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int tableId, int productId, string doughType = null, List<int> extraIds = null)
        {
            var table = await _context.Tables
                .Include(t => t.Orders)
                .FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null)
            {
                return NotFound();
            }

            var activeOrder = table.Orders.FirstOrDefault(o => !o.IsClosed);
            if (activeOrder == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Calculate total price
            decimal totalPrice = product.Price;

            // Create order item
            var orderItem = new OrderItem
            {
                OrderId = activeOrder.Id,
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                ProductType = product.Type,
                DoughType = doughType
            };

            // Add dough price if applicable
            if (!string.IsNullOrEmpty(doughType))
            {
                var selectedDough = await _context.DoughTypes.FirstOrDefaultAsync(d => d.Name == doughType);
                if (selectedDough != null)
                {
                    totalPrice += selectedDough.AdditionalPrice;
                }
            }

            // Add extras if applicable
            if (extraIds != null && extraIds.Any())
            {
                var selectedExtras = await _context.Extras
                    .Where(e => extraIds.Contains(e.Id))
                    .ToListAsync();

                foreach (var extra in selectedExtras)
                {
                    orderItem.Extras = orderItem.Extras ?? new List<OrderItemExtra>();
                    orderItem.Extras.Add(new OrderItemExtra
                    {
                        Name = extra.Name,
                        Price = extra.Price
                    });

                    totalPrice += extra.Price;
                }
            }

            orderItem.TotalPrice = totalPrice;

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            // Redirect back to menu
            return RedirectToAction("Menu", new { tableId = tableId, category = product.Category });
        }

        // POST: Orders/RemoveItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int id, int tableId)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Summary", new { tableId = tableId });
        }

        // GET: Orders/Summary/5 (tableId)
        public async Task<IActionResult> Summary(int? tableId)
        {
            if (tableId == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .Include(t => t.Orders)
                .ThenInclude(o => o.Items)
                .ThenInclude(i => i.Extras)
                .FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null)
            {
                return NotFound();
            }

            var activeOrder = table.Orders.FirstOrDefault(o => !o.IsClosed);
            if (activeOrder == null)
            {
                return NotFound();
            }

            var viewModel = new OrderViewModel
            {
                TableId = table.Id,
                TableNumber = table.Number,
                Covers = table.Covers,
                Items = activeOrder.Items.Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    TotalPrice = i.TotalPrice,
                    Category = i.Product?.Category,
                    Type = i.ProductType,
                    Customizations = i.GetCustomizations()
                }).ToList(),
                Total = activeOrder.Total
            };

            return View(viewModel);
        }

        // POST: Orders/SendOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.IsSent = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ordine inviato con successo!";
            return RedirectToAction("Index", "Tables");
        }

        // GET: Orders/Receipt/5 (orderId)
        public async Task<IActionResult> Receipt(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.Items)
                .ThenInclude(i => i.Extras)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Group items by type
            var kitchenItems = order.Items
                .Where(i => i.ProductType == "kitchen")
                .Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    TotalPrice = i.TotalPrice,
                    Customizations = i.GetCustomizations()
                })
                .ToList();

            var pizzeriaItems = order.Items
                .Where(i => i.ProductType == "pizzeria" && i.DoughType != "Senza Glutine")
                .Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    TotalPrice = i.TotalPrice,
                    Customizations = i.GetCustomizations()
                })
                .ToList();

            var glutenFreeItems = order.Items
                .Where(i => i.ProductType == "pizzeria" && i.DoughType == "Senza Glutine")
                .Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    TotalPrice = i.TotalPrice,
                    Customizations = i.GetCustomizations()
                })
                .ToList();

            // Count dough types
            var doughSummary = order.Items
                .Where(i => i.ProductType == "pizzeria" && !string.IsNullOrEmpty(i.DoughType))
                .GroupBy(i => i.DoughType)
                .ToDictionary(g => g.Key, g => g.Count());

            var viewModel = new ReceiptViewModel
            {
                TableId = order.Id,
                TableNumber = order.Table.Number,
                Covers = order.Table.Covers,
                Date = DateTime.Now,
                KitchenItems = kitchenItems,
                PizzeriaItems = pizzeriaItems,
                GlutenFreeItems = glutenFreeItems,
                DoughSummary = doughSummary,
                Total = order.Total
            };

            return View(viewModel);
        }

        // POST: Orders/CloseOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Close the order
            order.IsClosed = true;

            // Reset the table
            order.Table.Status = TableStatus.Free;
            order.Table.Covers = 0;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tavolo chiuso con successo!";
            return RedirectToAction("Index", "Tables");
        }
    }
}