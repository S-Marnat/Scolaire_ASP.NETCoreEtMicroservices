using Risques.DTOs;
using Risques.Services.Interfaces;

namespace Risques.Services.Implementations
{
    public class RisqueService : IRisqueService
    {
        private readonly IPatientApiService _patientApiService;
        private readonly INoteApiService _noteApiService;
        private readonly IDateProvider _dateProvider;

        public RisqueService(IPatientApiService patientApiService, INoteApiService noteApiService, IDateProvider dateProvider)
        {
            _patientApiService = patientApiService;
            _noteApiService = noteApiService;
            _dateProvider = dateProvider;
        }

        public async Task<int> CalculerAgePatientAsync(DateTime dateDeNaissance)
        {
            var today = _dateProvider.Today;
            var age = today.Year - dateDeNaissance.Year;

            if (dateDeNaissance.Date > today.AddYears(-age))
                age--;

            return age;
        }

        public async Task<int> CalculerNombreDeclencheursAsync(List<NoteDto> notes)
        {
            var declencheurs = new List<string>
            {
                "Hémoglobine A1C",
                "Microalbumine",
                "Taille",
                "Poids",
                "Fumeur",
                "Fumeuse",
                "Anormal",
                "Cholestérol",
                "Vertiges",
                "Rechute",
                "Réaction",
                "Anticorps"
            };

            var declencheursLower = declencheurs.Select(d => d.ToLower()).ToList();
            int nombreDeclencheurs = 0;

            foreach (var note in notes)
            {
                if (note.Contenu == null)
                    continue;

                foreach (var declencheurLower in declencheursLower)
                {
                    if (note.Contenu.ToLower().Contains(declencheurLower))
                        nombreDeclencheurs++;
                }
            }

            return nombreDeclencheurs;
        }

        public async Task<RisqueDto> EvaluerRisqueAsync(int idPatient)
        {
            var patient = await _patientApiService.GetPatientByIdAsync(idPatient);
            var notes = await _noteApiService.GetNotesByPatientAsync(idPatient);
            var age = await CalculerAgePatientAsync(patient.DateDeNaissance);
            var nombreDeclencheurs = await CalculerNombreDeclencheursAsync(notes);

            string none = "Aucun risque";
            string borderline = "Risque limité";
            string inDanger = "Danger";
            string earlyOnset = "Apparition précoce";

            if (nombreDeclencheurs == 0)
                return new RisqueDto {
                    IdPatient = idPatient,
                    Risque = none
                };

            if (age >= 30)
            {
                switch (nombreDeclencheurs)
                {
                    case 2 or 3 or 4 or 5:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = borderline
                        };

                    case 6 or 7:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = inDanger
                        };

                    case >= 8:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = earlyOnset
                        };

                    default:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = none
                        };
                }
            }

            if (patient.Genre == "M")
            {
                switch (nombreDeclencheurs)
                {
                    case 3 or 4:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = inDanger
                        };

                    case >= 5:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = earlyOnset
                        };

                    default:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = none
                        };
                }
            }

            else if (patient.Genre == "F")
            {
                switch (nombreDeclencheurs)
                {
                    case 4 or 5 or 6:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = inDanger
                        };

                    case >= 7:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = earlyOnset
                        };

                    default:
                        return new RisqueDto
                        {
                            IdPatient = idPatient,
                            Risque = none
                        };
                }
            }

            else
            {
                return new RisqueDto
                {
                    IdPatient = idPatient,
                    Risque = "Impossible de déterminer le risque"
                };
            }
        }
    }
}
