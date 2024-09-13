namespace ADMitroSremEmploye.Models.Domain.MP
{
    public class Dokument
    {
        public Guid Id { get; set; }
        public DateTime DatumDokumenta { get; set; }
        public required string NazivDokumenta { get; set; }
        public ulong BrojDokumenta { get; set; }
        public Dokument(string nazivDokumenta, ulong brojDokumenta)
        {
            Id = Guid.NewGuid();
            DatumDokumenta = DateTime.UtcNow;
            NazivDokumenta = nazivDokumenta;
            BrojDokumenta = brojDokumenta + 1;
        }
    }
}
