using DAL_TaskManager.ViewModels;

namespace BLL_TaskManager.Interfaces
{
    public interface ITaskManager
    {
        public List<Task_DetailsVM> getTasks(string search);
        public void AddTask(Task_DetailsVM model);
        public void DeleteTask(int TaskId);

        public Task_DetailsVM getEditData(int TaskId);

    }
}
