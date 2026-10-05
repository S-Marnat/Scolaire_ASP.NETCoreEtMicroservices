using Microsoft.AspNetCore.Http.HttpResults;
using Patients.DTOs;
using Patients.Entities;
using Patients.Repositories.Interfaces;
using Patients.Services.Interfaces;

namespace Patients.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<PatientReadDto> CreateAsync(PatientCreateDto dto)
        {
            // Mapper DTO -> Domain
            var entity = new Patient
            {
                Prenom = dto.Prenom,
                Nom = dto.Nom,
                DateDeNaissance = dto.DateDeNaissance,
                Genre = dto.Genre,
                Adresse = dto.Adresse,
                Telephone = dto.Telephone
            };

            // Appeler le repository
            var created = await _patientRepository.CreateAsync(entity);

            // Mapper Domain -> DTO Read
            return ToReadDto(created);
        }

        public async Task<IEnumerable<PatientReadDto>> GetAllAsync()
        {
            // Appeler le repository
            var entities = await _patientRepository.GetAllAsync();

            // Mapper Domain -> DTO Read
            return entities.Select(ToReadDto);
        }

        public async Task<PatientReadDto?> GetByIdAsync(int id)
        {
            // Appeeler le repository
            var entity = await _patientRepository.GetByIdAsync(id);

            if (entity == null)
                return null;

            // Mapper Domain -> DTO Read
            return ToReadDto(entity);
        }

        public async Task<PatientReadDto?> UpdateAsync(int id, PatientUpdateDto dto)
        {
            // Récupérer l'entité existante
            var entity = await _patientRepository.GetByIdAsync(id);

            if (entity == null)
                return null;

            // Mapper DTO -> Domain
            entity.Prenom = dto.Prenom;
            entity.Nom = dto.Nom;
            entity.DateDeNaissance = dto.DateDeNaissance;
            entity.Genre = dto.Genre;
            entity.Adresse = dto.Adresse;
            entity.Telephone = dto.Telephone;

            // Appeler le repository
            var updated = await _patientRepository.UpdateAsync(entity);

            // Mapper Domain -> DTO Read
            return ToReadDto(updated);

        }


        private static PatientReadDto ToReadDto(Patient entity)
        {
            return new PatientReadDto
            {
                Id = entity.Id,
                Prenom = entity.Prenom,
                Nom = entity.Nom,
                DateDeNaissance = entity.DateDeNaissance,
                Genre = entity.Genre,
                Adresse = entity.Adresse,
                Telephone = entity.Telephone
            };
        }
    }
}
