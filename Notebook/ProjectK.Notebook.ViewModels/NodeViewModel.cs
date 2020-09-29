using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels
{
    public enum ModifiedStatus
    {
        None,
        Modified,
        ChildModified
    }

    public class NodeViewModel : ViewModelBase, ITreeNode<NodeViewModel>
    {
        #region Static Fields
        private static readonly ILogger Logger = LogManager.GetLogger<NodeViewModel>();
        #endregion

        #region Fields

        // Model Wrappers
        //private Guid _id;
        //private Guid _parentId;
        //private string _context;
        //private string _name;
        //private DateTime _created;
        //private string _description;
        protected dynamic Model;

        // Misc
        private string _kind;
        private bool _isExpanded;
        private bool _isSelected;
        private ModifiedStatus _modified;

        #endregion

        #region INode - Implementation

        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();

        #endregion

        #region Properties

        // Model 

        public NodeViewModel()
        {
            SetKind("Node");
            Model = new NodeModel();
        }
        public NodeViewModel(dynamic model): this()
        {
            SetKind("Node");
            Model = model;
        }

        public void SetKind(string kind)
        {
            _kind = kind;
        }

        public void ViewModelToModel(NodeModel model)
        {
            SetKind("Node");
            model.Id = Id;
            model.ParentId = ParentId;
            model.Name = Name;
            model.Created = Created;
            model.Context = Context;
        }


        // Model Wrapper
        public string Kind { get => _kind; set => Set(ref _kind, value); }
        public Guid Id { get => Model.Id; set => this.Set(Id, v => Model.Id = v, value); }
        public Guid ParentId { get => Model.ParentId; set => this.Set(ParentId, v => Model.ParentId = v, value); }
        public string Name { get => Model.Name; set => this.Set(Name, v => Model.Name = v, value); }
        public DateTime Created { get => Model.Created; set => this.Set(Created, v => Model.Created = v, value); }
        public string Context { get => Model.Context; set => this.Set(Context, v => Model.Context = v, value); }

        public NodeViewModel Parent { get; set; }
        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TitleList { get; set; }


        public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value); }
        public ModifiedStatus Modified { get => _modified; set => Set(ref _modified, value); }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }


        public NodeViewModel LastSubNode => Nodes.LastOrDefault();
        // ToDo: Improve allocation, maybe allocate only when you needed?

        #endregion

        #region Constructors

        #endregion

        #region Override functions

        public override string ToString()
        {
            return $"{Context}:{Name}";
        }

        public override void RaisePropertyChanged<T>(string propertyName = null, T oldValue = default(T), T newValue = default(T), bool broadcast = false)
        {
            base.RaisePropertyChanged(propertyName, oldValue, newValue, broadcast);
            if (!IsNodeModelProperty(propertyName)) return;

            Logger?.LogDebug($@"[Node] PropertyChanged: {propertyName} | {oldValue} | {newValue}");
            Modified = ModifiedStatus.Modified;
            SetParentChildModified();
            MessengerInstance.Send(new NotificationMessage<NodeModel>(Model, "Modified"));
        }

        #endregion

        private static bool IsNodeModelProperty(string n) => 
            n == "Id" || 
            n == "ParentId" || 
            n == "Name" || 
            n == "Created" || 
            n == "Context" || 
            n == "Description";

        #region Public functions

        public void SaveTo(NotebookModel model)
        {
            foreach (var node in Nodes)
            {
                node.SaveRecursively(model);
            }
        }


        private void SaveRecursively(NotebookModel notebook)
        {
            var model = this.Model;
            if(model is TaskModel)
                notebook.Tasks.Add(model);
            else if (model is NoteModel)
                notebook.Notes.Add(model);

            TrySetId();

            foreach (var node in Nodes)
            {
                node.SaveRecursively(notebook);
                node.ParentId = Id;
            }
        }


        public void TrySetId()
        {
            if (Id != Guid.Empty)
                return;

            Id = Guid.NewGuid();
        }


        public virtual NodeViewModel AddNew()
        {
            var subNode = new NodeViewModel
            {
                Kind = "Node",
                Name = "New Node", 
                Created = DateTime.Now
            };

            Add(subNode);
            FixContext(subNode);
            return subNode;
        }


        public void Add(NodeViewModel node)
        {
            node.SetParent(this);
            Nodes.Add(node);
        }



        public void Remove(NodeViewModel node)
        {
            node.ParentId = Guid.Empty;
            Nodes.Remove(node);
        }

        public void Insert(int index, NodeViewModel node)
        {
            node.SetParent(this);
            Nodes.Insert(index, node);
        }

        public void SetParent(NodeViewModel node)
        {
            Parent = node;
            ParentId = node.Id;
        }

        public void SetParents()
        {
            foreach (var node in Nodes)
            {
                node.SetParent(this);
                node.SetParents();
            }
        }

        public void ResetModified()
        {
            this.Execute(a =>
            {
                if (a.Modified == ModifiedStatus.Modified)
                    a.Modified = ModifiedStatus.None;
            });
        }
        public void SetParentChildModified()
        {
            this.UpAction(a =>
            {
                 a.Modified = ModifiedStatus.ChildModified;
            });
        }

        public void ResetParentChildModified()
        {
            this.Execute(a =>
            {
                if (a.Modified == ModifiedStatus.ChildModified)
                    a.Modified = ModifiedStatus.None;
            });
        }



        public void ExtractContext(ObservableCollection<string> contextList)
        {
            if (!string.IsNullOrEmpty(Context) && !contextList.Contains(Context))
                contextList.Add(Context);

            foreach (var node in Nodes)
                node.ExtractContext(contextList);
        }

        private void FixContext(string context, string child, NodeViewModel node)
        {
            if (Context != context)
                return;

            node.Context = child;
        }

        protected void FixContext(NodeViewModel node)
        {
            FixContext("Time Tracker", "Year", node);
            FixContext("Year", "Month", node);
            FixContext("Month", "Week", node);
            FixContext("Week", "Day", node);
            FixContext("Day", "Task", node);
            FixContext("Task", "Task", node);
        }

        public void FixContext()
        {
            foreach (var node in Nodes)
            {
                FixContext(node);
                node.FixContext();
            }
        }



        public NodeViewModel FindNode(Guid id)
        {
            if (Id == id)
                return this;

            foreach (var node in Nodes)
            {
                var findNode = node.FindNode(id);
                if (findNode != null)
                    return findNode;
            }

            return null;
        }


        #endregion

    }
}