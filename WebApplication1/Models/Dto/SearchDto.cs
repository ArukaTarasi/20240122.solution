namespace WebApplication1.Models.Dto
{
    public class SearchDto
    {
        // 搜尋相關
        public string? keyword { get; set; }

        public int? categoryId { get; set; } = 0; // 0 表示不根據分類編號進行搜尋

        // 排序相關
        public string? sortBy { get; set;}

        public string? sortType { get; set; } = "asc";

        //分頁相關
        public int? page { get; set; } = 1; // 第一頁

        public int? pageSize { get; set; } = 9; // 每頁顯示 9 筆
    }
}
