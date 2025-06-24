using RistoranteManager.Models.ViewModels;
using RistoranteManager.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NMemory.Services.Contracts;
using RistoranteManager.Models;

namespace RistoranteManager.Services
{
    public class TableService : ITableService
    {
        private readonly ApplicationDbContext _context;

        public TableService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TableViewModel> GetAllTables()
        {
            return _context.Tables
                .Select(t => new TableViewModel
                {
                    Id = t.Id,
                    Number = t.Number,
                    // Converti l'enum TableStatus in string
                    Status = t.Status.ToString(), // Oppure usa una mappatura personalizzata
                    Covers = t.Covers,
                    // Correggi gli altri errori qui sotto...
                })
                .ToList();
        }

        public TableViewModel GetTableDetails(int id)
        {
            return _context.Tables
                .Where(t => t.Id == id)
                .Select(t => new TableViewModel
                {
                    Id = t.Id,
                    Number = t.Number,
                    Status = t.Status.ToString(),
                    Covers = t.Covers,
                    ItemCount = t.Orders.SelectMany(o => o.Items).Count(),
                    UseCount = t.Orders.Count(),
                    Total = t.Orders.Where(o => o.IsClosed == false)
                                   .SelectMany(o => o.Items)
                                   .Sum(oi => oi.TotalPrice)
                })
                .FirstOrDefault();
        }
        public void OpenTable(int tableId, int covers)
        {
            var table = _context.Tables.Find(tableId);
            if (table == null)
            {
                throw new ArgumentException($"Tavolo con ID {tableId} non trovato");
            }

            // Aggiorna lo stato del tavolo
            table.Status = TableStatus.Occupied;
            table.Covers = covers;

            // Crea un nuovo ordine per questo tavolo
            var order = new Order
            {
                TableId = tableId,
                CreatedAt = DateTime.Now,
                IsClosed = false,
                Items = new List<OrderItem>()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();
        }
    }
}

    // Implementa gli altri metodi dell'interfaccia...

