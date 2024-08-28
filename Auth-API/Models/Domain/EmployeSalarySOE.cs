using System.ComponentModel.DataAnnotations.Schema;

namespace ADMitroSremEmploye.Models.Domain
{
    public class EmployeSalarySOE
    {
        public Guid Id { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal DeductionPension { get; set; }
        public decimal DeductionHealth { get; set; }
        public decimal DeductionUnemployment { get; set; }
        public decimal DeductionTaxRelief { get; set; }
        public decimal ExpenseOfTheEmploye { get; set; }
        public decimal NetoSalary { get; set; }
        public Guid EmployeSalaryId { get; set; }
    }
}
