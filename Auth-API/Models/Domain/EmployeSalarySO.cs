using System.ComponentModel.DataAnnotations.Schema;

namespace ADMitroSremEmploye.Models.Domain
{
    public class EmployeSalarySO
    {
        public Guid Id { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal DeductionPension { get; set; }
        public decimal DeductionHealth { get; set; }
        public decimal ExpenseOfTheEmployer{ get; set; }
        public Guid EmployeSalaryId { get; set; }
        //public EmployeSalary? EmployeSalary { get; set; }
    }
}
