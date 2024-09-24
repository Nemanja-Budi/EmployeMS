namespace ADMitroSremEmploye.Models.DTOs.MP
{
    public class Izvestaj
    {
        public required ProizvodIzvestaj Proizvod { get; set; }
        public required decimal UkupnaUlaznaVrednost { get; set; }
        public required decimal UkupnaIzlaznaVrednost { get; set; }
        public required decimal RazlikaVrednosti { get; set; }
        public required decimal UkupnaUlaznaKolicina { get; set; }
        public required decimal UkupnaIzlaznaKolicina { get; set; }
        public required decimal RazlikaKolicina { get; set; }
        public decimal? ProsecnaVrednost { get; set; }
    }
}
