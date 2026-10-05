using Notes.DTOs;

namespace Notes.Services.Interfaces
{
    public interface INoteService
    {
        Task<List<NoteReadDto>> GetByPatientAsync(int idPatient);
        Task <NoteReadDto?> GetByIdAsync(string id);
        Task<NoteReadDto> CreateAsync(NoteCreateDto dto);
    }
}
