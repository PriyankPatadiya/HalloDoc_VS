using DAL_TaskManager.DataModels;
using DAL_TaskManager.ViewModels;
using Microsoft.VisualBasic;

namespace BLL_TaskManager.Interfaces
{
    public interface ITaskManager
    {
        public List<Task_DetailsVM> getTasks(string search);
        public void AddTask(Task_DetailsVM model);
        public void DeleteTask(int TaskId);

        public Task_DetailsVM getEditData(int TaskId);

        public void ediTask( string TaskId,string TaskName, string Assignee, string Discription, string DueDate, string City, string Category);
    }
}
