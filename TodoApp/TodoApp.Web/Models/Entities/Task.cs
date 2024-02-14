namespace TodoApp.Web.Models.Entities
{
    public class Task
    {
        public Guid TaskID { get; set; }
        public string TaskTitle { get; set; }
        public string TaskContext { get; set; }
        public bool IsCompleted { get; set; }
        public Guid CategoryID { get; set; }
        public Category Category { get; set; }

    }
}
