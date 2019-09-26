using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShutterUI
{
    public static class ShutterCommands
    {
        public static readonly RoutedUICommand Start = new RoutedUICommand
            (
                "Start",
                "Start",
                typeof(ShutterCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4)
                }
            );
        public static readonly RoutedUICommand Open = new RoutedUICommand
           (
               "Open",
               "Open",
               typeof(ShutterCommands),
               new InputGestureCollection()
               {
                    new KeyGesture(Key.F3)
               }
           );

        //Define more commands here, just like the one above
    }
}