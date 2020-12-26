using System;
using GalaSoft.MvvmLight;
using ProjectK.Notebook.Models.Interfaces;

namespace ProjectK.Notebook.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        private string _kind;
        public string Kind
        {
            get => _kind;
            set => Set(ref _kind, value);
        }
        public INode Model { get; set; }
        public string Description
        {
            get => Model.Description;
            set => this.Set(Description, v => Model.Description = v, value);
        }

        public Guid Id
        {
            get => Model.Id;
            set => this.Set(Id, v => Model.Id = v, value);
        }

        public Guid ParentId
        {
            get => Model.ParentId;
            set => this.Set(ParentId, v => Model.ParentId = v, value);
        }

        public string Name
        {
            get => Model.Name;
            set => this.Set(Name, v => Model.Name = v, value);
        }

        public DateTime Created
        {
            get => Model.Created;
            set => this.Set(Created, v => Model.Created = v, value);
        }

        public string Context
        {
            get => Model.Context;
            set => this.Set(Context, v => Model.Context = v, value);
        }

    }
}