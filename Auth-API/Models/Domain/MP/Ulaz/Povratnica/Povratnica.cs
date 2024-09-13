namespace ADMitroSremEmploye.Models.Domain.MP.Ulaz.Povratnica
{
    public class Povratnica : Ulaz
    {
        public required ICollection<PovratnicaStavke> PovratnicaStavke { get; set; }
    }
}
