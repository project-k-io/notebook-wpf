using System;
using System.Collections.ObjectModel;
using ProjectK.Notebook.Domain;
using ProjectK.Utils.Extensions;

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
            ContextList.AddRange(ModesRulesHelper.GlobalContextList);
        }
    }
}