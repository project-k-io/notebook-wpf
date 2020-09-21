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
        private readonly ILogger _logger = LogManager.GetLogger<NodeViewModel>();
        #endregion

        #region Fields

        private bool _isExpanded;
        private bool _isSelected;

        private Guid _id;
        private string _context;
        private string _title;
        private DateTime _created;
        private Guid _parentId;

        #endregion

        #region INode - Implementation

        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();

        #endregion

        #region Properties

         // Model 
        public Guid Id { get => _id; set => Set(ref _id, value); }
        public string Context { get => _context; set => Set(ref _context, value); }
        public string Title { get => _title; set => Set(ref _title, value); }
        public DateTime Created { get => _created; set => Set(ref _created, value); }
        public Guid ParentId { get => _parentId; set => Set(ref _parentId, value); }

        public NodeViewModel Parent { get; set; }
        public ObservableCollection<string> TypeList { get; set; }
        public ObservableCollection<string> ContextList { get; set; }
        public ObservableCollection<string> TitleList { get; set; }


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


        public NodeViewModel LastSubNode => Nodes.LastOrDefault();
        // ToDo: Improve allocation, maybe allocate only when you needed?

        #endregion

        #region Constructors

        public NodeViewModel()
        {
            Parent = null;
        }

        public NodeViewModel(string title)
        {
            Title = title;
        }


        #endregion

        #region Override functions

        public override string ToString()
        {
            return $"{Context}:{Title}";
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
            var subNode = new NodeViewModel { Title = "New Node", Created = DateTime.Now };

            Add(subNode);
            FixContext(subNode);
            return subNode;
        }


        public void Add(NodeViewModel node)
        {
            if (node.Title == "Time Tracker2")
                _logger.LogDebug(node.Title);

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

        public void Init(NodeModel b)
        {
            Id = b.NodeId;
            Title = b.Name;
            Created = b.Created;
            Context = b.Context;
        }


        #endregion

    }
}