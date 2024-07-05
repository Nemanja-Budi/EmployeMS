using ADMitroSremEmploye.Models.Domain;

namespace ADMitroSremEmploye.Repositories.Employe_Salary_repository
{
    public interface IEmployeSalaryRepository
    {
        Task<Employe?> GetEmployeByIdAsync(Guid employeId);
        Task<EmployeSalary?> GetEmployeSalaryById(Guid employeSalaryId);
        Task <EmployeSalary> AddEmployeSalaryAsync(EmployeSalary employeSalary);
        Task<List<EmployeSalary>?> GetEmployeSalarysByEmployeIdAsync(Guid employeId);
        Task<bool> DeleteEmployeSalarysByEmployeIdAsync(Guid employeId);
        Task<bool> DeleteEmployeSalaryByEmployeSalaryIdAsync(Guid employeSalaryId);
        Task SaveEmployeSalaryAsync();
    }
}
