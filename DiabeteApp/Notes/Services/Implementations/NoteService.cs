using Notes.DTOs;
using Notes.Services.Interfaces;
using Notes.Repositories.Interfaces;
using Notes.Entities;

namespace Notes.Services.Implementations
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<NoteReadDto> CreateAsync(NoteCreateDto dto)
        {
            // Mapper DTO -> Domain
            var entity = new Note
            {
                Contenu = dto.Contenu,
                DateCreation = DateTime.UtcNow,
                IdPatient = dto.IdPatient
            };

            // Appeler le repository
            var created = await _noteRepository.CreateAsync(entity);

            // Mapper Domain -> DTO Read
            return ToReadDto(created);
        }

        public async Task<NoteReadDto?> GetByIdAsync(string id)
        {
            // Appeler le repository
            var entity = await _noteRepository.GetByIdAsync(id);

            if (entity == null)
                return null;

            // Mapper Domain -> DTO Read
            return ToReadDto(entity);
        }

        public async Task<List<NoteReadDto>> GetByPatientAsync(int idPatient)
        {
            // Appeler le repository
            var entities = await _noteRepository.GetByPatientAsync(idPatient);

            // Mapper Domain -> DTO Read
            return entities.Select(ToReadDto).ToList();
        }


        private static NoteReadDto ToReadDto(Note entity)
        {
            return new NoteReadDto
            {
                Id = entity.Id!,
                Contenu = entity.Contenu,
                DateCreation = entity.DateCreation,
                IdPatient = entity.IdPatient
            };
        }
    }
}
