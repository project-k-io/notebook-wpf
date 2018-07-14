// Decompiled with JetBrains decompiler
// Type: Generic.Models.XKeyboard
// Assembly: Vibor.Generic.ViewModels, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 18134161-73B0-45D8-9612-67C25563536B
// Assembly location: C:\Users\alan\Downloads\Ver 1.1.8\Debug\Vibor.Generic.ViewModels.dll

using System.Windows.Input;

namespace Vibor.View.Helpers
{
    public class XKeyboard
    {
        public static bool IsCtrlShiftPressed => (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) ==
                                                 (ModifierKeys.Control | ModifierKeys.Shift);

        public static bool IsShiftPressed => (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        public static bool IsControlPressed => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
    }
}