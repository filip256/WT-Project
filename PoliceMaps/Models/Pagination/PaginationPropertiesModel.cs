namespace PoliceMaps.Models.Pagination
{
    public class PaginationPropertiesModel
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 1;
        public string? Filter { get; set; }
        public string? OrderByColumnId { get; set; }
        public bool OrderByDecreasing { get; set; } = false;

        public static PaginationPropertiesModel Default = new PaginationPropertiesModel
        {
            Skip = 0,
            Take = 20,
        };
    }
}
