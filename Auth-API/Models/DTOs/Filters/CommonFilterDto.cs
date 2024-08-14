namespace ADMitroSremEmploye.Models.DTOs.Filters
{
    public class CommonFilterDto
    {
        public string? SortBy { get; set; } = null;
        public bool IsAscending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 1000;

    }
}
