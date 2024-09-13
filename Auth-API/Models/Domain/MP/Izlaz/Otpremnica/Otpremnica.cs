namespace ADMitroSremEmploye.Models.Domain.MP.Izlaz.Otpremnica
{
    public class Otpremnica : Izlaz
    {
        public required ICollection<OtpremnicaStavke> OtpremnicaStavke { get; set; }
    }
}
