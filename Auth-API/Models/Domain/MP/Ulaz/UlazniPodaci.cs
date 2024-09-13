namespace ADMitroSremEmploye.Models.Domain.MP.Ulaz
{
    public abstract class UlazniPodaci
    {
        public Guid Id { get; set; }
        public decimal UlaznaKolicina { get; set; }
        public decimal UlaznaVrednost { get; set; }
        public Guid ProizvodId { get; set; }
        public Proizvod? Proizvod { get; set; }
    }
}
