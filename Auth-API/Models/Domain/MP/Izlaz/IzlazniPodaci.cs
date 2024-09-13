namespace ADMitroSremEmploye.Models.Domain.MP.Izlaz
{
    public abstract class IzlazniPodaci
    {
        public Guid Id { get; set; }
        public decimal IzlaznaKolicina { get; set; }
        public decimal IzlaznaVrednost { get; set; }
        public int PDV { get; set; }
        public decimal CenaBezPdv { get; set; }
        public decimal PdvUDin { get; set; }
        public Guid ProizvodId { get; set; }
        public Proizvod? Proizvod { get; set; }
    }
}
