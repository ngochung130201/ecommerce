namespace ecommerce.Helpers
{
    public static class Paging
    {
        // page , page size, total page
        public static  (int Page, int PageSize, int TotalPage) GetPaging(int page, int pageSize, int total)
        {
            var totalPage = (int)Math.Ceiling(total / (double)pageSize);
            if (page < 1)
            {
                page = 1;
            }
            return (page, pageSize, totalPage);
        }
    }
}