using System;
using ProjectK.Notebook.Models;
using ProjectK.Notebook.ViewModels.Enums;

namespace ProjectK.Notebook.ViewModels.Design
{
    public class DesignMainViewModel : MainViewModel
    {

        public DesignMainViewModel()
        {
            var notebook = CreateNotebook();
            RootNode.Add(notebook);
        }


        public static NodeViewModel CreateNotebook()
        {
            var notebook = new NodeViewModel { Model = new NotebookModel { Name = "Time Tracker", Context = "App" }, IsExpanded = true };

            // 2018
            var year2018 = new NodeViewModel { Model = new NodeModel { Name = "2018", Context = "Year", Created = new DateTime(2018, 10, 1) } };
            var monthOct2018 = new NodeViewModel { Model = new NodeModel { Name = "October", Context = "Month", Created = new DateTime(2018, 10, 1) } };
            year2018.Add(monthOct2018);

            // 2019
            var year2019 = new NodeViewModel { Model = new NodeModel { Name = "2019", Context = "Year", Created = new DateTime(2019, 1, 10) } };
            var monthJan2019 = new NodeViewModel { Model = new NodeModel { Name = "January", Context = "Month", Created = new DateTime(2019, 1, 10) } };
            year2019.Add(monthJan2019);

            // 2020
            var year2020 = new NodeViewModel { Model = new NodeModel { Name = "2020", Context = "Year", Created = new DateTime(2020, 5, 3) }, IsExpanded = true };
            var monthMay2020 = new NodeViewModel { Model = new NodeModel { Name = "May", Context = "Month", Created = new DateTime(2020, 5, 3) }, IsExpanded = true };
            year2020.Add(monthMay2020);

            // May 2020
            var week1 = new NodeViewModel { Model = new NodeModel { Name = "Week1", Context = "Week", Created = new DateTime(2020, 5, 3) }, IsExpanded = true };
            var thursday = new NodeViewModel { Model = new NodeModel { Name = "Thursday", Context = "Day", Created = new DateTime(2020, 5, 3) }, IsExpanded = true };

            var node1 = new NodeViewModel { Model = new NodeModel { Name = "Dinner", Context = "TaskModel", Created = new DateTime(2020, 5, 14, 20, 34, 0), }, IsExpanded = true };
            var node2 = new NodeViewModel { Model = new NodeModel { Name = "Movie", Context = "TaskModel", Created = new DateTime(2020, 5, 14, 21, 50, 0), }, Modified = ModifiedStatus.Modified, IsExpanded = true };
            thursday.Add(node1);
            thursday.Add(node2);
            week1.Add(thursday);
            monthMay2020.Add(week1);
            // Years
            notebook.Add(year2018);
            notebook.Add(year2019);
            notebook.Add(year2020);

            return notebook;
        }
    }
}