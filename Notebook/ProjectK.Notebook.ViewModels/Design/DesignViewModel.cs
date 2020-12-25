using System;
using ProjectK.Notebook.Domain;

namespace ProjectK.Notebook.ViewModels.Design
{
    public class DesignViewModel : MainViewModel
    {
        public DesignViewModel()
        {
            var notebook = new NodeViewModel
            {
                IsExpanded = true,
                Name = "Time Tracker",
                Context = "App"
            };

            // 2018
            var year2018 = new NodeViewModel {Name = Title = "2018", Context = "Year"};
            var monthOct2018 = new NodeViewModel {Name = "October", Context = "Month"};
            year2018.Add(monthOct2018);

            // 2019
            var year2019 = new NodeViewModel {Name = "2019", Context = "Year"};
            var monthJan2019 = new NodeViewModel {Name = "January", Context = "Month"};
            year2019.Add(monthJan2019);

            // 2020
            var year2020 = new NodeViewModel {Name = "2020", Context = "Year", IsExpanded = true};
            var monthMay2020 = new NodeViewModel {Name = "May", Context = "Month", IsExpanded = true};
            year2020.Add(monthMay2020);

            // May 2020
            var week1 = new NodeViewModel {Name = "Week1", Context = "Week", IsExpanded = true};
            var thursday = new NodeViewModel {Name = "Thursday", Context = "Day", IsExpanded = true};

            var task1 = new TaskModel // Design
            {
                Name = "Dinner",
                Context = "TaskModel",
                DateStarted = new DateTime(2020, 5, 14, 20, 34, 0),
                DateEnded = new DateTime(2020, 5, 14, 21, 40, 0)
            };
            var task2 = new TaskModel // Design
            {
                Name = "Movie",
                Context = "TaskModel",
                DateStarted = new DateTime(2020, 5, 14, 21, 50, 0),
                DateEnded = new DateTime(2020, 5, 14, 23, 20, 0)
            };

            var node1 = new NodeViewModel
            {
                Model = task1,
                IsExpanded = true
            };

            var node2 = new NodeViewModel
            {
                Model = task2,
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

            RootTask.Add(notebook);
        }
    }
}