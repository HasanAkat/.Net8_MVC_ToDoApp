using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TodoApp.Web.Data;
using TodoApp.Web.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoApp.Web.Models;

namespace TodoApp.Web.Controllers
{
    public class ToDo : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToDo(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool TaskExists(Guid id)
        {
            return _context.Tasks.Any(e => e.TaskID == id);
        }

        [HttpGet]
        public IActionResult AddTask()
        {
            var categories = _context.Categories.ToList();

            if (categories != null && categories.Any())
            {
                // SelectList oluşturulurken CategoryID değeri ValueField, CategoryName değeri ise TextField olarak belirtilmiştir
                ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName");
            }
            else
            {
                ViewBag.Categories = new SelectList(new List<Category>(), "CategoryID");
            }

            return View();
        }



        [HttpPost]
        public IActionResult AddTask(TaskViewModel taskViewModel)
        {
            if (ModelState.IsValid)
            {
                // AddTaskViewModel'den Task entity'sine dönüştürme
                var newTask = new Models.Entities.Task
                {
                    TaskID = Guid.NewGuid(), // Yeni bir GUID oluştur
                    TaskTitle = taskViewModel.TaskTitle,
                    TaskContext = taskViewModel.TaskContext,
                    IsCompleted = taskViewModel.IsCompleted,
                    CategoryID = taskViewModel.CategoryID
                };

                // Yeni görevi Tasks tablosuna eklemek
                _context.Tasks.Add(newTask);
                _context.SaveChanges();

                // İşlemler başarıyla tamamlandıktan sonra başka bir sayfaya yönlendirme:
                return RedirectToAction("Index");
            }

            // Eğer ModelState geçerli değilse, hataları göstermek için aynı sayfaya geri dön
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName");
            return View(taskViewModel);
        }

        [HttpGet]
        public IActionResult Index(Guid? category, bool? isCompleted)
        {
            var selectedCategoryID = category ?? _context.Categories.FirstOrDefault()?.CategoryID ?? Guid.Empty;

            var tasks = _context.Tasks
                .Include(t => t.Category)
                .Where(t => (selectedCategoryID == Guid.Empty || t.CategoryID == selectedCategoryID)
                            && (!isCompleted.HasValue || t.IsCompleted == isCompleted.Value))
                .Select(t => new TaskWithCategoryName
                {
                    TaskID = t.TaskID,
                    TaskTitle = t.TaskTitle,
                    TaskContext = t.TaskContext,
                    IsCompleted = t.IsCompleted,
                    CategoryID = t.CategoryID,
                    CategoryName = t.Category.CategoryName
                })
                .ToList();

            var categories = _context.Categories.ToList();

            var model = new IndexViewModel
            {
                Categories = categories,
                Tasks = tasks,
                SelectedCategory = selectedCategoryID,
                IsCompletedFilter = isCompleted
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult EditTask(Guid id)
        {
            var task = _context.Tasks.Include(t => t.Category).FirstOrDefault(t => t.TaskID == id);

            if (task == null)
            {
                return NotFound();
            }

            // Kategorileri ViewBag'e ekleyerek düzenleme sayfasına yönlendir
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryID", "CategoryName");
            return View("EditTask", task);
        }

        [HttpPost]
        public IActionResult EditTask(Guid id, TaskViewModel editedTaskViewModel, string submitButton)
        {
            if (id != editedTaskViewModel.TaskID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTask = _context.Tasks.Include(t => t.Category).FirstOrDefault(t => t.TaskID == id);

                    if (existingTask == null)
                    {
                        return NotFound();
                    }

                    existingTask.TaskTitle = editedTaskViewModel.TaskTitle;
                    existingTask.TaskContext = editedTaskViewModel.TaskContext;
                    existingTask.IsCompleted = editedTaskViewModel.IsCompleted;
                    existingTask.CategoryID = editedTaskViewModel.CategoryID;

                    if (submitButton == "save")
                    {
                        // Save Changes
                        _context.Update(existingTask);
                        _context.SaveChanges();

                        return RedirectToAction(nameof(Index));
                    }
                    else if (submitButton == "delete")
                    {
                        // Delete Task
                        _context.Tasks.Remove(existingTask);
                        _context.SaveChanges();

                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryID", "CategoryName");
            return View(editedTaskViewModel);
        }
        
            [HttpGet]
            public IActionResult EditCategory()
            {
                var categories = _context.Categories.ToList();
                var categoryViewModels = new List<CategoryViewModel>();

                foreach (var category in categories)
                {
                    var taskCount = _context.Tasks.Count(t => t.CategoryID == category.CategoryID);

                    categoryViewModels.Add(new CategoryViewModel
                    {
                        CategoryID = category.CategoryID,
                        CategoryName = category.CategoryName,
                        TaskCount = taskCount
                    });
                }

                return View(categoryViewModels);
            }

            [HttpPost]
            public IActionResult EditCategory(CategoryViewModel editedCategory)
            {
                // Burada kategori güncelleme işlemlerini gerçekleştirebilirsiniz
                // Örneğin, editedCategory.CategoryID kullanarak ilgili kategoriyi veritabanında bulup güncelleyebilirsiniz

                // Ardından güncellenmiş kategori listesini tekrar göster
                var updatedCategories = _context.Categories.ToList();
                var updatedCategoryViewModels = new List<CategoryViewModel>();

                foreach (var category in updatedCategories)
                {
                    var taskCount = _context.Tasks.Count(t => t.CategoryID == category.CategoryID);

                    updatedCategoryViewModels.Add(new CategoryViewModel
                    {
                        CategoryID = category.CategoryID,
                        CategoryName = category.CategoryName,
                        TaskCount = taskCount
                    });
                }

                return View(updatedCategoryViewModels);
            }
        [HttpPost]
        public IActionResult DeleteCategory(Guid id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            // Burada yönlendirme yapabilirsiniz, örneğin EditCategory action'ına yönlendirme
            return RedirectToAction(nameof(EditCategory));
        }
        [HttpPost]
        public IActionResult AddCategory(CategoryViewModel model)
        {
            if (!string.IsNullOrEmpty(model.NewCategoryName))
            {
                // Yeni kategoriyi ekle
                var newCategory = new Category
                {
                    CategoryID = Guid.NewGuid(),
                    CategoryName = model.NewCategoryName
                };

                _context.Categories.Add(newCategory);
                _context.SaveChanges();
            }

            // Güncellenmiş kategori listesini tekrar göster
            var updatedCategories = _context.Categories.ToList();
            var updatedCategoryViewModels = new List<CategoryViewModel>();

            foreach (var category in updatedCategories)
            {
                var taskCount = _context.Tasks.Count(t => t.CategoryID == category.CategoryID);

                updatedCategoryViewModels.Add(new CategoryViewModel
                {
                    CategoryID = category.CategoryID,
                    CategoryName = category.CategoryName,
                    TaskCount = taskCount
                });
            }

            // EditCategory sayfasını göster
            return RedirectToAction(nameof(EditCategory));
        }

    }
}
