namespace ADMitroSremEmploye.Models.Domain.MP.Ulaz.Prijemnica
{
    public class Prijemnica : Ulaz
    {
        public required ICollection<PrijemnicaStavke> PrijemnicaStavke { get; set; }
    }
}
