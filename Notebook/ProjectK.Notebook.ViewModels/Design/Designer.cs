using System;
using System.Collections.ObjectModel;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.ViewModels.Enums;
using ProjectK.Utils.Extensions;

namespace ProjectK.Notebook.ViewModels.Design
{
    public class Designer
    {
        public static MainViewModel Main
        {
            get
            {
                var model = new MainViewModel();
                var notebook = new NodeViewModel
                {
                    Model = new NotebookModel()
                    {
                        Name = "Time Tracker",
                        Context = "App"
                    },
                    IsExpanded = true
                };


                // 2018
                var year2018 = new NodeViewModel {Model = new NodeModel {Name = "2018", Context = "Year"}};
                var monthOct2018 = new NodeViewModel {Model = new NodeModel {Name = "October", Context = "Month"}};
                year2018.Add(monthOct2018);

                // 2019
                var year2019 = new NodeViewModel {Model = new NodeModel {Name = "2019", Context = "Year"}};
                var monthJan2019 = new NodeViewModel {Model = new NodeModel {Name = "January", Context = "Month"}};
                year2019.Add(monthJan2019);

                // 2020
                var year2020 = new NodeViewModel
                    {Model = new NodeModel {Name = "2020", Context = "Year"}, IsExpanded = true};
                var monthMay2020 = new NodeViewModel
                    {Model = new NodeModel {Name = "May", Context = "Month"}, IsExpanded = true};
                year2020.Add(monthMay2020);

                // May 2020
                var week1 = new NodeViewModel
                    {Model = new NodeModel {Name = "Week1", Context = "Week"}, IsExpanded = true};
                var thursday = new NodeViewModel
                    {Model = new NodeModel {Name = "Thursday", Context = "Day"}, IsExpanded = true};


                var node1 = new NodeViewModel
                {
                    Model = new TaskModel // Design
                    {
                        Name = "Dinner",
                        Context = "TaskModel",
                        DateStarted = new DateTime(2020, 5, 14, 20, 34, 0),
                        DateEnded = new DateTime(2020, 5, 14, 21, 40, 0)
                    },
                    IsExpanded = true
                };

                var node2 = new NodeViewModel
                {
                    Model = new TaskModel // Design
                    {
                        Name = "Movie",
                        Context = "TaskModel",
                        DateStarted = new DateTime(2020, 5, 14, 21, 50, 0),
                        DateEnded = new DateTime(2020, 5, 14, 23, 20, 0)
                    },
                    Modified = ModifiedStatus.Modified,
                    IsExpanded = true
                };
                thursday.Add(node1);
                thursday.Add(node2);
                week1.Add(thursday);
                monthMay2020.Add(week1);


                // SelectedNotebook
                notebook.Add(year2018);
                notebook.Add(year2019);
                notebook.Add(year2020);

                model.RootNode.Add(notebook);
                return model;
            }
        }

        public static NodeViewModel Node
        {
            get
            {
                var node = new NodeViewModel();
                var model = new NodeModel
                {
                    Name = "May",
                    Context = "Month",
                    Created = DateTime.Now
                };

                node.Model = model;
                node.ContextList = new ObservableCollection<string>();
                node.ContextList.AddRange(ModelRules.GlobalContextList);
                return node;
            }
        }

    }
}