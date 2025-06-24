namespace RistoranteManager.Models
{
    public class OrderItemExtra
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public virtual OrderItem OrderItem { get; set; }
    }
}