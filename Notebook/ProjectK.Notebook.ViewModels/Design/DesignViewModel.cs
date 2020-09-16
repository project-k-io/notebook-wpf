using System;

namespace ProjectK.Notebook.ViewModels.Design
{
    public class DesignViewModel : MainViewModel
    {
        public DesignViewModel()
        {
            var notebook = new NodeViewModel {Title = "Time Tracker", Context = "App", IsExpanded = true};

            // 2018
            var year2018 = new NodeViewModel {Title = "2018", Context = "Year"};
            var monthOct2018 = new NodeViewModel {Title = "October", Context = "Month"};
            year2018.Add(monthOct2018);

            // 2019
            var year2019 = new NodeViewModel {Title = "2019", Context = "Year"};
            var monthJan2019 = new NodeViewModel {Title = "January", Context = "Month"};
            year2019.Add(monthJan2019);

            // 2020
            var year2020 = new NodeViewModel {Title = "2020", Context = "Year", IsExpanded = true};
            var monthMay2020 = new NodeViewModel {Title = "May", Context = "Month", IsExpanded = true};
            year2020.Add(monthMay2020);

            // May 2020
            var week1 = new NodeViewModel {Title = "Week1", Context = "Week", IsExpanded = true};
            var thursday = new NodeViewModel {Title = "Thursday", Context = "Day", IsExpanded = true};
            var task1 = new NodeViewModel
            {
                Title = "Dinner", Context = "Task", IsExpanded = true,
#if AK
                DateStarted = new DateTime(2020, 5, 14, 20, 34, 0),
                DateEnded = new DateTime(2020, 5, 14, 21, 40, 0)
#endif
            };
            var task2 = new NodeViewModel
            {
                Title = "Movie", Context = "Task", IsExpanded = true,
#if AK
                DateStarted = new DateTime(2020, 5, 14, 21, 50, 0),
                DateEnded = new DateTime(2020, 5, 14, 23, 20, 0)
#endif
            };
            thursday.Add(task1);
            thursday.Add(task2);
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