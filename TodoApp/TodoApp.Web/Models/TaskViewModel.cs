using TodoApp.Web.Models.Entities;

namespace TodoApp.Web.Models
{
    public class TaskViewModel
    {
        public Guid TaskID { get; set; }
        public string TaskTitle { get; set; }
        public string TaskContext { get; set; }
        public bool IsCompleted { get; set; }
        public Guid CategoryID { get; set; }

    }
}
