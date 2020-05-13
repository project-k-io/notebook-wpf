using System.Windows.Input;

namespace ProjectK.View.Helpers
{
    public class XKeyboard
    {
        public static bool IsCtrlShiftPressed =>
            (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) ==
            (ModifierKeys.Control | ModifierKeys.Shift);

        public static bool IsShiftPressed => (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        public static bool IsControlPressed => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
    }
}