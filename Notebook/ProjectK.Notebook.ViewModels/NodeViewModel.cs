using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using ProjectK.Logging;
using ProjectK.Notebook.Domain;
using ProjectK.Utils;

namespace ProjectK.Notebook.ViewModels
{
    public class NodeViewModel : ViewModelBase, ITreeNode<NodeViewModel>
    {
        #region Static Fields
        private static readonly ILogger Logger = LogManager.GetLogger<NodeViewModel>();
        #endregion

        #region Fields

        // Model Wrappers
        private Guid _id;
        private Guid _parentId;
        private string _context;
        private string _name;
        private DateTime _created;
        private string _description;

        // Misc
        private string _kind;
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isModified;

        #endregion

        #region INode - Implementation

        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();

        #endregion

        #region Properties

        // Model 

        public NodeViewModel()
        {
            SetKind("Node");
        }
        public NodeViewModel(NodeModel model): this()
        {
            SetKind("Node");
            _id = model.NodeId;
            _parentId = model.ParentId;
            _name = model.Name;
            _created = model.Created;
            _context = model.Context;
        }

        public void SetKind(string kind)
        {
            _kind = kind;
        }

        public void ViewModelToModel(NodeModel model)
        {
            model.NodeId = Id;
            model.ParentId = ParentId;
            model.Name = Name;
            model.Created = Created;
            model.Context = Context;
        }


        // Model Wrapper
        public string Kind { get => _kind; set => Set(ref _kind, value); }
        public Guid Id { get => _id; set => Set(ref _id, value); }
        public Guid ParentId { get => _parentId; set => Set(ref _parentId, value); }
        public string Name { get => _name; set => Set(ref _name, value); }
        public DateTime Created { get => _created; set => Set(ref _created, value); }
        public string Context { get => _context; set => Set(ref _context, value); }
        public string Description { get => _description; set => Set(ref _description, value); }

        public NodeViewModel Parent { get; set; }
        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TitleList { get; set; }


        public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value); }
        public bool IsModified { get => _isModified; set => Set(ref _isModified, value); }

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
            if (propertyName == "IsSelected")
                return;

            Logger?.LogDebug($@"[Node] PropertyChanged: {propertyName} | {oldValue} | {newValue}");
            IsModified = true;
        }

        #endregion

        #region Public functions

        public void SaveTo(List<NodeModel> list)
        {
            foreach (var node in Nodes)
            {
                node.SaveRecursively(list);
            }
        }


        private void SaveRecursively(ICollection<NodeModel> list)
        {
            list.Add(GetModel());
            TrySetId();

            foreach (var node in Nodes)
            {
                node.SaveRecursively(list);
                node.ParentId = Id;
            }
        }

        private NodeModel GetModel()
        {
            var model = new NodeModel();
            return model;
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
            if (node.Name == "Time Tracker2")
                Logger?.LogDebug((string)node.Name);

            node.Parent = this;
            Nodes.Add(node);
        }

        public void Insert(int index, NodeViewModel node)
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