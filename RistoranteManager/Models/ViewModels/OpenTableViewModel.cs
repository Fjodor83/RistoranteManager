using System.ComponentModel.DataAnnotations;

namespace RistoranteManager.Models.ViewModels
{
    public class OpenTableViewModel
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }

        [Required(ErrorMessage = "Il numero di coperti è obbligatorio")]
        [Range(1, 20, ErrorMessage = "Il numero di coperti deve essere tra 1 e 20")]
        public int Covers { get; set; }
    }
}
