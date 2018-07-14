// Decompiled with JetBrains decompiler
// Type: ProjectApp.Commands
// Assembly: ProjectApp, Version=1.1.8.29131, Culture=neutral, PublicKeyToken=null
// MVID: A7331AD2-AF8A-4A84-BF9D-60C36001D1E0
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\ProjectApp.exe

using System.Windows.Input;

namespace ProjectApp
{
  public class Commands
  {
    public static RoutedUICommand Clear { get; private set; }

    public static RoutedUICommand Edit { get; private set; }

    public static RoutedUICommand FixTime { get; private set; }

    public static RoutedUICommand ExtractContext { get; private set; }

    public static RoutedUICommand FixContext { get; private set; }

    public static RoutedUICommand FixTitles { get; private set; }

    public static RoutedUICommand FixTypes { get; private set; }

    public static RoutedUICommand CopyTask { get; private set; }

    public static RoutedUICommand ContinueTask { get; private set; }

    static Commands()
    {
      Commands.Clear = new RoutedUICommand("Clear Project", nameof (Clear), typeof (Commands));
      Commands.Edit = new RoutedUICommand("Edit Project", nameof (Edit), typeof (Commands));
      Commands.FixTime = new RoutedUICommand("FixT ime", nameof (FixTime), typeof (Commands));
      Commands.ExtractContext = new RoutedUICommand("Extract Context", nameof (ExtractContext), typeof (Commands));
      Commands.FixContext = new RoutedUICommand("Fix Context", nameof (FixContext), typeof (Commands));
      Commands.FixTitles = new RoutedUICommand("Fix Titles", nameof (FixTitles), typeof (Commands));
      Commands.FixTypes = new RoutedUICommand("Fix Types", nameof (FixTypes), typeof (Commands));
      Commands.CopyTask = new RoutedUICommand("Copy Task    ", "CopyTask    ", typeof (Commands));
      Commands.ContinueTask = new RoutedUICommand("Continue Task", nameof (ContinueTask), typeof (Commands));
    }
  }
}
