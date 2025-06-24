using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RistoranteManager.Data;
using RistoranteManager.Models;
using RistoranteManager.Models.ViewModels;

namespace RistoranteManager.Controllers
{
    public class TablesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tables
        public async Task<IActionResult> Index()
        {
            var tables = await _context.Tables
                .Include(t => t.Orders)
                .ThenInclude(o => o.Items)
                .Where(t => !t.IsClosed)
                .Select(t => new TableViewModel
                {
                    Id = t.Id,
                    Number = t.Number,
                    Status = t.Status.ToString(),
                    Covers = t.Covers,
                    UseCount = t.UseCount,
                    ItemCount = t.Orders.Where(o => !o.IsClosed).SelectMany(o => o.Items).Count(),
                    Total = t.Orders.Where(o => !o.IsClosed).SelectMany(o => o.Items).Sum(i => i.TotalPrice)
                })
                .ToListAsync();

            return View(tables);
        }

        // GET: Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .Include(t => t.Orders)
                .ThenInclude(o => o.Items)
                .ThenInclude(i => i.Extras)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (table == null)
            {
                return NotFound();
            }

            var activeOrder = table.Orders.FirstOrDefault(o => !o.IsClosed);

            if (activeOrder == null)
            {
                // No active order, create a new one
                return View("SetCovers", new { tableId = table.Id });
            }

            // Redirect to menu
            return RedirectToAction("Menu", "Orders", new { tableId = table.Id });
        }

        // GET: Tables/SetCovers/5
        public async Task<IActionResult> SetCovers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            ViewBag.TableId = table.Id;
            ViewBag.TableNumber = table.Number;

            return View();
        }

        // POST: Tables/SetCovers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCovers(int tableId, int covers)
        {
            var table = await _context.Tables.FindAsync(tableId);
            if (table == null)
            {
                return NotFound();
            }

            table.Status = TableStatus.Occupied;
            table.Covers = covers;
            table.UseCount++;

            // Create a new order for this table
            var order = new Order
            {
                TableId = tableId,
                CreatedAt = DateTime.Now,
                IsSent = false,
                IsClosed = false
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Menu", "Orders", new { tableId = tableId });
        }

        // GET: Tables/CloseTable/5
        public async Task<IActionResult> CloseTable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .Include(t => t.Orders)
                .ThenInclude(o => o.Items)
                .ThenInclude(i => i.Extras)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (table == null)
            {
                return NotFound();
            }

            var activeOrder = table.Orders.FirstOrDefault(o => !o.IsClosed);
            if (activeOrder == null)
            {
                return NotFound();
            }

            return RedirectToAction("Receipt", "Orders", new { orderId = activeOrder.Id });
        }
    }
}