using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;

namespace ADMitroSremEmploye.Repositories.Employe_Salary_repository
{
    public interface IEmployeSalaryRepository
    {
        Task<Employe?> GetEmployeByIdAsync(Guid employeId);
        Task<EmployeSalary?> GetEmployeSalaryById(Guid employeSalaryId);
        Task<IEnumerable<EmployeSalary>> GetAllEmployeSalarysAsync(EmployeSalaryFilterDto filterDto, string? sortBy, bool isAscending, int pageNumber, int pageSize);
        Task<int> GetTotalEmployeSalariesCountAsync(EmployeSalaryFilterDto filterDto);
        Task<EmployeSalary> AddEmployeSalaryAsync(EmployeSalary employeSalary);
        Task<List<EmployeSalary>?> GetEmployeSalarysByEmployeIdAsync(Guid employeId);
        Task<bool> DeleteEmployeSalarysByEmployeIdAsync(Guid employeId);
        Task<bool> DeleteEmployeSalaryByEmployeSalaryIdAsync(Guid employeSalaryId);
        Task SaveEmployeSalaryAsync();
    }
}
