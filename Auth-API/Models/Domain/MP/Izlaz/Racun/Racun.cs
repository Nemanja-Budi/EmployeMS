namespace ADMitroSremEmploye.Models.Domain.MP.Izlaz.Racun
{
    public class Racun : Izlaz
    {
        public required ICollection<RacunStavke> RacunStavke { get; set; }
        public required string PIB { get; set; }
        public required string MaticniBroj { get; set; }
        public required string Primalac { get; set; }
        public Guid KomintentiId { get; set; }
        public Komintenti? Komintenti { get; set; }
    }
}
