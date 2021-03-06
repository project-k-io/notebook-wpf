using ProjectK.Notebook.Models;

namespace ProjectK.Notebook.ViewModels.Extensions;

public static class TaskModelExtensions
{
    public static bool IsSubTypeSleep(this TaskModel model)
    {
        if (string.IsNullOrEmpty(model.SubType))
            return false;

        return model.SubType.ToUpper().Contains("SLEEP");
    }

    public static bool IsSubTypeSleep(this TaskViewModel model)
    {
        if (string.IsNullOrEmpty(model.SubType))
            return false;

        return model.SubType.ToUpper().Contains("SLEEP");
    }
}