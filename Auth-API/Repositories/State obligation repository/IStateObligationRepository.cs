using ADMitroSremEmploye.Models.Domain;

namespace ADMitroSremEmploye.Repositories.State_obligation_repository
{
    public interface IStateObligationRepository
    {
        Task<StateObligation?> GetStateObligationAsync();
        Task<StateObligationsEmploye?> GetStateObligationEmployeAsync();
    }
}
