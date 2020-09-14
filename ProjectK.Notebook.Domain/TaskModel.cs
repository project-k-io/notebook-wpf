﻿using System;
using System.Text.Json.Serialization;

namespace ProjectK.Notebook.Domain
{

    public class TaskModel
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonPropertyName("Title")]
        public string Name { get; set; }
        public int Level = 0;
        [JsonPropertyName("Id")]
        public Guid TaskId { get; set; }
        public Guid ParentId { get; set; }
        public int Rating { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Description { get; set; }
        public string Context { get; set; }

        public TaskModel Copy()
        {
            return new TaskModel
            {
                TaskId = TaskId,
                ParentId = ParentId,
                Rating = Rating,
                DateStarted = DateStarted,
                DateEnded = DateEnded,
                Type = Type,
                SubType = SubType,
                Name = Name,
                Description = Description,
                Context = Context
            };
        }


        public static TaskModel NetTask()
        {
            return new TaskModel {TaskId = Guid.NewGuid(), DateStarted = DateTime.Now};
        }

        public override string ToString()
        {
            return $"{Context}:{Type}:{Name}:{DateStarted}:{DateEnded}";
        }

        public bool IsSame(TaskModel m)
        {
            if (TaskId != m.TaskId) return false;
            if (ParentId != m.ParentId) return false;
            if (Rating != m.Rating) return false;
            if (DateStarted != m.DateStarted) return false;
            if (DateEnded != m.DateEnded) return false;
            if (Type != m.Type) return false;
            if (SubType != m.SubType) return false;
            if (Name != m.Name) return false;
            if (Description != m.Description) return false;
            if (Context != m.Context) return false;
            return true;
        }
    }
}