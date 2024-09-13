namespace ADMitroSremEmploye.Models.Domain.MP.Ulaz
{
    public class UlazCreate
    {
        public required List<UlazProizvodi> Proizvodi { get; set; }
        public Guid KomintentiId { get; set; }
    }
}
