using Microsoft.AspNetCore.Mvc;
using RistoranteManager.Models.ViewModels;
using RistoranteManager.Services;

namespace RistoranteManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITableService _tableService;

        public HomeController(ITableService tableService)
        {
            _tableService = tableService;
        }

        public IActionResult Index()
        {
            var tables = _tableService.GetAllTables();
            return View(tables);
        }
        public IActionResult Details(int id)
        {
            // Recupera i dettagli del tavolo
            var table = _tableService.GetTableDetails(id);
            if (table == null)
            {
                return NotFound();
            }
            return View(table);
        }
        public IActionResult OpenTable(int id)
        {
            // Se hai bisogno di mostrare un form per aprire il tavolo
            var table = _tableService.GetTableDetails(id);
            if (table == null)
            {
                return NotFound();
            }

            // Crea un modello per il form di apertura tavolo
            var model = new OpenTableViewModel
            {
                TableId = table.Id,
                TableNumber = table.Number,
                Covers = 1 // Valore predefinito
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult OpenTable(OpenTableViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Chiama il servizio per aprire il tavolo
                _tableService.OpenTable(model.TableId, model.Covers);

                // Reindirizza alla pagina dei dettagli del tavolo
                return RedirectToAction(nameof(Details), new { id = model.TableId });
            }

            return View(model);
        }
    }
}
    // Altri metodi...

