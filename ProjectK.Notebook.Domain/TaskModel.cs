﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using ProjectK.Notebook.Domain.Extensions;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class TaskModel : ITask
    {
        // Primary Key
        [Key] 
        public Guid Id { get; set; }
        // INode
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public string Context { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }

        // ITask
        public string Type { get; set; }
        public string SubType { get; set; }
        public int Rating { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateEnded { get; set; }

        [XmlIgnore]
        public TimeSpan Duration
        {
            get
            {
                if (DateStarted == DateTime.MinValue || DateEnded == DateTime.MinValue)
                    return TimeSpan.Zero;

                return DateEnded - DateStarted;
            }
        }


        // Foreign Key
        [ForeignKey("NotebookModel")]
        public int NotebookId { get; set; }
        public virtual NotebookModel NotebookModel { get; set; }

        public bool IsSame(TaskModel b)
        {
            return ((ITask)this).IsSame(b);
        }

        public void Init(Versions.Version2.TaskModel task2)
        {
            Id = task2.Id;
            ParentId = task2.ParentId;
            Context = task2.Context;
            Name = task2.Title;
            Description = task2.Description;
            Rating = task2.Rating;
            DateStarted = task2.DateStarted;
            DateEnded = task2.DateEnded;
            Type = task2.Type;
            SubType = task2.SubType;
        }

        public static TaskModel NewTask()
        {
            var task = new TaskModel
            {
                Id = Guid.NewGuid(),
                DateStarted = DateTime.Now
            };
            return task;
        }

#if AK

        public override string ToString()
        {
            return $"{base.ToString()}:{Type}:{SubType}: {Rating}: {DateStarted}:{DateEnded}";
        }





        public TaskModel()
        {
        }
        public TaskModel(TaskModel b) : base(b)
        {
            Rating = b.Rating;
            DateStarted = b.DateStarted;
            DateEnded = b.DateEnded;
            SubType = b.SubType;
            Type = b.Type;
            Description = b.Description;
         }

        public new TaskModel Copy()
        {
            return new TaskModel(this);
        }

        Guid INode.Id
        {
            get => _id;
            set => _id = value;
        }
#endif
    }
}