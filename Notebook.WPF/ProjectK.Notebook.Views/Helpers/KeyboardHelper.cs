using System.Windows.Input;
using ProjectK.Notebook.ViewModels.Enums;

namespace ProjectK.Notebook.Views.Helpers
{
    public class KeyboardHelper
    {
        public static bool IsCtrlShiftPressed =>
            (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) ==
            (ModifierKeys.Control | ModifierKeys.Shift);

        public static bool IsShiftPressed => (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        public static bool IsControlPressed => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

        public static KeyboardKeys GetKeyState(Key key)
        {
            var keyStates = KeyboardKeys.None;
            switch (key)
            {
                case Key.Left:
                    keyStates = KeyboardKeys.Left;
                    break;
                case Key.Up:
                    keyStates = KeyboardKeys.Up;
                    break;
                case Key.Right:
                    keyStates = KeyboardKeys.Right;
                    break;
                case Key.Down:
                    keyStates = KeyboardKeys.Down;
                    break;
                case Key.Insert:
                    keyStates = KeyboardKeys.Insert;
                    break;
                case Key.Delete:
                    keyStates = KeyboardKeys.Delete;
                    break;
            }

            return keyStates;
        }
        public static KeyboardStates KeyboardState
        {
            get
            {
                var keyboardStates = KeyboardStates.None;
                if (IsCtrlShiftPressed)
                    keyboardStates = KeyboardStates.IsCtrlShiftPressed;
                else if (IsShiftPressed)
                    keyboardStates = KeyboardStates.IsShiftPressed;
                else if (IsControlPressed)
                    keyboardStates = KeyboardStates.IsControlPressed;
                return keyboardStates;
            }
        }
    }
}