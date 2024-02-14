using System;
using System.Collections.Generic;
using TodoApp.Web.Models.Entities;

namespace TodoApp.Web.Models
{
        public class IndexViewModel
        {
            public List<Category> Categories { get; set; }
            public List<TaskWithCategoryName> Tasks { get; set; }
            public Guid SelectedCategory { get; set; }
            public bool? IsCompletedFilter { get; set; }
        }

        public class TaskWithCategoryName
        {
        public Guid TaskID { get; set; }
        public string TaskTitle { get; set; }
        public string TaskContext { get; set; }
        public bool IsCompleted { get; set; }
        public Guid CategoryID { get; set; }
        public string CategoryName { get; set; }
        }
}