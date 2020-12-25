﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectK.Notebook.Domain.Interfaces;

namespace ProjectK.Notebook.Domain
{
    public class NoteModel : INote
    {
        // Foreign Key
        [ForeignKey("NotebookModel")] public Guid NotebookId { get; set; }
        public virtual NotebookModel NotebookModel { get; set; }
        [Key] public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public string Context { get; set; }
        public DateTime Created { get; set; }

        public string Description { get; set; }

        // INote
        public string Text { get; set; }
    }
}