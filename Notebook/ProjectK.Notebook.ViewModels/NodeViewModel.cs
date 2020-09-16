﻿using System;
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

        #region Model Wrappers

        public IItem Model { get; set; }

        public Guid Id => Model.Id;

        public string Title
        {
            get => Model.Name;
            set => this.Set(Model.Name, v => { Model.Name = v; }, value);
        }

        public DateTime Created
        {
            get => Model.Created;
            set => this.Set(Model.Created, v => { Model.Created = v; }, value);
        }

        public Guid ParentId
        {
            get => Model.ParentId;
            set => Model.ParentId = value;
        }


        public string Context
        {
            get => Model.Context;
            set => this.Set(Context, v => Model.Context = v, value);
        }



        #endregion

        #region Properties

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
        public virtual void CreateModel()
        {
            Model = new TaskModel();
        }

        public NodeViewModel()
        {
            Model = new NodeModel();
            Parent = null;
        }

        public NodeViewModel(string title) : this()
        {
            Title = title;
        }

        public NodeViewModel(IItem model)
        {
            Model = model;
            Parent = null;
        }

        public NodeViewModel(IItem model, string title) : this(model)
        {
            Title = title;
        }




        #endregion

        #region Override functions

        public override string ToString()
        {
            return Model.ToString();
        }

        #endregion

        #region Public functions

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


        public void TrySetId()
        {
            if (Model.Id != Guid.Empty)
                return;

            Model.Id = Guid.NewGuid();
        }


        public virtual NodeViewModel AddNew()
        {
            var subNode = new NodeViewModel{ Title = "New Node", Created = DateTime.Now };

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




        #endregion

    }
}