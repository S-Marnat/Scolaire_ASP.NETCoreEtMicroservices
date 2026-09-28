using Front.Services;
using Front.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Front.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientApiService _patientApiService;
        private readonly NoteApiService _noteApiService;
        private readonly RisqueApiService _risqueApiService;

        public PatientController(PatientApiService patientApiService, NoteApiService noteApiService, RisqueApiService risqueApiService)
        {
            _patientApiService = patientApiService;
            _noteApiService = noteApiService;
            _risqueApiService = risqueApiService;
        }

        // GET: Patient
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var patients = await _patientApiService.GetAllPatientsAsync();
            return View(patients);
        }

        // GET: Patient/Details/5
        [HttpGet]
        public async Task<IActionResult> FichePatient(int id)
        {
            var patient = await _patientApiService.GetPatientByIdAsync(id);
            var notes = await _noteApiService.GetNotesByPatientIdAsync(id);
            var risque = await _risqueApiService.GetRisqueByPatientIdAsync(id);

            if (patient == null)
                return NotFound();

            var viewModel = new FichePatientViewModel
            {
                Patient = patient,
                Notes = notes,
                Risque = risque
            };

            return View(viewModel);
        }

        // GET: Patient/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Patient/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _patientApiService.CreatePatientAsync(model);
            return RedirectToAction("Index");
        }

        // GET: Patient/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientApiService.GetPatientByIdAsync(id);

            if (patient == null)
                return NotFound();

            var model = new PatientEditViewModel
            {
                Id = patient.Id,
                Prenom = patient.Prenom,
                Nom = patient.Nom,
                DateDeNaissance = patient.DateDeNaissance,
                Genre = patient.Genre,
                Adresse = patient.Adresse,
                Telephone = patient.Telephone
            };

            return View(model);
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _patientApiService.UpdatePatientAsync(id, model);
            return RedirectToAction("Index");
        }

        // GET: Patient/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientApiService.GetPatientByIdAsync(id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // POST: Patient/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _patientApiService.DeletePatientAsync(id);
            return RedirectToAction("Index");
        }
    }
}
