namespace ADMitroSremEmploye.Models.Domain
{
    public class StateObligationsEmploye : StateObligation
    {
        public decimal Unemployment { get; set; } = 0.0075m;
        public decimal TaxRelief { get; set; } = 25000;
        public decimal Tax { get; set; } = 0.10m;

        public StateObligationsEmploye()
        {
            PIO = 0.14m;
        }
    }
}
