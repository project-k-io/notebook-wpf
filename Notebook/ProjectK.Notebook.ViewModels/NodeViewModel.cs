using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Notebook.Domain.Interfaces;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Utils;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels
{
    public class NodeViewModel : ViewModelBase, INode<NodeViewModel>
    {
        private readonly ILogger _logger = LogManager.GetLogger<NodeViewModel>();

        #region Fields

        private bool _isExpanded;
        private bool _isSelected;
        private TimeSpan _total;

        #endregion

        #region Properties - Model Wrappers

        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();

        public IItem Model { get; set; }

        public Guid Id => Model.Id;
        public string Title
        {
            get => Model.Name;
            set => this.Set(Model.Name, v =>
            {
                Model.Name = v;
            }, value);
        }
        public DateTime Created
        {
            get => Model.Created;
            set => this.Set(Model.Created, v =>
            {
                Model.Created = v;
            }, value);
        }

        public Guid ParentId
        {
            get => Model.ParentId;
            set => Model.ParentId = value;
        }

        public string Description
        {
            get => Model.Description;
            set => this.Set(Description, v => Model.Description = v, value);
        }


        #endregion

        #region Properties

        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TitleList { get; set; }

        public string Context
        {
            get => Model.Context;
            set => this.Set(Context, v => Model.Context = v, value);
        }

        public NodeViewModel Parent { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value);
        }

        public TimeSpan Total
        {
            get => _total;
            set => Set(ref _total, value);
        }
        public string Type
        {
            get => Model.Type;
            set => this.Set(Type, v => Model.Type = v, value);
        }

        public NodeViewModel LastSubNode => Nodes.LastOrDefault();
        // ToDo: Improve allocation, maybe allocate only when you needed?

        #endregion

        #region Override functions

        public override string ToString()
        {
            return Model.ToString();
        }

        #endregion

        public void SaveTo(List<IItem> list)
        {
            foreach (var node in Nodes)
            {
                node.SaveRecursively(list);
            }
        }

        private void SaveRecursively(ICollection<IItem> list)
        {
            list.Add(Model);
            TrySetId();

            foreach (var node in Nodes)
            {
                node.SaveRecursively(list);
                node.ParentId = Model.Id;
            }
        }




        #region Constructors
        public NodeViewModel()
        {
            Parent = null;
            Model = new NodeModel();
        }

        public NodeViewModel(IItem model)
        {
            Parent = null;
            Model = model;
        }

        public NodeViewModel(IItem model, string title)
        {
            Model = model;
            Parent = null;
            Title = title;
        }


#endregion

#region Public functions

        public void TrySetId()
        {
            if (Model.Id != Guid.Empty)
                return;

            Model.Id = Guid.NewGuid();
        }


        protected virtual NodeViewModel AddNew()
        {
            IItem model = CreateModel();
            var subNode = new NodeViewModel(model) {Title = "New Node", Created = DateTime.Now};
            Add(subNode);
            FixContext(subNode);
            return subNode;
        }

        private IItem CreateModel()
        {
            return new NodeModel();
        }

        public void Add(NodeViewModel node)
        {
            if (node.Title == "Time Tracker2")
                _logger.LogDebug(node.Title);

            node.Parent = this;
            Nodes.Add(node);
        }

        private void Insert(int index, NodeViewModel node)
        {
            node.Parent = this;
            Nodes.Insert(index, node);
        }

        public void SetParents()
        {
            foreach (var node in Nodes)
            {
                node.Parent = this;
                node.SetParents();
            }
        }


        public void ExtractContext(ObservableCollection<string> contextList)
        {
            if (!string.IsNullOrEmpty(Context) && !contextList.Contains(Context))
                contextList.Add(Context);
            foreach (var node in Nodes)
                node.ExtractContext(contextList);
        }

        private void FixContext(string parent, string child, NodeViewModel node)
        {
            if (!(Context == parent))

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


        public void KeyboardAction(
            KeyboardKeys keyboardKeys,
            Func<KeyboardStates> getState,
            Action handled,
            Action<NodeViewModel> selectItem, Action<NodeViewModel> expandItem,
            Func<bool> deleteMessageBox,
            Action<Action> dispatcher)
        {
            var state = getState();
            
            // don't show logging ctl, alt, shift or arrow keys
            if(keyboardKeys != KeyboardKeys.None && state != KeyboardStates.None)
                _logger.LogDebug($"KeyboardAction: {keyboardKeys}, {state}");

            switch (keyboardKeys)
            {
                case KeyboardKeys.Insert:
                    NodeViewModel node;
                    switch (state)
                    {
                        case KeyboardStates.IsShiftPressed:
                            node = Parent.AddNew();
                            node.Created = DateTime.Now;
                            break;
                        case KeyboardStates.IsControlPressed:
                            var lastSubNode = Parent.LastSubNode;
                            node = Parent.AddNew();
                            if (lastSubNode != null)
                            {
                                node.Type = Type;
                                node.Title = Title;
                                node.Created = DateTime.Now;
                            }

                            break;
                        default:
                            node = AddNew();
                            break;
                    }

                    IsSelected = true;
                    selectItem(this);
                    expandItem(this);
                    handled();
                    _logger.LogDebug($"Added [{node.Title}] to [{Title}]");
                    break;
                case KeyboardKeys.Delete:
                    if (deleteMessageBox())
                        break;
                    var parent = Parent;
                    if (parent == null)
                        break;
                    
                    var num1 = parent.Nodes.IndexOf(this);
                    dispatcher(() => parent.Nodes.Remove(this));
                    
                    var parentNode = num1 > 0 ? parent.Nodes[num1 - 1] : parent;
                    if (parentNode == null)
                        break;

                    selectItem(parentNode);
                    handled();
                    break;

                case KeyboardKeys.Left:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = Parent;
                        if (parent1 == null)
                            break;
                        var parent2 = parent1.Parent;
                        if (parent2 == null)
                            break;
                        parent1.Nodes.Remove(this);
                        var num2 = parent2.Nodes.IndexOf(parent1);
                        parent2.Insert(num2 + 1, this);
                        selectItem(this);
                        handled();
                    }

                    break;
                case KeyboardKeys.Right:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(this);
                        if (num2 <= 0)
                            break;

                        var parentNode2 = parent1.Nodes[num2 - 1];
                        if (parentNode2 == null)
                            break;

                        parent1.Nodes.Remove(this);
                        parentNode2.Add(this);
                        selectItem(this);
                        parent1.IsExpanded = true;
                        IsSelected = true;
                        handled();
                    }

                    break;
                case KeyboardKeys.Up:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(this);
                        if (num2 <= 0)
                            break;
                        parent1.Nodes.Remove(this);
                        parent1.Insert(num2 - 1, this);
                        selectItem(this);
                        parent1.IsExpanded = true;
                        IsSelected = true;
                        handled();
                    }

                    break;
                case KeyboardKeys.Down:
                    if (state == KeyboardStates.IsCtrlShiftPressed)
                    {
                        var parent1 = Parent;
                        if (parent1 == null)
                            break;
                        var num2 = parent1.Nodes.IndexOf(this);
                        if (num2 >= parent1.Nodes.Count - 1)
                            break;
                        parent1.Nodes.Remove(this);
                        parent1.Insert(num2 + 1, this);
                        selectItem(this);
                        parent1.IsExpanded = true;
                        IsSelected = true;
                        handled();
                    }

                    break;
            }
        }

#endregion
    }
}