using Patients.DTOs;

namespace Patients.Services.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientReadDto>> GetAllAsync();
        Task<PatientReadDto?> GetByIdAsync(int id);
        Task<PatientReadDto> CreateAsync(PatientCreateDto dto);
        Task<PatientReadDto?> UpdateAsync(int id, PatientUpdateDto dto);
    }
}
