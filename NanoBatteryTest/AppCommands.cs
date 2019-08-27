using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NanoBatteryTest.Commands
{
    public class BanchCommands
    {
        private static RoutedUICommand start;
        private static RoutedUICommand stop;
        private static RoutedUICommand connect;
        private static RoutedUICommand exit;
        private static RoutedUICommand home;
        private static RoutedUICommand back;
        private static RoutedUICommand par;
        private static RoutedUICommand msg;
        private static RoutedUICommand dyn;
        private static RoutedUICommand save;
        private static RoutedUICommand extend;
        private static RoutedUICommand resetparams;
        private static RoutedUICommand paramstodemo;


        static BanchCommands()
        {
            // Initialize the command.
            start = new RoutedUICommand("Start automatic cycle", "Start", typeof(BanchCommands), null);

            stop = new RoutedUICommand("Stop automatic cycle", "Stop", typeof(BanchCommands), null);

            connect = new RoutedUICommand("Check facility", "Connect", typeof(BanchCommands), null);

            exit = new RoutedUICommand("Close application", "Exit", typeof(BanchCommands), null);

            home = new RoutedUICommand("Go to Home page", "Home", typeof(BanchCommands), null);

            back = new RoutedUICommand("Go back", "Back", typeof(BanchCommands), null);

            par = new RoutedUICommand("Process parameters", "Parameters", typeof(BanchCommands), null);

            msg = new RoutedUICommand("Show messages", "Messages", typeof(BanchCommands), null);

            dyn = new RoutedUICommand("Show dynamics", "Dynamic", typeof(BanchCommands), null);

            save = new RoutedUICommand("Save data", "Save", typeof(BanchCommands), null);

            extend = new RoutedUICommand("Switch to extended mode", "Extend", typeof(BanchCommands), null);

            resetparams = new RoutedUICommand("Reset work parameters to default", "Reset parameters", typeof(BanchCommands), null);

            paramstodemo = new RoutedUICommand("Reset work parameters to demo", "Demo parameters", typeof(BanchCommands), null);
        }



        public static RoutedUICommand Start
        {
            get { return start; }
        }

        public static RoutedUICommand Connect
        {
            get { return connect; }
        }

        public static RoutedUICommand ExtendedMode
        {
            get { return extend; }
        }

        public static RoutedUICommand Parameters
        {
            get { return par; }
        }
        public static RoutedUICommand RessetParameters
        {
            get { return resetparams; }
        }

        public static RoutedUICommand DemoParameters
        {
            get { return paramstodemo; }
        }

        public static RoutedUICommand Home
        {
            get { return home; }
        }

        public static RoutedUICommand Back
        {
            get { return back; }
        }

        public static RoutedUICommand Stop
        {
            get { return stop; }
        }

        public static RoutedUICommand Exit
        {
            get { return exit; }
        }

        public static RoutedUICommand Messages
        {
            get { return msg; }
        }

        public static RoutedUICommand Dynamic
        {
            get { return dyn; }
        }

        public static RoutedUICommand Save
        {
            get { return save; }
        }
    }
}
