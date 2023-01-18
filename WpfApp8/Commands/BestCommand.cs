using System.Windows.Input;

namespace WpfApp8.Commands
{
    internal class BestCommand 
    {
        public static RoutedUICommand QueryCommand { get; }

        static BestCommand()
        {
            InputGestureCollection input = new InputGestureCollection();
            input.Add(new KeyGesture(Key.Q, ModifierKeys.Control, "Ctrl+Q"));
            input.Add(new MouseGesture(MouseAction.LeftClick,ModifierKeys.Control));
            QueryCommand = new RoutedUICommand("Query", "Query", typeof(BestCommand), input);
        }
    }
}
