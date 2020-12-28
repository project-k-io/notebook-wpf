using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectK.Notebook.Models.Versions.Version2;
using ProjectK.Utils;
using TaskModel = ProjectK.Notebook.Models.TaskModel;

namespace ProjectK.Notebook.Data
{
    public static class ImportHelper
    {
        public static async Task<List<TaskModel>> ReadFromFileVersionTwo(string path)
        {
            // Load DataModel
            var dataModel = await FileHelper.ReadFromFileAsync<DataModel>(path);

            // Add Tasks
            var tasks = new List<TaskModel>();
            foreach (var task2 in dataModel.Tasks)
            {
                var task = new TaskModel(); // ReadFromFileVersionTwo
                task.Init(task2);
                tasks.Add(task);
            }

            return tasks;
        }
    }
}