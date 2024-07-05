using ADMitroSremEmploye.Models.Domain;

namespace ADMitroSremEmploye.Repositories.Salary_Service_repository
{
    public interface ISalaryServiceRepository
    {
        //IncomeFromWork
        Task<IncomeFromWork?> GetIncomFromWorkByEmployeSalaryIdAsync(Guid employeSalaryId);
        Task<IncomeFromWork> AddIncomeFromWorkAsync(IncomeFromWork incomeFromWork);
        Task<IncomeFromWork> SaveIncomeFromWorkAsync(Guid employeSalaryId, IncomeFromWork input, bool isUpdate = false);
        
        //EmployeSalarySOE
        Task<EmployeSalarySOE?> GetEmployeSalarySOEByEmployeSalaryIdAsync(Guid employeSalaryId);
        Task<EmployeSalarySOE> AddEmployeSalarySOEAsync(EmployeSalarySOE employeSalarySOE);
        Task<EmployeSalarySOE> SaveSalarySOEAsync(Guid employeSalaryId, decimal grossSalary, StateObligationsEmploye stateObligationsEmploye, bool isUpdate = false);

        //EmployeSalarySO
        Task<EmployeSalarySO?> GetEmployeSalarySOByEmployeSalaryIdAsync(Guid employeSalaryId);
        Task<EmployeSalarySO> AddEmployeSalarySOAsync(EmployeSalarySO employeSalarySO);
        Task<EmployeSalarySO> SaveSalarySOAsync(Guid employeSalaryId, decimal grossSalary, StateObligation stateObligation, bool isUpdate = false);

        //SaveChanges
        Task SaveChangesAsync();
    }
}
