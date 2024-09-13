namespace ADMitroSremEmploye.Models.Domain.MP.Ulaz
{
    public abstract class Ulaz
    {
        public Guid Id { get; set; }
        public Guid DokumentId { get; set; }
        public Dokument? Dokument { get; set; }
        public Guid KomintentId { get; set; }
        public Komintenti? Komintent { get; set; }
    }
}
