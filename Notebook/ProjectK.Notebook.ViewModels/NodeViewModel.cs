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

        #endregion

        #region INode - Implementation

        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();

        #endregion

        #region Properties

        // Model 
        public dynamic Model { get; set; }

        // Model Wrapper
        //public Guid Id { get => Model.NodeId; set => this.Set(Model.NodeId, v => Model.NodeId = v, value); }
        //public string Context { get => Model.Context; set => this.Set(Model.Context, v => Model.Context = v, value); }
        //// public string Title { get => Model.Name; set => this.Set(Model.Name, v => Model.Name = v, value); }
        //public DateTime Created { get => Model.Created; set => this.Set(Model.Created, v => Model.Created = v, value); }
        //public Guid ParentId { get => Model.ParentId; set => this.Set(Model.ParentId, v => Model.ParentId = v, value); }

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
            Model = new NodeModel();
        }

        public NodeViewModel(NodeModel model)
        {
            Model = model;
        }
        

        #endregion

        #region Override functions

        public override string ToString()
        {
            return $"{Model.Context}:{Model.Name}";
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
                node.Model.ParentId = Model.NodeId;
            }
        }

        private NodeModel GetModel()
        {
            var model = new NodeModel();
            return model;
        }

        public void TrySetId()
        {
            if (Model.NodeId != Guid.Empty)
                return;

            Model.NodeId = Guid.NewGuid();
        }


        public virtual NodeViewModel AddNew()
        {
            var subNode = new NodeViewModel { Model = { Name = "New Node",  Created = DateTime.Now} };

            Add(subNode);
            FixContext(subNode);
            return subNode;
        }


        public void Add(NodeViewModel node)
        {
            if (node.Model.Name == "Time Tracker2")
                _logger.LogDebug((string)node.Model.Name);

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
            if (!string.IsNullOrEmpty(Model.Context) && !contextList.Contains(Model.Context))
                contextList.Add(Model.Context);

            foreach (var node in Nodes)
                node.ExtractContext(contextList);
        }

        private void FixContext(string parent, string child, NodeViewModel node)
        {
            if (Model.Context != parent)
                return;

            node.Model.Context = child;
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
            if (Model.NodeId == id)
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