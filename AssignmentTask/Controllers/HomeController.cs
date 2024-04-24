using AssignmentTask.Models;
using BLL_TaskManager.Interfaces;
using DAL_TaskManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Printing;

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
        public IActionResult getTasks(string searchstring, int pageSize, int currentPage)
        {
            var tasks = _taskManager.getTasks(searchstring);
            int totalPages = (int)Math.Ceiling((double)tasks.Count() / pageSize);
            var paginatedData = tasks.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            if (paginatedData.Count != 0)
            {
                ViewBag.CurrentPage = currentPage;
                ViewBag.IsEmpty = false;
                ViewBag.TotalPages = totalPages;
                ViewBag.Count = paginatedData.Count;
                ViewBag.TotalCount = tasks.Count;
            }
            else
            {
                ViewBag.IsEmpty = true;
            }
            return PartialView("_TasksPartialView", paginatedData);
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
        
        // edit 
        public IActionResult EditTask(IFormCollection formcollection)
        {
            string TaskId = formcollection["TaskId"];
            string TaskName = formcollection["TaskName"];
            string Assignee = formcollection["Assignee"];
            string Discription = formcollection["Discription"];
            string DueDate = formcollection["DueDate"];
            string City = formcollection["City"];
            string Category = formcollection["Catogery"];

            try
            {
                _taskManager.ediTask(TaskId ,TaskName, Assignee, Discription, DueDate, City, Category);
                TempData["Message"] = "Task Edited Successfully";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
            catch(Exception ex) 
            {
                TempData["Message"] = "something went wrong";
                TempData["MessageType"] = "error";
                return RedirectToAction("Index");
            }

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