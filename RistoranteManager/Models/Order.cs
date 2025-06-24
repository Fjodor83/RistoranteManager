using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RistoranteManager.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSent { get; set; }
        public bool IsClosed { get; set; }
        public int TableId { get; set; }
        public virtual Table Table { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }

        public decimal Total => Items != null ? Items.Sum(i => i.TotalPrice) : 0;
    }
}