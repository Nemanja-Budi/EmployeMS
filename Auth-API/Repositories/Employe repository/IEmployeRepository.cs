using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories.Employe_repository
{
    public interface IEmployeRepository
    {
        Task<IEnumerable<Employe>> GetEmployesAsync(EmployeFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize);
        Task<int> GetTotalEmployesCountAsync(EmployeFilterDto filterDto);
        Task<Employe> GetEmployeAsync(Guid id);
        Task<Employe> CreateEmployeAsync(Employe employe);
        Task<Employe> UpdateEmployeAsync(Employe employe);
        Task<bool> DeleteEmployeAsync(Guid id);
        Task<bool> EmployeExistsAsync(Guid id);

    }
}
