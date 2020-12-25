using ProjectK.Notebook.Domain;
using ProjectK.Notebook.ViewModels.Helpers;
using ProjectK.Utils.Extensions;
using System;
using System.Collections.ObjectModel;

namespace ProjectK.Notebook.ViewModels.Design
{
    public class DesignNodeViewModel : NodeViewModel
    {
        public DesignNodeViewModel()
        {
            var model = new NodeModel
            {
                Name = "May",
                Context = "Month",
                Created = DateTime.Now
            };

            Model = model;
            ContextList = new ObservableCollection<string>();
            ContextList.AddRange(RulesHelper.GlobalContextList);
        }
    }
}