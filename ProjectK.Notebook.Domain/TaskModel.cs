using System;
using System.Text.Json.Serialization;

namespace ProjectK.Notebook.Domain
{
    public class TaskModel : NodeModel<TaskModelBase>
    {

        public override string ToString()
        {
            return $"{Data.Context}:{Data.Type}:{Name}:{Data.DateStarted}:{Data.DateEnded}";
        }

        public static TaskModel NewTask()
        {
            var task = new TaskModel
            {
                Id = Guid.NewGuid(), 
                Data =
                {
                    DateStarted = DateTime.Now
                }
            };
            return task;
        }
    }
}