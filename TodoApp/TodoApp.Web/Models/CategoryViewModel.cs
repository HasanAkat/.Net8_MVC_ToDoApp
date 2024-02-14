namespace TodoApp.Web.Models
{
    public class CategoryViewModel
    {
        public Guid CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int TaskCount { get; set; }

        // Yeni kategori adını tutacak özellik
        public string NewCategoryName { get; set; }
    }
}
