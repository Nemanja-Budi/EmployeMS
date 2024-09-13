namespace ADMitroSremEmploye.Models.Domain.MP.Izlaz.Racun
{
    public class IzlazCreateRacun : IzlazCreate
    {
        public required string PIB { get; set; }
        public required string MaticniBroj { get; set; }
        public required string Primalac { get; set; }
        public Guid KomintentiId { get; set; }
    }
}
