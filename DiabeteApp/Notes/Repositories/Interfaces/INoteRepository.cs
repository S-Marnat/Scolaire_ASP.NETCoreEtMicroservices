using Notes.Entities;

namespace Notes.Repositories.Interfaces
{
    public interface INoteRepository
    {
        Task<List<Note>> GetByPatientAsync(int idPatient);
        Task<Note?> GetByIdAsync(string id);
        Task<Note> CreateAsync(Note note);
    }
}
