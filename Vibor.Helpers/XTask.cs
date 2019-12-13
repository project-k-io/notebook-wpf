using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Vibor.Helpers
{
    public class XTask
    {
        private static readonly ILogger Logger = LogManager.GetLogger<XTask>();

        public static async void RunAsync<T>(IList<T> files, Action<T> action, Action<int> progress)
        {
            await RunParallelAsync(files, action, () => progress(0), progress, () => progress(0));
        }

        public static async void RunAsync<T>(IList<T> files, Action<T> action, Action started = null,
            Action<int> changed = null, Action completed = null)
        {
            await RunParallelAsync(files, action, started, changed, completed);
        }

        public static Task RunParallelAsync<T>(IList<T> files, Action<T> action, Action started = null,
            Action<int> changed = null, Action completed = null)
        {
            return Task.Run(() => RunParallel(files, action, started, changed, completed));
        }

        public static void RunParallel<T>(IList<T> files, Action<T> action, Action started = null,
            Action<int> changed = null, Action completed = null)
        {
            started?.Invoke();
            var ii = 0;
            var step = files.Count > 100 ? (int) (files.Count / 100.0 + 0.5) : 1;
            var parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = -1};
            Parallel.ForEach(files, parallelOptions, (f, pls, i) =>
            {
                try
                {
                    action(f);
                    Interlocked.Add(ref ii, 1);
                    if (ii % step != 0 || changed == null)
                        return;
                    changed(ii / step);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            });
            completed?.Invoke();
        }

        public static void AddToList<T>(ICollection<T> list, T task) where T : ITask<T>
        {
            list.Add(task);
            foreach (var subTask in task.SubTasks)
                AddToList(list, subTask);
        }

        public interface ITask<T>
        {
            ObservableCollection<T> SubTasks { get; }
        }
    }
}