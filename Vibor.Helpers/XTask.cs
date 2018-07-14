// Decompiled with JetBrains decompiler
// Type: Vibor.Helpers.XTask
// Assembly: Vibor.Helpers, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: E29329B7-F05A-4CC7-B834-7BAFB4348D90
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Helpers.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Vibor.Helpers
{
  public class XTask
  {
    private static readonly ILog Log = XLogger.GetLogger();

    public static async void RunAsync<T>(IList<T> files, Action<T> action, Action<int> progress)
    {
      await XTask.RunParallelAsync<T>(files, action, (Action) (() => progress(0)), progress, (Action) (() => progress(0)));
    }

    public static async void RunAsync<T>(IList<T> files, Action<T> action, Action started = null, Action<int> changed = null, Action completed = null)
    {
      await XTask.RunParallelAsync<T>(files, action, started, changed, completed);
    }

    public static Task RunParallelAsync<T>(IList<T> files, Action<T> action, Action started = null, Action<int> changed = null, Action completed = null)
    {
      return Task.Run((Action) (() => XTask.RunParallel<T>(files, action, started, changed, completed)));
    }

    public static void RunParallel<T>(IList<T> files, Action<T> action, Action started = null, Action<int> changed = null, Action completed = null)
    {
      if (started != null)
        started();
      int ii = 0;
      int step = files.Count > 100 ? (int) ((double) files.Count / 100.0 + 0.5) : 1;
      ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = -1 };
      Parallel.ForEach<T>((IEnumerable<T>) files, parallelOptions, (Action<T, ParallelLoopState, long>) ((f, pls, i) =>
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
          XTask.Log.Error(ex);
        }
      }));
      if (completed == null)
        return;
      completed();
    }

    public static void AddToList<T>(ICollection<T> list, T task) where T : XTask.ITask<T>
    {
      list.Add(task);
      foreach (T subTask in (Collection<T>) task.SubTasks)
        XTask.AddToList<T>(list, subTask);
    }

    public interface ITask<T>
    {
      ObservableCollection<T> SubTasks { get; }
    }
  }
}
