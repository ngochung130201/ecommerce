namespace ecommerce.Helpers
{
    public static class Paging
    {
        // page , page size, total page
        public static  (int Page, int PageSize, int TotalPage) GetPaging(int page, int pageSize, int total)
        {
            return (page, pageSize, (int)Math.Ceiling(total / (double)pageSize));
        }
    }
}