using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Notebook.ViewModels.Extensions;
using ProjectK.Notebook.ViewModels.Interfaces;
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

        public void SaveTo(List<dynamic> list)
        {
            foreach (var node in Nodes)
            {
                node.SaveRecursively(list);
            }
        }

        private void SaveRecursively(List<dynamic> list)
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

        public void KeyboardAction( KeyboardKeys keyboardKeys, IActionService service)
        {
            var item = this;
            var state = service.GetState();

            // don't show logging ctl, alt, shift or arrow keys
            if (keyboardKeys != KeyboardKeys.None && state != KeyboardStates.None)
                Logger.LogDebug($"KeyboardAction: {keyboardKeys}, {state}");

            switch (keyboardKeys)
            {
                case KeyboardKeys.Insert:
                    NodeViewModel node;
                    switch (state)
                    {
                        case KeyboardStates.IsShiftPressed:
                            node = item.Parent.AddNew();
                            node.Created = DateTime.Now;
                            break;
                        case KeyboardStates.IsControlPressed:
                            var lastSubNode = item.Parent.LastSubNode;
                            node = item.Parent.AddNew();

                            if (lastSubNode != null)
                            {
                                node.Name = item.Name;
                                node.Created = DateTime.Now;
                            }

                            break;
                        default:
                            node = item.AddNew();
                            break;
                    }

                    item.IsSelected = true;
                    service.SelectItem(item);
                    service.ExpandItem(item);
                    service.Handled();
                    Logger.LogDebug($"Added [{node.Name}] to [{item.Name}]");
                    break;
                case KeyboardKeys.Delete:
                    DeleteNode(service);
                    break;

                case KeyboardKeys.Left:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = item.Parent;
                        if (parent1 == null)
                            break;
                        var parent2 = parent1.Parent;
                        if (parent2 == null)
                            break;

                        parent1.Remove(item);

                        var num2 = parent2.Nodes.IndexOf(parent1);
                        parent2.Insert(num2 + 1, item);
                        service.SelectItem(item);
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Right:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = item.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(item);
                        if (num2 <= 0)
                            break;

                        var parentNode2 = parent1.Nodes[num2 - 1];
                        if (parentNode2 == null)
                            break;

                        parent1.Remove(item);
                        parentNode2.Add(item);
                        service.SelectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Up:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = item.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(item);
                        if (num2 <= 0)
                            break;
                        parent1.Remove(item);
                        parent1.Insert(num2 - 1, item);
                        service.SelectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        service.Handled();
                    }

                    break;
                case KeyboardKeys.Down:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = item.Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(item);
                        if (num2 >= parent1.Nodes.Count - 1)
                            break;
                        parent1.Remove(item);
                        parent1.Insert(num2 + 1, item);
                        service.SelectItem(item);
                        parent1.IsExpanded = true;
                        item.IsSelected = true;
                        service.Handled();
                    }

                    break;
            }
        }

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

            MessengerInstance.Send(new NotificationMessage<NodeViewModel>(item, "Delete"));
        }

    }
}