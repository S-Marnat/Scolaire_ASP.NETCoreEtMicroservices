using Microsoft.AspNetCore.Mvc;
using Front.ViewModels;
using Front.Services;

namespace Front.Controllers
{
    public class NoteController : Controller
    {
        private readonly NoteApiService _noteApiService;

        public NoteController(NoteApiService noteApiService)
        {
            _noteApiService = noteApiService;
        }

        // GET: Note/Create?idPatient=1
        [HttpGet]
        public IActionResult Create(int idPatient)
        {
            var model = new NoteCreateViewModel
            {
                IdPatient = idPatient
            };

            return View(model);
        }

        // POST: Note/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _noteApiService.CreateNoteAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", "Impossible de créer la note.");
                return View(model);
            }

            return RedirectToAction("FichePatient", "Patient", new { id = model.IdPatient });
        }
    }
}
