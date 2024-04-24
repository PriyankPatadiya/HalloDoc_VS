using BLL_TaskManager.Interfaces;
using DAL_TaskManager.DataContext;
using DAL_TaskManager.DataModels;
using DAL_TaskManager.ViewModels;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_TaskManager.Repositories
{
    public  class TaskManagerRepository : ITaskManager
    {
        private readonly ApplicationDbContext _context;
        public TaskManagerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region getTasks

        public List<Task_DetailsVM> getTasks(string search)
        {
            var result = (from tasks in _context.Tasks
                          join category in _context.Categories
                          on tasks.CategoryId equals category.Id
                          select new Task_DetailsVM()
                          {
                              TaskId = tasks.Id,
                              TaskName = tasks.TaskName,
                              TaskDescription = tasks.Description,
                              Assignee = tasks.Assignee,
                              DueDate = DateTime.Parse(tasks.DueDate.ToString()),
                              Category = category.Name,
                              city = tasks.City
                          }).Where(u => String.IsNullOrEmpty(search) || u.TaskName.Contains(search));
            return result.ToList();
        }

        #endregion

        #region AddTask
        public void AddTask(Task_DetailsVM model)
        {

            DAL_TaskManager.DataModels.Category category = new DAL_TaskManager.DataModels.Category();
            category.Name = model.Category;
            _context.Categories.Add(category);
            _context.SaveChanges();

            DAL_TaskManager.DataModels.Task task = new DAL_TaskManager.DataModels.Task();
            task.TaskName = model.TaskName;
            task.Assignee = model.Assignee;
            task.CategoryId = category.Id;
            task.Description = model.TaskDescription;
            task.DueDate = DateOnly.FromDateTime((DateTime)model.DueDate);
            task.Category = model.Category;
            task.City = model.city;
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        #endregion

        #region Delete Task
        public void DeleteTask(int TaskId)
        {
            var task = _context.Tasks.Find(TaskId);
            if (task != null)
            {
                _context.Tasks.Remove(task); _context.SaveChanges();
            }
        }
        #endregion

        #region getEditData

        public Task_DetailsVM getEditData(int TaskId)
        {
            var data = _context.Tasks.FirstOrDefault(u => u.Id == TaskId);
            Task_DetailsVM model = new Task_DetailsVM
            {
                TaskName = data.TaskName,
                Assignee = data.Assignee,
                Category = data.Category,
                TaskId = TaskId,
                TaskDescription = data.Description,
                DueDate = DateTime.Parse(data.DueDate.ToString()),
                city = data.City
            };
            return model;
        }

        #endregion
    }
}
