using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;

namespace ADMitroSremEmploye.Repositories.Employe_Salary_repository
{
    public interface IEmployeSalaryRepository
    {
        Task<Employe?> GetEmployeByIdAsync(Guid employeId);
        Task<EmployeSalary?> GetEmployeSalaryById(Guid employeSalaryId);
        Task<(int totalCount, IEnumerable<EmployeSalary>)> GetAllEmployeSalarysAsync(EmployeSalaryFilterDto filterDto, CommonFilterDto commonFilterDto);
        Task<EmployeSalary> AddEmployeSalaryAsync(EmployeSalary employeSalary);
        Task<List<EmployeSalary>?> GetEmployeSalarysByEmployeIdAsync(Guid employeId);
        Task<bool> DeleteEmployeSalarysByEmployeIdAsync(Guid employeId);
        Task<bool> DeleteEmployeSalaryByEmployeSalaryIdAsync(Guid employeSalaryId);
        Task<IEnumerable<BankSalaryDto>?> GetSalariesByBankAsync(int month, int year);
        Task<decimal> GetGrandTotalSalaryAsync(int month, int year);
        Task SaveEmployeSalaryAsync();
    }
}
