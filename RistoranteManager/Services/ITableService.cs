using RistoranteManager.Models.ViewModels;

namespace RistoranteManager.Services
{
    public interface ITableService
    {
        // Aggiungi questo metodo all'interfaccia
        IEnumerable<TableViewModel> GetAllTables();
        TableViewModel GetTableDetails(int id);
        void OpenTable(int tableId, int covers);
        // Altri metodi esistenti...
    }
}