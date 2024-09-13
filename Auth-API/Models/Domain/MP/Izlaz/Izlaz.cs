namespace ADMitroSremEmploye.Models.Domain.MP.Izlaz
{
    public abstract class Izlaz
    {
        public Guid Id { get; set; }
        public required string Paritet { get; set; }
        public required string BrojFiskalnogRacuna { get; set; }
        public Guid DokumentId { get; set; }
        public Dokument? Dokument { get; set; }
    }
}
