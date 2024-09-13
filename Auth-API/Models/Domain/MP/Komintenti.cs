namespace ADMitroSremEmploye.Models.Domain.MP
{
    public class Komintenti
    {
        public Guid Id { get; set; }
        public required string Komintent { get; set; }
        public required string Mesto { get; set; }
        public required string Adresa { get; set; }
    }
}
