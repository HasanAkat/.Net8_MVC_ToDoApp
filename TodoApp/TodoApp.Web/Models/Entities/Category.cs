namespace TodoApp.Web.Models.Entities
{
    public class Category
    {
        public Guid CategoryID { get; set; }
        public string CategoryName { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
