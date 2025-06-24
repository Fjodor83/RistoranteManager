namespace RistoranteManager.Models.ViewModels
{
    public class TableViewModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Status { get; set; }
        public int Covers { get; set; }
        public int UseCount { get; set; }
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
    }
}