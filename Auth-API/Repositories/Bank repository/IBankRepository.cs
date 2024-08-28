using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ADMitroSremEmploye.Repositories.Bank_repository
{
    public interface IBankRepository
    {
        Task<IEnumerable<Bank>> GetBanksAsync();
        Task<Bank?> GetBankByIdAsync(Guid id);
        Task<Bank> CreateBankAsync(Bank bank);
        Task<Bank?> UpdateBankAsync(Bank bank);
        Task<bool> DeleteBankByIdAsync(Guid id);
    }
}
