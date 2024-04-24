using AssignmentTask.Models;
using BLL_TaskManager.Interfaces;
using DAL_TaskManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AssignmentTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITaskManager _taskManager;

        public HomeController(ILogger<HomeController> logger, ITaskManager taskManager)
        {
            _logger = logger;
            _taskManager = taskManager;
        }


        //Index

        public IActionResult Index()
        {
            return View("TaskDetails");
        }

        // Get Tasks Data
        public IActionResult getTasks(string searchstring)
        {
            var tasks = _taskManager.getTasks(searchstring);
            return PartialView("_TasksPartialView", tasks);
        }

        // Add Task
        public IActionResult AddTask(Task_DetailsVM model)
        {
            
                if (ModelState.IsValid)
                {
                    _taskManager.AddTask(model);
                    TempData["Message"] = "Task Added";
                    TempData["MessageType"] = "success";
                   
                }
                else
                {
                    TempData["Message"] = "Unable to add Task";
                    TempData["MessageType"] = "error";
                }
                return RedirectToAction("Index");
            
        }

        // Delete Task

        public IActionResult deleteTask(int TaskId)
        {
            try
            {
                _taskManager.DeleteTask(TaskId);
                TempData["Message"] = "Task Deleted";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Unable to delete Task";
                TempData["MessageType"] = "error";
                return RedirectToAction("Index");
            }
        }
        
        // edit modal data

        public IActionResult getEditModalData(int TaskId)
        {
            var data = _taskManager.getEditData(TaskId);
            return Json(new { data = data });
        }


        private void cache(Exception exception, object ex)
        {
            throw new NotImplementedException();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}