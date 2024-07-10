using ADMitroSremEmploye.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories.Employe_repository
{
    public interface IEmployeRepository
    {
        Task<IEnumerable<Employe>> GetEmployesAsync();
        Task<Employe> GetEmployeAsync(Guid id);
        Task<Employe> CreateEmployeAsync(Employe employe);
        Task<Employe> UpdateEmployeAsync(Employe employe);
        Task<bool> DeleteEmployeAsync(Guid id);
        Task<bool> EmployeExistsAsync(Guid id);

    }
}
