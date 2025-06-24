using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RistoranteManager.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string DoughType { get; set; }
        public bool IsGlutenFree => DoughType == "Senza Glutine";
        public string ProductType { get; set; } // kitchen, pizzeria
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<OrderItemExtra> Extras { get; set; }

        public List<string> GetCustomizations()
        {
            var customizations = new List<string>();
            if (!string.IsNullOrEmpty(DoughType))
            {
                customizations.Add(DoughType);
            }

            if (Extras != null && Extras.Any())
            {
                customizations.AddRange(Extras.Select(e => e.Name));
            }

            return customizations;
        }
    }
}