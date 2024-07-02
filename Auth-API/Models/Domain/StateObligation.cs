namespace ADMitroSremEmploye.Models.Domain
{
    public class StateObligation
    {
        public Guid Id { get; set; }
        public decimal PIO { get; set; } = 0.10m;
        public decimal HealthCare { get; set; } = 0.0515m;
    }
}
