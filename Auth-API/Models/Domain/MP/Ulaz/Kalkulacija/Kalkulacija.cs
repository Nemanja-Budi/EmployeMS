namespace ADMitroSremEmploye.Models.Domain.MP.Ulaz.Kalkulacija
{
    public class Kalkulacija : Ulaz
    {
        public required ICollection<KalkulacijaStavke> KalkulacijaStavke { get; set; }
    }
}
