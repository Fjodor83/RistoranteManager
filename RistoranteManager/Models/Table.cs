namespace RistoranteManager.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public TableStatus Status { get; set; }
        public int Covers { get; set; }
        public int UseCount { get; set; }
        public bool IsClosed { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public enum TableStatus
    {
        Free,
        Occupied
    }
}