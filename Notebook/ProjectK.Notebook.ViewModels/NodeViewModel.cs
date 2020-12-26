using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.Models.Extensions;
using ProjectK.Notebook.Models.Interfaces;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Notebook.ViewModels.Helpers;
using ProjectK.Notebook.ViewModels.Interfaces;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels
{
    public class NodeViewModel : ItemViewModel, ITreeNode<NodeViewModel>
    {
        #region Static Fields

        private static readonly ILogger Logger = LogManager.GetLogger<NodeViewModel>();

        #endregion

        #region INode - Implementation

        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();

        #endregion



        public void DeleteNode(IActionService service)
        {
            Logger.LogDebug($"Delete Node : {Name}");
            var item = this;
            if (service.DeleteMessageBox())
                return;

            var parent = item.Parent;
            if (parent == null)
                return;

            var num1 = parent.Nodes.IndexOf(item);
            service.Dispatcher(() => parent.Remove(item));

            var parentNode = num1 > 0 ? parent.Nodes[num1 - 1] : parent;
            if (parentNode == null)
                return;

            service.SelectItem(parentNode);
            service.Handled();
        }

        private void FixTitles()
        {
            foreach (var node in Nodes)
            {
                var title = this.GetSubNodeTitle(node.Model);
                if (!string.IsNullOrEmpty(title))
                    node.Name = title;

                node.FixTitles();
            }
        }

        public NodeViewModel CreateNode(INode model)
        {
            var node = new NodeViewModel(model);
            Add(node);
            return node;
        }


        #region Fields

        // Model Wrappers
        //private Guid _id;
        //private Guid _parentId;
        //private string _context;
        //private string _name;
        //private DateTime _created;
        //private string _description;

        // Misc
        private bool _isExpanded;
        private bool _isSelected;
        private ModifiedStatus _modified;

        #endregion

        #region Properties

        // Model 

        public NodeViewModel()
        {
            Kind = "Node";
        }

        public NodeViewModel(INode model) : this()
        {
            Model = model;
        }


        public void ViewModelToModel(INode model)
        {
            model.Id = Id;
            model.ParentId = ParentId;
            model.Name = Name;
            model.Created = Created;
            model.Context = Context;
        }


        // Model Wrapper


        public NodeViewModel Parent { get; set; }
        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TitleList { get; set; }


        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public ModifiedStatus Modified
        {
            get => _modified;
            set => Set(ref _modified, value);
        }

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

        public override void RaisePropertyChanged<T>(string propertyName = null, T oldValue = default,
            T newValue = default, bool broadcast = false)
        {
            base.RaisePropertyChanged(propertyName, oldValue, newValue, broadcast);
            if (!ModelRules.IsNodeModelProperty(propertyName)) return;

            Logger?.LogDebug($@"[Node] PropertyChanged: {propertyName} | {oldValue} | {newValue}");
            Modified = ModifiedStatus.Modified;
            SetParentChildModified();
            MessengerInstance.Send(new NotificationMessage<INode>(Model, "Modified"));
        }

        #endregion

        #region Public functions

        public void SaveTo(List<INode> list)
        {
            foreach (var node in Nodes) node.SaveRecursively(list);
        }
        private void SaveRecursively(List<INode> list)
        {
            list.Add(Model);

            TrySetId();

            foreach (var node in Nodes)
            {
                node.SaveRecursively(list);
                node.ParentId = Id;
            }
        }
        public void TrySetId()
        {
            if (Id != Guid.Empty)
                return;

            Id = Guid.NewGuid();
        }
        public void Add(NodeViewModel node)
        {
            node.SetParent(this);
            Nodes.Add(node);
        }
        public void Remove(NodeViewModel node)
        {
            Nodes.Remove(node);
        }
        public void Insert(int index, NodeViewModel node)
        {
            node.SetParent(this);
            Nodes.Insert(index, node);
        }
        public void SetParent(NodeViewModel parent)
        {
            Parent = parent;
            ParentId = parent.Id;
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
            this.UpAction(a => { a.Modified = ModifiedStatus.ChildModified; });
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
        public void FixContext(NodeViewModel node)
        {
            if (ModelRules.GetSubNodeContext(Context, out var context))
                node.Context = context;
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
        public List<INode> GetModels()
        {
            var models = Nodes.GetModels();
            models.Add(Model);
            return models;
        }

        #endregion

        private void FixTypes()
        {
            Model.FixTypes(Name);
            foreach (var node in Nodes)
                node.FixTypes();
        }

        public void LoadFrom(Models.Versions.Version1.TaskModel model)
        {
            IsSelected = model.IsSelected;
            IsExpanded = model.IsExpanded;
            Description = model.Description;
            if (Model is TaskModel task)
            {
                task.Type = model.Type;
                task.DateStarted = model.DateStarted;
                task.DateEnded = model.DateEnded;
            }

            Name = model.Title;

            if (model.SubTasks.IsNullOrEmpty())
                return;

            foreach (var subTask in model.SubTasks)
            {
                var node = new NodeViewModel();
                node.Model = new TaskModel();
                node.LoadFrom(subTask);
                Nodes.Add(node);
            }
        }

        public void FixTime()
        {
#if AK
            if (Model is TaskModel task)
            {
                if (ModelRules.IsPersonalType(task.Type))
                    return;
            }

            if (Nodes.IsNullOrEmpty())
            {
                Total = Duration;
            }
            else
            {
                for (var index = 0; index < Nodes.Count; ++index)
                {
                    var subTask = (TaskViewModel)Nodes[index];
                    if (subTask.DateEnded == DateTime.MinValue && index < Nodes.Count - 1)
                        subTask.DateEnded = ((TaskViewModel)Nodes[index + 1]).DateStarted;
                }

                Total = TimeSpan.Zero;
                for (var index = 0; index < Nodes.Count; ++index)
                {
                    var subTask = (TaskViewModel)Nodes[index];
                    subTask.FixTime();
                    Total += subTask.Total;
                }

                var subTask1 = (TaskViewModel)Nodes[^1];
                if (subTask1.DateEnded != DateTime.MinValue)
                    DateEnded = subTask1.DateEnded;
                var subTask2 = (TaskViewModel)Nodes[0];
                if (subTask2.DateStarted != DateTime.MinValue)
                    DateStarted = subTask2.DateStarted;
            }
#endif
        }



    }
}