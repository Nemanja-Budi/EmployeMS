namespace ADMitroSremEmploye.Models.Domain.MP
{
    public class Proizvod
    {
        public Guid Id { get; set; }
        public required string SifraProizvoda { get; set; }
        public required string NazivProizvoda { get; set; }
        public required string JM { get; set; }
        public int PoreskaGrupa { get; set; }
        public decimal CenaProizvoda { get; set; }
        public decimal CenaProizvodaBezPdv { get; set; }
        public decimal ZaliheProizvoda { get; set; }
        public required string NazivProizvodaZaPrikaz { get; set; }
    }
}
