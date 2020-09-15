﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProjectK.Notebook.Domain.Versions.Version1
{
    [XmlRoot("TaskViewModel")]
    public class TaskModel
    {
        public int Rating { get; set; }
        [XmlArrayItem("TaskViewModel")] public List<TaskModel> SubTasks { get; set; }

        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }
        public DateTime TimeStarted { get; set; }
        public DateTime TimeEnded { get; set; }
        public string Title { get; set; }
    }
}