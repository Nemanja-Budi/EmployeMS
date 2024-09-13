namespace ADMitroSremEmploye.Models.Domain.MP.Ulaz.Kalkulacija
{
    public class KalkulacijaStavke : UlazniPodaci
    {
        public decimal Kolicina { get; set; }
        public decimal UlaznaCena { get; set; }
        public int PDV { get; set; }
        public decimal NabavnaCena { get; set; }
        public decimal NabavnaVrednost { get; set; }
        public decimal VrednostRobeBezPdv { get; set; }
        public decimal PdvUDin { get; set; }
        public decimal VrednostRobeSaPdv { get; set; }
        public decimal CenaProizvodaBezPdv { get; set; }
        public decimal CenaProizvodaSaPdv { get; set; }
    }
}
