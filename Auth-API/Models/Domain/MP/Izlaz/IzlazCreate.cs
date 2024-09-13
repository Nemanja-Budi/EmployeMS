namespace ADMitroSremEmploye.Models.Domain.MP.Izlaz
{
    public class IzlazCreate
    {
        public required List<IzlazProizvodi> Proizvodi { get; set; }
        public required string Paritet { get; set; }
        public required string BrojFiskalnogRacuna { get; set; }
    }
}
