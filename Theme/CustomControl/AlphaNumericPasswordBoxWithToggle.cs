using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Theme.CustomControl
{
    public class AlphaNumericPasswordBoxWithToggle : CommonPasswordBoxWithToggle
    {
        protected static Regex _passwordRegex = new Regex("^[a-zA-Z0-9]+$");
        public static Regex PasswordRegex
        {
            get => _passwordRegex;
            set
            {
                _passwordRegex = value;
            }
        }

        protected override void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pasteText = (string)e.DataObject.GetData(typeof(string));
                if (!PasswordRegex.IsMatch(pasteText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        protected override void PARTPasswordBoxAndTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Allow control keys(Back, Delete, Directional keys)
            if (e.Key == Key.Back || e.Key == Key.Delete ||
                e.Key == Key.Left || e.Key == Key.Right ||
                Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                return;
            }

            // Check if it is a letter or number
            if (!((e.Key >= Key.A && e.Key <= Key.Z) ||
                  (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                  (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)))
            {
                e.Handled = true;
            }
        }

        protected override void PARTPasswordBoxAndTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !PasswordRegex.IsMatch(e.Text);
        }
    }
}
