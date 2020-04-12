﻿using System;
using System.ComponentModel;
using System.Windows;
using Buffer.Basic;
using Controller.MainWindow;
using CustomElements.SizeSavedWindow;
using Errors;



namespace View.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowController _controller;
        public MainWindow(MainWindowController controller)
        {
            _controller = controller;
            DataContext = _controller;
            InitializeComponent();

            SizeSavedWindow.addToSizeSavedWindows(this);
            ErrorWindow.MainWindow = this;

        }


        private void ShutdownApplication(object sender, CancelEventArgs e)
        {
            //Evtl hardware ausschalten? also aDwinSystem1.Processes[1].Stop(); ??
            //Buffer.HardwareManager.HardwareManager

            if (_controller.OutputHandler.OutputLoopState != OutputHandler.OutputLoopStates.Sleeping)
            {
                MessageBoxResult result =
                    MessageBox.Show(
                        "The hardware Output is still in progress. Do you really want to Close this application?",
                        "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                if (result == MessageBoxResult.Yes)
                {
                    HardwareAdWin.HardwareAdWin.ControlAdwinProcess.StopAdwin();
                    Application.Current.Shutdown();
                }
            }
            Application.Current.Shutdown();

        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // show info when in debugging mode
            if (Global.GetHardwareType() == Model.Settings.HW_TYPES.AdWin_Simulator || !Global.CanAccessDatabase())
            {
                String output = "DEBUG MODE";
                if (Global.GetHardwareType() == Model.Settings.HW_TYPES.AdWin_Simulator)
                    output += "\n * no hardware Output";
                if (!Global.CanAccessDatabase())
                    output += "\n * no data will be written into the database";
                MessageBox.Show(output);
            }
        }

    }
}